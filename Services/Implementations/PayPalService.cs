using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services.Implementations
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayPalService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;

        // Các thông tin cấu hình PayPal
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode; // sandbox hoặc live
        private readonly string _returnUrl;
        private readonly string _cancelUrl;

        public PayPalService(
            IConfiguration configuration,
            ILogger<PayPalService> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _httpClient = httpClientFactory.CreateClient("PayPal");


            _clientId = _configuration["PayPal:ClientId"];
            _clientSecret = _configuration["PayPal:ClientSecret"];
            _mode = _configuration["PayPal:Mode"];

            // Xác định API URL dựa trên mode
            string baseUrl = _mode.ToLower() == "live"
                ? "https://api.paypal.com"
                : "https://api.sandbox.paypal.com";

            _httpClient.BaseAddress = new Uri(baseUrl);

            // URL callback
            _returnUrl = _configuration["PayPal:ReturnUrl"];
            _cancelUrl = _configuration["PayPal:CancelUrl"];
        }

        /// <summary>
        /// Lấy access token từ PayPal API
        /// </summary>
        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                // Tạo request để lấy access token
                var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"))
                );

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                });

                request.Content = content;

                // Gửi request
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Đọc response
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return tokenResponse.GetProperty("access_token").GetString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy access token từ PayPal");
                throw;
            }
        }

        /// <summary>
        /// Tạo URL thanh toán PayPal
        /// </summary>
        public async Task<string> CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context)
        {
            try
            {
                // Lấy access token
                string accessToken = await GetAccessTokenAsync();

                // Lấy thông tin người dùng hiện tại
                var userId = context.User.Identity.Name;

                // Tạo URL callback với userId
                var returnUrl = $"{_returnUrl}?userId={userId}";
                var cancelUrl = $"{_cancelUrl}?userId={userId}";

                // Tạo request để tạo payment
                var request = new HttpRequestMessage(HttpMethod.Post, "/v1/payments/payment");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Chuyển đổi số tiền từ VND sang USD (tỷ giá tạm tính 1 USD = 24,000 VND)
                decimal amountInUsd = Math.Round(model.Amount / 24000m, 2);

                // Tạo nội dung payment
                var payment = new
                {
                    intent = "sale",
                    payer = new
                    {
                        payment_method = "paypal"
                    },
                    transactions = new[]
                      {
                        new
                        {
                          amount = new
                          {
                            total = amountInUsd.ToString("0.00", CultureInfo.InvariantCulture),
                            currency = "USD"
                          },
                          description = model.OrderDescription
                        }
                      },
                    redirect_urls = new
                    {
                        return_url = returnUrl,
                        cancel_url = cancelUrl
                    }
                };

                // Chuyển đổi object thành JSON
                var json = JsonSerializer.Serialize(payment);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi request
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Đọc response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("PayPal response: {Response}", responseContent);

                var paymentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Lấy approval_url từ response
                var links = paymentResponse.GetProperty("links").EnumerateArray();
                string approvalUrl = "";

                foreach (var link in links)
                {
                    if (link.GetProperty("rel").GetString() == "approval_url")
                    {
                        approvalUrl = link.GetProperty("href").GetString();
                        break;
                    }
                }

                return approvalUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo URL thanh toán PayPal");
                throw;
            }
        }

        /// <summary>
        /// Xử lý kết quả thanh toán từ PayPal
        /// </summary>
        public async Task<PayPalPaymentResponse> ExecutePaymentAsync(string paymentId, string payerId, HttpContext context)
        {
            try
            {
                // Lấy access token
                string accessToken = await GetAccessTokenAsync();

                // Tạo request để execute payment
                var request = new HttpRequestMessage(HttpMethod.Post, $"/v1/payments/payment/{paymentId}/execute");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Tạo nội dung execute payment
                var executeObject = new
                {
                    payer_id = payerId
                };

                // Chuyển đổi object thành JSON
                var json = JsonSerializer.Serialize(executeObject);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi request
                var response = await _httpClient.SendAsync(request);

                // Đọc response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("PayPal response: {Response}", responseContent);

                var paymentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Tạo response model
                var result = new PayPalPaymentResponse
                {
                    PaymentTime = DateTime.Now,
                    PaymentMethod = "PayPal"
                };

                // Kiểm tra kết quả
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Lấy thông tin từ response
                        var state = paymentResponse.GetProperty("state").GetString();

                        if (state == "approved")
                        {
                            // Lấy thông tin giao dịch
                            var transaction = paymentResponse.GetProperty("transactions")[0];
                            var amount = transaction.GetProperty("amount");

                            // Lấy số tiền (USD)
                            var amountValue = decimal.Parse(
                                amount.GetProperty("total").GetString(),
                                CultureInfo.InvariantCulture
                            );

                            // Chuyển đổi từ USD sang VND
                            var amountInVnd = amountValue * 24000m;

                            // Lấy ID giao dịch
                            var transactionId = paymentResponse.GetProperty("id").GetString();

                            // Cập nhật kết quả
                            result.TransactionId = transactionId;
                            result.Status = "success";
                            result.Amount = amountInVnd;
                            result.Description = transaction.GetProperty("description").GetString();

                            // Lấy userId từ query string
                            var userId = context.Request.Query["userId"].ToString();
                            result.UserId = userId;

                            // Cập nhật số dư tài khoản
                            if (!string.IsNullOrEmpty(userId))
                            {
                                await UpdateUserBalanceAsync(userId, amountInVnd);
                            }
                        }
                        else
                        {
                            result.Status = "failed";
                            result.ErrorMessage = "Thanh toán không được chấp nhận";
                            _logger.LogWarning("PayPal payment not approved. State: {State}", state);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Status = "failed";
                        result.ErrorMessage = "Lỗi khi xử lý dữ liệu thanh toán";
                        _logger.LogError(ex, "Lỗi khi xử lý dữ liệu thanh toán PayPal");
                    }
                }
                else
                {
                    result.Status = "failed";

                    // Phân tích lỗi cụ thể từ PayPal
                    if (paymentResponse.TryGetProperty("name", out var errorName))
                    {
                        string errorCode = errorName.GetString();
                        switch (errorCode)
                        {
                            case "INSTRUMENT_DECLINED":
                                result.ErrorMessage = "Phương thức thanh toán bị từ chối. Vui lòng thử phương thức khác.";
                                break;
                            case "INSUFFICIENT_FUNDS":
                                result.ErrorMessage = "Tài khoản không đủ tiền để thực hiện giao dịch.";
                                break;
                            default:
                                result.ErrorMessage = "Lỗi khi xử lý thanh toán: " + errorCode;
                                break;
                        }
                    }
                    else
                    {
                        result.ErrorMessage = "Lỗi khi xử lý thanh toán";
                    }

                    // Ghi log lỗi
                    _logger.LogError("Lỗi PayPal: {Response}", responseContent);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý kết quả thanh toán PayPal");

                return new PayPalPaymentResponse
                {
                    Status = "failed",
                    ErrorMessage = "Đã xảy ra lỗi trong quá trình xử lý thanh toán",
                    PaymentTime = DateTime.Now,
                    PaymentMethod = "PayPal"
                };
            }
        }

        /// <summary>
        /// Cập nhật số dư tài khoản người dùng
        /// </summary>
        public async Task<bool> UpdateUserBalanceAsync(string userId, decimal amount)
        {
            try
            {
                // Tìm người dùng
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogError("Không tìm thấy người dùng với ID: {UserId}", userId);
                    return false;
                }

                // Cập nhật số dư
                user.SoDuTaiKhoan += Convert.ToDouble(amount);

                // Lưu thay đổi
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Ghi log giao dịch
                    var transaction = new GiaoDich
                    {
                        UserId = userId,
                        LoaiGiaoDich = "Nạp tiền",
                        SoTien = amount, // Đã là decimal, không cần chuyển đổi
                        NgayGiaoDich = DateTime.Now,
                        MoTa = "Nạp tiền qua PayPal",
                        TrangThai = "Thành công"
                    };

                    _context.GiaoDichs.Add(transaction);
                    await _context.SaveChangesAsync();

                    return true;
                }

                _logger.LogError("Lỗi khi cập nhật số dư: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật số dư tài khoản");
                return false;
            }
        }
    }
}