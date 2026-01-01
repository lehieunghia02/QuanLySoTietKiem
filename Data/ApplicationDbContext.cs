using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<LoaiSoTietKiem> LoaiSoTietKiems { get; set; }
        public DbSet<SavingAccount> SoTietKiems { get; set; }
        public DbSet<BaoCaoNgay> BaoCaoNgays { get; set; }
        public DbSet<BaoCaoThang> BaoCaoThangs { get; set; }
        public DbSet<HinhThucDenHan> HinhThucDenHans { get; set; }
        public DbSet<TransactionType> LoaiGiaoDichs { get; set; }
        public DbSet<GiaoDich> GiaoDichs { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define primary key for IdentityUserLogin<string>
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<BaoCaoThang>().Property(b => b.ChenhLech).HasPrecision(18, 2);
            modelBuilder.Entity<BaoCaoNgay>().Property(b => b.TongTienGui).HasPrecision(18, 2);
            modelBuilder.Entity<BaoCaoNgay>().Property(b => b.TongTienRut).HasPrecision(18, 2);
            modelBuilder.Entity<LoaiSoTietKiem>().Property(l => l.SoTienGuiToiThieu).HasPrecision(18, 2);
            modelBuilder.Entity<SavingAccount>().Property(s => s.SoDuSoTietKiem).HasPrecision(18, 2);
            modelBuilder.Entity<SavingAccount>().Property(s => s.SoTienGui).HasPrecision(18, 2);
            modelBuilder.Entity<SavingAccount>().Property(s => s.LaiSuatKyHan).HasPrecision(18, 3);
            modelBuilder.Entity<BaoCaoThang>().Property(b => b.TongTienGui).HasPrecision(18, 2);
            modelBuilder.Entity<BaoCaoThang>().Property(b => b.TongTienRut).HasPrecision(18, 2);
            modelBuilder.Entity<SavingAccount>().Property(s => s.LaiSuatApDung).HasPrecision(18, 3);
            modelBuilder.Entity<GiaoDich>().Property(g => g.SoTien).HasPrecision(18, 2);



            //  Seed data for HinhThucDenHan
            modelBuilder.Entity<HinhThucDenHan>().HasData(
                new HinhThucDenHan() { MaHinhThucDenHan = 1, TenHinhThucDenHan = "Rút hết" },
                new HinhThucDenHan() { MaHinhThucDenHan = 2, TenHinhThucDenHan = "Quay vòng gốc" },
                new HinhThucDenHan() { MaHinhThucDenHan = 3, TenHinhThucDenHan = "Quay vòng cả gốc và lãi" }
            );
            //Seed data for LoaiGiaoDich
            modelBuilder.Entity<TransactionType>().HasData(new TransactionType()
            {
                MaLoaiGiaoDich = 1,
                TenLoaiGiaoDich = "Rút tiền"
            }, new TransactionType()
            {
                MaLoaiGiaoDich = 2,
                TenLoaiGiaoDich = "Gửi tiền"
            });

            modelBuilder.Entity<LoaiSoTietKiem>().HasData(
                    new LoaiSoTietKiem()
                    {
                        MaLoaiSo = 1,
                        TenLoaiSo = "Không kỳ hạn",
                        LaiSuat = 0.002, //0.2%/năm
                        KyHan = 0,
                        ThoiGianGuiToiThieu = 0,
                        SoTienGuiToiThieu = 1000000 //1 triệu
                    },
                    // 1 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 2,
                        TenLoaiSo = "1 Tháng",
                        LaiSuat = 0.035,         // 3.5%/năm
                        KyHan = 1,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },
                    // 3 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 3,
                        TenLoaiSo = "3 Tháng",
                        LaiSuat = 0.045,         // 4.5%/năm
                        KyHan = 3,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },

                    // 6 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 4,
                        TenLoaiSo = "6 Tháng",
                        LaiSuat = 0.055,         // 5.5%/năm
                        KyHan = 6,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },
                    // 9 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 5,
                        TenLoaiSo = "9 Tháng",
                        LaiSuat = 0.057,         // 5.7%/năm
                        KyHan = 9,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },

                    // 12 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 6,
                        TenLoaiSo = "12 Tháng",
                        LaiSuat = 0.065,         // 6.5%/năm
                        KyHan = 12,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },

                    // 18 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 7,
                        TenLoaiSo = "18 Tháng",
                        LaiSuat = 0.068,         // 6.8%/năm
                        KyHan = 18,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },

                    // 24 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 8,
                        TenLoaiSo = "24 Tháng",
                        LaiSuat = 0.070,         // 7.0%/năm
                        KyHan = 24,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    },

                    // 36 tháng
                    new LoaiSoTietKiem
                    {
                        MaLoaiSo = 9,
                        TenLoaiSo = "36 Tháng",
                        LaiSuat = 0.072,         // 7.2%/năm
                        KyHan = 36,
                        ThoiGianGuiToiThieu = 1,
                        SoTienGuiToiThieu = 1000000
                    }
             );
        }
    }
}

