﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XFLCSMS.Data;

#nullable disable

namespace XFLCSMS.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240122044523_UpdateClosedby")]
    partial class UpdateClosedby
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.24")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("XFLCSMS.Models.Affected.AffectedSection", b =>
                {
                    b.Property<int>("AffectedSectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AffectedSectionId"), 1L, 1);

                    b.Property<string>("ASection")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AffectedSectionId");

                    b.ToTable("AffectedSectionss");
                });

            modelBuilder.Entity("XFLCSMS.Models.Branch.Branchh", b =>
                {
                    b.Property<int>("BranchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BranchId"), 1L, 1);

                    b.Property<string>("BranchName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("BrokerageId")
                        .HasColumnType("int");

                    b.HasKey("BranchId");

                    b.HasIndex("BrokerageId");

                    b.ToTable("Branchhs");
                });

            modelBuilder.Entity("XFLCSMS.Models.Brocarage.Brokerage", b =>
                {
                    b.Property<int>("BrokerageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrokerageId"), 1L, 1);

                    b.Property<string>("BrokerageHouseAcronym")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrokerageHouseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrokerageId");

                    b.ToTable("Brokerages");
                });

            modelBuilder.Entity("XFLCSMS.Models.DataTable.Customer", b =>
                {
                    b.Property<int>("Cust_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Cust_Id"), 1L, 1);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Cust_Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("XFLCSMS.Models.Issue.Attachment", b =>
                {
                    b.Property<int>("AttachmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttachmentId"), 1L, 1);

                    b.Property<string>("AttachmentLoc")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IssueId")
                        .HasColumnType("int");

                    b.HasKey("AttachmentId");

                    b.HasIndex("IssueId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("XFLCSMS.Models.Issue.IssueTable", b =>
                {
                    b.Property<int>("IssueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IssueId"), 1L, 1);

                    b.Property<int?>("AffectedSectionId")
                        .HasColumnType("int");

                    b.Property<string>("ApproveBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ApproveOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("AssignBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("AssignOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("BranchhBranchId")
                        .HasColumnType("int");

                    b.Property<int>("BrokerageId")
                        .HasColumnType("int");

                    b.Property<int?>("ClosedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ClosedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ITitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Priority")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SupportCatagoryId")
                        .HasColumnType("int");

                    b.Property<int?>("SupportSubCatagoryId")
                        .HasColumnType("int");

                    b.Property<int?>("SupportTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("IssueId");

                    b.HasIndex("AffectedSectionId");

                    b.HasIndex("BranchhBranchId");

                    b.HasIndex("BrokerageId");

                    b.HasIndex("SupportCatagoryId");

                    b.HasIndex("SupportSubCatagoryId");

                    b.HasIndex("SupportTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("XFLCSMS.Models.Register.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Branch")
                        .HasColumnType("int");

                    b.Property<int>("BrokerageHouseAcronym")
                        .HasColumnType("int");

                    b.Property<int>("BrokerageHouseName")
                        .HasColumnType("int");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Designation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PhonNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Terms")
                        .HasColumnType("bit");

                    b.Property<bool>("UCatagory")
                        .HasColumnType("bit");

                    b.Property<bool>("UStatus")
                        .HasColumnType("bit");

                    b.Property<bool>("UType")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("XFLCSMS.Models.Support.SupportCatagory", b =>
                {
                    b.Property<int>("SupportCatagoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SupportCatagoryId"), 1L, 1);

                    b.Property<string>("SCatagory")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SupportCatagoryId");

                    b.ToTable("SupportCatagories");
                });

            modelBuilder.Entity("XFLCSMS.Models.Support.SupportSubCatagory", b =>
                {
                    b.Property<int>("SupportSubCatagoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SupportSubCatagoryId"), 1L, 1);

                    b.Property<string>("SubCatagory")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SupportSubCatagoryId");

                    b.ToTable("SupportSubCatagories");
                });

            modelBuilder.Entity("XFLCSMS.Models.Support.SupportType", b =>
                {
                    b.Property<int>("SupportTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SupportTypeId"), 1L, 1);

                    b.Property<string>("SType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SupportTypeId");

                    b.ToTable("SupportTypes");
                });

            modelBuilder.Entity("XFLCSMS.Models.Branch.Branchh", b =>
                {
                    b.HasOne("XFLCSMS.Models.Brocarage.Brokerage", "Brokerage")
                        .WithMany("branches")
                        .HasForeignKey("BrokerageId");

                    b.Navigation("Brokerage");
                });

            modelBuilder.Entity("XFLCSMS.Models.Issue.Attachment", b =>
                {
                    b.HasOne("XFLCSMS.Models.Issue.IssueTable", "issue")
                        .WithMany("attachment")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("issue");
                });

            modelBuilder.Entity("XFLCSMS.Models.Issue.IssueTable", b =>
                {
                    b.HasOne("XFLCSMS.Models.Affected.AffectedSection", "affecteds")
                        .WithMany("issue")
                        .HasForeignKey("AffectedSectionId");

                    b.HasOne("XFLCSMS.Models.Branch.Branchh", null)
                        .WithMany("Issues")
                        .HasForeignKey("BranchhBranchId");

                    b.HasOne("XFLCSMS.Models.Brocarage.Brokerage", "Brokerages")
                        .WithMany()
                        .HasForeignKey("BrokerageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XFLCSMS.Models.Support.SupportCatagory", "supportCatagorys")
                        .WithMany("issue")
                        .HasForeignKey("SupportCatagoryId");

                    b.HasOne("XFLCSMS.Models.Support.SupportSubCatagory", "SupportSubCatagorys")
                        .WithMany("issue")
                        .HasForeignKey("SupportSubCatagoryId");

                    b.HasOne("XFLCSMS.Models.Support.SupportType", "supportTypes")
                        .WithMany("issue")
                        .HasForeignKey("SupportTypeId");

                    b.HasOne("XFLCSMS.Models.Register.User", "Users")
                        .WithMany("issue")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brokerages");

                    b.Navigation("SupportSubCatagorys");

                    b.Navigation("Users");

                    b.Navigation("affecteds");

                    b.Navigation("supportCatagorys");

                    b.Navigation("supportTypes");
                });

            modelBuilder.Entity("XFLCSMS.Models.Affected.AffectedSection", b =>
                {
                    b.Navigation("issue");
                });

            modelBuilder.Entity("XFLCSMS.Models.Branch.Branchh", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("XFLCSMS.Models.Brocarage.Brokerage", b =>
                {
                    b.Navigation("branches");
                });

            modelBuilder.Entity("XFLCSMS.Models.Issue.IssueTable", b =>
                {
                    b.Navigation("attachment");
                });

            modelBuilder.Entity("XFLCSMS.Models.Register.User", b =>
                {
                    b.Navigation("issue");
                });

            modelBuilder.Entity("XFLCSMS.Models.Support.SupportCatagory", b =>
                {
                    b.Navigation("issue");
                });

            modelBuilder.Entity("XFLCSMS.Models.Support.SupportSubCatagory", b =>
                {
                    b.Navigation("issue");
                });

            modelBuilder.Entity("XFLCSMS.Models.Support.SupportType", b =>
                {
                    b.Navigation("issue");
                });
#pragma warning restore 612, 618
        }
    }
}
