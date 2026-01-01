using System.Text;
using System.Security.Cryptography;
using QuanLySoTietKiem.Models;
using System.Net;
using System.Globalization;
using QuanLySoTietKiem.Services.Interfaces;
using System.Linq;

namespace QuanLySoTietKiem.Services.Implementations
{
  public class VNPayService : IVNPayService
  {
    private readonly IConfiguration _configuration;

    public VNPayService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
    {
      var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
      var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
      var tick = DateTime.Now.Ticks.ToString();
      var pay = new VNPayLibrary();

      pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
      pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
      pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
      pay.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());
      pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
      pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
      pay.AddRequestData("vnp_IpAddr", GetIpAddress(context));
      pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
      pay.AddRequestData("vnp_OrderInfo", $"{model.Name} thanh toan hoa don: {model.OrderDescription}");
      pay.AddRequestData("vnp_OrderType", model.OrderType);
      pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]);
      pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

      var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

      return paymentUrl;
    }

    public PaymentInformationModel PaymentExecute(IQueryCollection collections)
    {
      var pay = new VNPayLibrary();
      var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

      return new PaymentInformationModel()
      {
        OrderDescription = response.OrderDescription,
        Amount = (decimal)Convert.ToDouble(response.Amount) / 100,
       
        OrderType = response.OrderType,
        // Status = response.TransactionStatus,
        CreatedDate = response.PayDate,
        Name = collections["vnp_Bill_FirstName"]
      };
    }

    private string GetIpAddress(HttpContext context)
    {
      string ipAddress;
      try
      {
        ipAddress = context.Connection.RemoteIpAddress.ToString();
        if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
        {
          ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "127.0.0.1";
        }
      }
      catch (Exception)
      {
        ipAddress = "127.0.0.1";
      }
      return ipAddress;
    }
  }

  public class VNPayLibrary
  {
    private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
      if (!String.IsNullOrEmpty(value))
      {
        _requestData.Add(key, value);
      }
    }

    public string GetResponseData(string key)
    {
      string retValue;
      if (_responseData.TryGetValue(key, out retValue))
      {
        return retValue;
      }
      else
      {
        return string.Empty;
      }
    }

    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
      StringBuilder data = new StringBuilder();
      foreach (KeyValuePair<string, string> kv in _requestData)
      {
        if (!String.IsNullOrEmpty(kv.Value))
        {
          data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        }
      }
      string queryString = data.ToString();

      baseUrl += "?" + queryString;
      String signData = queryString;
      if (signData.Length > 0)
      {
        signData = signData.Remove(data.Length - 1, 1);
      }
      string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);
      baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

      return baseUrl;
    }

    public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
    {
      var vnpayData = new PaymentResponseModel();
      foreach (var (key, value) in collection)
      {
        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
        {
          _responseData.Add(key, value.ToString());
        }
      }

      vnpayData.OrderDescription = GetResponseData("vnp_OrderInfo");
      vnpayData.TransactionStatus = GetResponseData("vnp_TransactionStatus");
      vnpayData.OrderType = GetResponseData("vnp_OrderType");
      vnpayData.Amount = GetResponseData("vnp_Amount");

      string payDate = GetResponseData("vnp_PayDate");
      if (!string.IsNullOrEmpty(payDate))
      {
        vnpayData.PayDate = DateTime.ParseExact(payDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
      }
      else
      {
        vnpayData.PayDate = DateTime.Now;
      }

      return vnpayData;
    }
  }

  public class VnPayCompare : IComparer<string>
  {
    public int Compare(string x, string y)
    {
      if (x == y) return 0;
      if (x == null) return -1;
      if (y == null) return 1;
      var comparer = StringComparer.InvariantCulture;
      return comparer.Compare(x, y);
    }
  }

  public class Utils
  {
    public static String HmacSHA512(string key, String inputData)
    {
      var hash = new StringBuilder();
      byte[] keyBytes = Encoding.UTF8.GetBytes(key);
      byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
      using (var hmac = new HMACSHA512(keyBytes))
      {
        byte[] hashValue = hmac.ComputeHash(inputBytes);
        foreach (var theByte in hashValue)
        {
          hash.Append(theByte.ToString("x2"));
        }
      }
      return hash.ToString();
    }
  }

  public class PaymentResponseModel
  {
    public string OrderDescription { get; set; }
    public string TransactionStatus { get; set; }
    public string OrderType { get; set; }
    public string Amount { get; set; }
    public DateTime PayDate { get; set; }
  }
}