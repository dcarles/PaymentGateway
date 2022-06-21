﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Data.Migrations
{
    [DbContext(typeof(GatewayDbContext))]
    [Migration("20210203174955_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PaymentGateway.Data.Entities.Merchant", b =>
                {
                    b.Property<int>("MerchantId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApiKey");

                    b.Property<string>("EmailAddress");

                    b.Property<DateTime>("CreatedOn");

                    b.HasKey("MerchantId");

                    b.HasIndex("ApiKey")
                        .IsUnique()
                        .HasFilter("[ApiKey] IS NOT NULL");

                    b.ToTable("Merchants");

                    b.HasData(
                        new
                        {
                            MerchantId = 1,
                            ApiKey = "testMerchant1Key3264",
                            EmailAddress = "danielcarles@gmail.com",
                            CreatedOn = new DateTime(2022, 6, 21, 17, 30, 55, 466, DateTimeKind.Utc).AddTicks(7834)
                        },
                        new
                        {
                            MerchantId = 2,
                            ApiKey = "testMerchant2Key007",
                            EmailAddress = "daniel.carles@gmail.com",
                            CreatedOn = new DateTime(2022, 6, 21, 17, 35, 55, 466, DateTimeKind.Utc).AddTicks(7834)
                        });
                });

            modelBuilder.Entity("PaymentGateway.Data.Entities.Transaction", b =>
                {
                    b.Property<Guid>("TransactionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MerchantTransactionId")
                    .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount")
                        .HasColumnType("Money");

                    b.Property<string>("BankReferenceId");

                    b.Property<string>("CardNumber");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Currency");

                    b.Property<string>("Cvv");

                    b.Property<string>("ErrorMessage");

                    b.Property<int>("ExpiryMonth");

                    b.Property<int>("ExpiryYear");

                    b.Property<int>("MerchantId");

                    b.Property<int>("Status");

                    b.HasKey("TransactionId");

                    b.HasIndex("MerchantId");

                    b.ToTable("Transactions");

                    b.HasData(
                        new
                        {
                            TransactionId = new Guid("408a2ade-0931-4aa5-9a77-f5dd2eb6ceb7"),
                            MerchantTransactionId = "bfb4844e-c2cf-4f22-abe8-d05633fd6e2a",
                            Amount = 10.999m,
                            BankReferenceId = "pay_f8c6166f-a50f-447b-b33d-920a6f7bbf37",
                            CardNumber = "123451234456123456",
                            CreatedOn = new DateTime(2022, 6, 21, 17, 49, 55, 466, DateTimeKind.Utc).AddTicks(7834),
                            Currency = "EUR",
                            Cvv = "123",
                            ExpiryMonth = 12,
                            ExpiryYear = 2020,
                            MerchantId = 1,
                            Status = 1
                        });
                });

            modelBuilder.Entity("PaymentGateway.Data.Entities.Transaction", b =>
                {
                    b.HasOne("PaymentGateway.Data.Entities.Merchant", "Merchant")
                        .WithMany()
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
