namespace Helpers 
{
  public static class DateHelper 
  {
    public static string GetMonthNow()
    {
      return DateTime.Now.Month.ToString();
    }
    public static string GetYearNow()
    {
      return DateTime.Now.Year.ToString();
    }
  }
}