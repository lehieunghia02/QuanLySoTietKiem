namespace QuanLySoTietKiem.Models.Admin
{
    public class InterestRate
    {
        public int Id { get; set; }
        public string NameOfInterestRate { get; set; } = string.Empty;
        public decimal Rate { get; set; } 
        public double Term { get; set; } 
        public int TermInMonths { get; set; }
        public decimal MinimumDepositAmount { get; set; }
    }
}
