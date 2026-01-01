namespace QuanLySoTietKiem.Models.SavingAccountTypeModel
{
    public class SavingAccountTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float InterestRate { get; set; }
        public int Term { get; set; }
        public int MinimumDeposit { get; set; }

    }
}
