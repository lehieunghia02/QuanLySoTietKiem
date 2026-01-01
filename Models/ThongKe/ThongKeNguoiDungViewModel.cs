namespace QuanLySoTietKiem.Models.ThongKe
{
  public class ThongKeNguoiDungViewModel
  {
    public List<TopUserViewModel> TopUsers {get;set;} = new List<TopUserViewModel>();

    // Phân bố số lượng sổ
      public List<string> NhomSoLuongSo { get; set; } = new List<string>();
      public List<int> SoLuongNguoiDung { get; set; } = new List<int>();
      public List<string> MauSac { get; set; } = new List<string>();
  }
  public class TopUserViewModel
  {
    public string UserId {get;set;} = string.Empty; 
    public string UserName {get;set;} = string.Empty; 
    public string FullName {get;set;} = string.Empty; 
    public int SoLuongSo {get;set;} 
    public double TongTienGui {get;set;}
  }
}