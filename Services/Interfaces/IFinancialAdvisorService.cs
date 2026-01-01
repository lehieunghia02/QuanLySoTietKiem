public interface IFinancialAdvisorService
{
  Task<string> GetFinancialAdvice(string userQuestion, string userId);
}