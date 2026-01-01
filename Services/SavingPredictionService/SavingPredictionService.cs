using Microsoft.ML;
using Microsoft.ML.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.SavingPredictionModel;


public class SavingPredictionService
{
  private readonly MLContext _mlContext;
  private ITransformer _model;
  private readonly string _modelPath = Path.Combine(AppContext.BaseDirectory, "savings_prediction_model.zip");

  public SavingPredictionService()
  {
    _mlContext = new MLContext(seed: 0);
  }

  // public async Task TrainModel(IEnumerable<SoTietKiem> historicalData)
  // {
  //   // Chuyển đổi dữ liệu lịch sử thành training data
  //   var trainingData = historicalData.Select(s => new SavingPredictionInput
  //   {
  //     InitialAmount = (float)s.SoTien,
  //     InterestRate = (float)s.LaiSuatKyHan,
  //     Term = (float)s.KyHan,
  //     MonthlyDeposit = CalculateMonthlyDeposit(s)
  //   }).ToList();

  //   // Tạo pipeline cho model
  //   var pipeline = _mlContext.Transforms.Concatenate("Features",
  //           nameof(SavingPredictionInput.InitialAmount),
  //           nameof(SavingPredictionInput.InterestRate),
  //           nameof(SavingPredictionInput.Term),
  //           nameof(SavingPredictionInput.MonthlyDeposit))
  //       .Append(_mlContext.Regression.Trainers.FastForest());

  //   // Train model
  //   var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);
  //   _model = pipeline.Fit(trainingDataView);

  //   // Lưu model
  //   await SaveModelAsync();
  // }

  public async Task<float> PredictFutureBalance(SavingPredictionInput input)
  {
    // Load model nếu chưa có
    if (_model == null)
    {
      await LoadModelAsync();
    }

    // Tạo prediction engine
    var predEngine = _mlContext.Model.CreatePredictionEngine<SavingPredictionInput, SavingPredictionOutput>(_model);

    // Dự đoán
    var prediction = predEngine.Predict(input);
    return prediction.PredictedBalance;
  }

  private async Task SaveModelAsync()
  {
    await using var fs = new FileStream(_modelPath, FileMode.Create);
    _mlContext.Model.Save(_model, null, fs);
  }

  private async Task LoadModelAsync()
  {
    if (File.Exists(_modelPath))
    {
      await using var fs = new FileStream(_modelPath, FileMode.Open);
      _model = _mlContext.Model.Load(fs, out _);
    }
  }

  private float CalculateMonthlyDeposit(SavingAccount stk)
  {
    // Logic tính toán tiền gửi hàng tháng dựa trên lịch sử
    return 0; // Implement logic thực tế
  }
}