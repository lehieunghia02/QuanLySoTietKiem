using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Services.Interfaces;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace QuanLySoTietKiem.Services.Implementations
{
  public class GoogleAuthService : IGoogleAuthService
  {
    private readonly GoogleAuthSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        IOptions<GoogleAuthSettings> settings,
        IHttpClientFactory httpClientFactory,
        UserManager<ApplicationUser> userManager,
        ILogger<GoogleAuthService> logger)
    {
      _settings = settings.Value;
      _httpClientFactory = httpClientFactory;
      _userManager = userManager;
      _logger = logger;
    }

    /// <summary>
    /// Tạo URL xác thực Google OAuth
    /// </summary>
    public string GetAuthUrl(string returnUrl)
    {
      var queryParams = new Dictionary<string, string>
            {
                { "client_id", _settings.ClientId },
                { "redirect_uri", _settings.RedirectUri },
                { "response_type", "code" },
                { "scope", string.Join(" ", _settings.Scopes) },
                { "access_type", "offline" },
                { "state", returnUrl ?? "/" }
            };

      var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
      return $"{_settings.AuthorizationEndpoint}?{queryString}";
    }

    /// <summary>
    /// Xử lý callback từ Google OAuth
    /// </summary>
    public async Task<ClaimsPrincipal> HandleCallbackAsync(string code)
    {
      try
      {
        // Đổi code lấy token
        var tokenResponse = await ExchangeCodeForTokenAsync(code);
        if (tokenResponse == null)
        {
          _logger.LogError("Failed to exchange code for token");
          return null;
        }

        // Lấy thông tin người dùng từ token
        var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);
        if (userInfo == null)
        {
          _logger.LogError("Failed to get user info");
          return null;
        }

        // Tạo claims từ thông tin người dùng
        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.Name, userInfo.Name),
                    new Claim("picture", userInfo.Picture)
                };

        var identity = new ClaimsIdentity(claims, "Google");
        return new ClaimsPrincipal(identity);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error handling Google callback");
        return null;
      }
    }

    /// <summary>
    /// Tạo hoặc cập nhật thông tin người dùng từ Google
    /// </summary>
    public async Task<bool> CreateOrUpdateUserAsync(ClaimsPrincipal principal)
    {
      try
      {
        var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var name = principal.FindFirstValue(ClaimTypes.Name);
        var picture = principal.FindFirstValue("picture");

        // Tìm user theo Google ID
        var user = await _userManager.FindByLoginAsync("Google", googleId);

        // Nếu không tìm thấy, tìm theo email
        if (user == null)
        {
          user = await _userManager.FindByEmailAsync(email);
        }

        // Nếu vẫn không tìm thấy, tạo user mới
        if (user == null)
        {
          user = new ApplicationUser
          {
            UserName = email,
            Email = email,
            FullName = name ?? "",
            EmailConfirmed = true,
            AvatarUrl = picture
          };

          var result = await _userManager.CreateAsync(user);
          if (!result.Succeeded)
          {
            _logger.LogError("Failed to create user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
          }

          // Thêm login Google cho user
          result = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googleId, "Google"));
          if (!result.Succeeded)
          {
            _logger.LogError("Failed to add Google login: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
          }

          // Thêm role User cho user mới
          result = await _userManager.AddToRoleAsync(user, "User");
          if (!result.Succeeded)
          {
            _logger.LogError("Failed to add role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
          }
        }
        else
        {
          // Cập nhật thông tin nếu cần
          bool needUpdate = false;

          if (string.IsNullOrEmpty(user.FullName) || user.FullName != name)
          {
            user.FullName = name;
            needUpdate = true;
          }

          if (string.IsNullOrEmpty(user.AvatarUrl) || user.AvatarUrl != picture)
          {
            user.AvatarUrl = picture;
            needUpdate = true;
          }

          // Nếu user chưa có login Google, thêm vào
          var logins = await _userManager.GetLoginsAsync(user);
          if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleId))
          {
            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googleId, "Google"));
            if (!result.Succeeded)
            {
              _logger.LogError("Failed to add Google login: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
              return false;
            }
          }

          if (needUpdate)
          {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
              _logger.LogError("Failed to update user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
              return false;
            }
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating or updating user from Google");
        return false;
      }
    }

    #region Private Methods

    /// <summary>
    /// Đổi authorization code lấy access token
    /// </summary>
    private async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
      var client = _httpClientFactory.CreateClient();

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret },
                { "redirect_uri", _settings.RedirectUri },
                { "grant_type", "authorization_code" }
            });

      var response = await client.PostAsync(_settings.TokenEndpoint, content);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Failed to exchange code for token. Status: {StatusCode}", response.StatusCode);
        return null;
      }

      var responseContent = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<TokenResponse>(responseContent);
    }

    /// <summary>
    /// Lấy thông tin người dùng từ access token
    /// </summary>
    private async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
      var client = _httpClientFactory.CreateClient();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      var response = await client.GetAsync(_settings.UserInfoEndpoint);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Failed to get user info. Status: {StatusCode}", response.StatusCode);
        return null;
      }

      var responseContent = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<GoogleUserInfo>(responseContent);
    }

    #endregion

    #region Helper Classes

    private class TokenResponse
    {
      public string AccessToken { get; set; }
      public string RefreshToken { get; set; }
      public int ExpiresIn { get; set; }
      public string TokenType { get; set; }
      public string IdToken { get; set; }
    }

    private class GoogleUserInfo
    {
      public string Sub { get; set; }
      public string Name { get; set; }
      public string GivenName { get; set; }
      public string FamilyName { get; set; }
      public string Picture { get; set; }
      public string Email { get; set; }
      public bool EmailVerified { get; set; }
      public string Locale { get; set; }
    }

    #endregion
  }
}