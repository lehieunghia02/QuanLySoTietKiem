using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using OpenAI.Managers;
using OpenAI.Interfaces;
using OpenAI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;


public class FinancialAdvisorService : IFinancialAdvisorService
{
  private readonly IOpenAIService _openAiService;
  private readonly IMemoryCache _cache;
  private readonly ApplicationDbContext _context;
  private List<ChatMessage> _chatHistory = new List<ChatMessage>();
  private readonly string _systemPrompt = @"Bạn là trợ lý thông minh của website Quản lý sổ tiết kiệm. 
        Dưới đây là thông tin về website:

        THÔNG TIN CHUNG:
        - Website Quản lý sổ tiết kiệm giúp người dùng theo dõi và quản lý các khoản tiết kiệm
        - Người dùng có thể tạo tài khoản, mở sổ tiết kiệm, theo dõi lãi suất
        - Hỗ trợ nạp tiền qua VNPay và PayPal
        - Có tính năng dự đoán số tiền tiết kiệm trong tương lai

        TÍNH NĂNG CHÍNH:
        1. Quản lý sổ tiết kiệm:
           - Mở sổ tiết kiệm mới
           - Theo dõi lãi suất và kỳ hạn
           - Tính toán lãi tự động
           - Quản lý nhiều sổ tiết kiệm cùng lúc

        2. Nạp/Rút tiền:
           - Nạp tiền qua VNPay
           - Nạp tiền qua PayPal
           - Theo dõi lịch sử giao dịch
           - Rút tiền về tài khoản

        3. Tính năng bảo mật:
           - Xác thực email
           - Đăng nhập bằng Google
           - Bảo mật hai lớp
           - Khôi phục mật khẩu qua email

        4. Tiện ích:
           - Dự đoán số tiền tiết kiệm
           - Thống kê chi tiết
           - Biểu đồ theo dõi
           - Thông báo qua email

        Hãy trả lời các câu hỏi của người dùng một cách chi tiết, dễ hiểu và chính xác về website.
        Nếu không chắc chắn về thông tin nào, hãy đề nghị người dùng liên hệ admin để được hỗ trợ.";

  private readonly ILogger<FinancialAdvisorService> _logger;

  public FinancialAdvisorService(
    IOpenAIService openAiService,
    IMemoryCache cache,
    ApplicationDbContext context,
    ILogger<FinancialAdvisorService> logger)
  {
    _openAiService = openAiService;
    _cache = cache;
    _context = context;
    _logger = logger;
  }

  public async Task<string> GetFinancialAdvice(string userQuestion, string userId)
  {
    // Check cache first
    string cacheKey = $"chat_{userId}_{userQuestion.GetHashCode()}";
    if (_cache.TryGetValue(cacheKey, out string cachedResponse))
    {
      return cachedResponse;
    }

    var response = await GetOpenAIResponse(userQuestion);

    // Save to cache
    _cache.Set(cacheKey, response, TimeSpan.FromHours(1));

    // Save to history
    await SaveToHistory(userQuestion, response, userId);

    return response;
  }

  private async Task SaveToHistory(string question, string answer, string userId)
  {
    try
    {
      // Check if user exists
      var user = await _context.Users.FindAsync(userId);
      if (user == null)
      {
        _logger.LogWarning($"User not found with ID: {userId}");
        return;
      }

      var history = new QuanLySoTietKiem.Entity.ChatHistory
      {
        UserId = userId,
        Question = question,
        Answer = answer,
        CreatedAt = DateTime.UtcNow,
        User = user 
      };

      await _context.ChatHistories.AddAsync(history);
      await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError($"Error saving chat history: {ex.Message}");
    }
  }

  private async Task<string> GetOpenAIResponse(string userQuestion)
  {
    try
    {
      _chatHistory.Add(ChatMessage.FromUser(userQuestion));

      if (_chatHistory.Count > 10)
      {
        _chatHistory = _chatHistory.Skip(_chatHistory.Count - 10).ToList();
      }

      var messages = new List<ChatMessage>
      {
        ChatMessage.FromSystem(_systemPrompt)
      };
      messages.AddRange(_chatHistory);

      var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
      {
        Messages = messages,
        Model = Models.Gpt_3_5_Turbo,
        Temperature = (float)0.7,
        MaxTokens = 1000
      });

      if (completionResult.Successful)
      {
        var response = completionResult.Choices.First().Message.Content;
        _chatHistory.Add(ChatMessage.FromAssistant(response));
        return response;
      }
      else
      {
        return $"Xin lỗi, đã có lỗi xảy ra: {completionResult.Error?.Message}";
      }
    }
    catch (Exception ex)
    {
      return $"Xin lỗi, đã có lỗi xảy ra: {ex.Message}";
    }
  }
}