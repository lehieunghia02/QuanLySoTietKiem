using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLySoTietKiem.Migrations
{
    /// <inheritdoc />
    public partial class SeederDataLoaiSoTietKiemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PaymentHistories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "PaymentHistories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "LoaiSoTietKiems",
                columns: new[] { "MaLoaiSo", "KyHan", "LaiSuat", "SoTienGuiToiThieu", "TenLoaiSo", "ThoiGianGuiToiThieu" },
                values: new object[,]
                {
                    { 1, 0, 0.002, 1000000m, "Không kỳ hạn", 0 },
                    { 2, 1, 0.035000000000000003, 1000000m, "1 Tháng", 1 },
                    { 3, 3, 0.044999999999999998, 1000000m, "3 Tháng", 3 },
                    { 4, 6, 0.055, 1000000m, "6 Tháng", 6 },
                    { 5, 9, 0.057000000000000002, 1000000m, "9 Tháng", 9 },
                    { 6, 12, 0.065000000000000002, 1000000m, "12 Tháng", 12 },
                    { 7, 18, 0.068000000000000005, 1000000m, "18 Tháng", 18 },
                    { 8, 24, 0.070000000000000007, 1000000m, "24 Tháng", 24 },
                    { 9, 36, 0.071999999999999995, 1000000m, "36 Tháng", 36 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "LoaiSoTietKiems",
                keyColumn: "MaLoaiSo",
                keyValue: 9);

           

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
