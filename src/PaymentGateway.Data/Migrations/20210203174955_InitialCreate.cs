using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    MerchantId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApiKey = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.MerchantId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(nullable: false),
                    MerchantTransactionId = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CardNumber = table.Column<string>(nullable: true),
                    ExpiryMonth = table.Column<int>(nullable: false),
                    ExpiryYear = table.Column<int>(nullable: false),
                    Cvv = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    MerchantId = table.Column<int>(nullable: false),
                    BankReferenceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "MerchantId", "ApiKey", "EmailAddress", "CreatedOn", },
                values: new object[] { 1, "testMerchant1Key3264", "danielcarles@gmail.com", new DateTime(2022, 6, 21, 17, 30, 55, 466, DateTimeKind.Utc).AddTicks(7834) });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "MerchantId", "ApiKey", "EmailAddress", "CreatedOn", },
                values: new object[] { 2, "testMerchant2Key007", "daniel.carles@gmail.com", new DateTime(2022, 6, 21, 17, 35, 55, 466, DateTimeKind.Utc).AddTicks(7834) });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "TransactionId","MerchantTransactionId", "Amount", "BankReferenceId", "CardNumber", "CreatedOn", "Currency", "Cvv", "ErrorMessage", "ExpiryMonth", "ExpiryYear", "MerchantId", "Status" },
                values: new object[] { new Guid("408a2ade-0931-4aa5-9a77-f5dd2eb6ceb7"), "bfb4844e-c2cf-4f22-abe8-d05633fd6e2a", 10.999m, "pay_f8c6166f-a50f-447b-b33d-920a6f7bbf37", "123451234456123456", new DateTime(2022, 6, 21, 17, 49, 55, 466, DateTimeKind.Utc).AddTicks(7834), "EUR", "123", null, 12, 2020, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_ApiKey",
                table: "Merchants",
                column: "ApiKey",
                unique: true,
                filter: "[ApiKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MerchantId",
                table: "Transactions",
                column: "MerchantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Merchants");
        }
    }
}
