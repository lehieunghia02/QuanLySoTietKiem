using Microsoft.ML.Data;
namespace QuanLySoTietKiem.Models.SavingPredictionModel
{
  public class SavingPredictionInput
  {
    [LoadColumn(0)]
    public float InitialAmount { get; set; }

    [LoadColumn(1)]
    public float InterestRate { get; set; }

    [LoadColumn(2)]
    public float Term { get; set; }  // Kỳ hạn (tháng)

    [LoadColumn(3)]
    public float MonthlyDeposit { get; set; }
  }
  public class SavingPredictionOutput
  {
    [ColumnName("Score")]
    public float PredictedBalance { get; set; }
  }
}