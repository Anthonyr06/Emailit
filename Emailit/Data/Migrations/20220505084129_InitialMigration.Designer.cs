﻿// <auto-generated />
using System;
using Emailit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Emailit.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220505084129_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.24")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Emailit.Models.AttachedFile", b =>
                {
                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.HasKey("MessageId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("AttachedFiles");
                });

            modelBuilder.Entity("Emailit.Models.BranchOffice", b =>
                {
                    b.Property<int>("BranchOfficeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(80)")
                        .HasMaxLength(80);

                    b.HasKey("BranchOfficeId");

                    b.HasIndex("ManagerId")
                        .IsUnique()
                        .HasFilter("[ManagerId] IS NOT NULL");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("BranchOffices");
                });

            modelBuilder.Entity("Emailit.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("BranchOfficeId")
                        .HasColumnType("int");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(80)")
                        .HasMaxLength(80);

                    b.HasKey("DepartmentId");

                    b.HasIndex("BranchOfficeId");

                    b.HasIndex("ManagerId")
                        .IsUnique()
                        .HasFilter("[ManagerId] IS NOT NULL");

                    b.HasIndex("Name", "BranchOfficeId")
                        .IsUnique()
                        .HasFilter("[BranchOfficeId] IS NOT NULL");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Emailit.Models.FileData", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("LengthInBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("OriginalName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FileId");

                    b.ToTable("FilesData");
                });

            modelBuilder.Entity("Emailit.Models.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(40)")
                        .HasMaxLength(40);

                    b.HasKey("JobId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Emailit.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BodyInHtml")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Confidential")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Date")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.Property<string>("Tittle")
                        .IsRequired()
                        .HasColumnType("nvarchar(254)")
                        .HasMaxLength(254);

                    b.HasKey("MessageId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Emailit.Models.ReceivedMessage", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<bool>("CC")
                        .HasColumnType("bit");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "MessageId");

                    b.HasIndex("MessageId");

                    b.ToTable("ReceivedMessages");
                });

            modelBuilder.Entity("Emailit.Models.ReceivedMessageState", b =>
                {
                    b.Property<int>("ReceivedMessageStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ReceivedMessageStateId");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId", "MessageId");

                    b.ToTable("ReceivedMessagesStates");
                });

            modelBuilder.Entity("Emailit.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(80)")
                        .HasMaxLength(80);

                    b.Property<decimal>("Permissions")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("RoleId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            Active = true,
                            Created = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "user with administrative rights",
                            Name = "administrator",
                            Permissions = 1145m
                        });
                });

            modelBuilder.Entity("Emailit.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(254)")
                        .HasMaxLength(254);

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasColumnType("nvarchar(11)")
                        .HasMaxLength(11);

                    b.Property<int?>("JobId")
                        .HasColumnType("int");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("nvarchar(80)")
                        .HasMaxLength(80);

                    b.Property<int>("LoginAttempts")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<bool>("MustChangePassword")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(80)")
                        .HasMaxLength(80);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Permission")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("UserId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("IdCard")
                        .IsUnique();

                    b.HasIndex("JobId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Active = true,
                            Created = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mensajeriamensajeriamensajeria@gmail.com",
                            Gender = "other",
                            IdCard = "00000000000",
                            Lastname = "moore",
                            LoginAttempts = 0,
                            MustChangePassword = true,
                            Name = "patricia",
                            Password = "AQAAAAEAACcQAAAAEKxECOFtjFd1Qpg9B+ewlk/4SARoKqgaBC1tLUdhbshxMHdngrTG8H166uW+agNgng==",
                            Permission = 262143m
                        });
                });

            modelBuilder.Entity("Emailit.Models.UserModification", b =>
                {
                    b.Property<int>("UserModificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModificationType")
                        .IsRequired()
                        .HasColumnType("nvarchar(15)");

                    b.Property<int?>("ModifiedUserId")
                        .HasColumnType("int");

                    b.Property<int?>("ModifierId")
                        .HasColumnType("int");

                    b.HasKey("UserModificationId");

                    b.HasIndex("ModifiedUserId");

                    b.HasIndex("ModifierId");

                    b.ToTable("UsersModifications");
                });

            modelBuilder.Entity("Emailit.Models.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UsersRoles");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            RoleId = 1
                        });
                });

            modelBuilder.Entity("Emailit.Models.UserSession", b =>
                {
                    b.Property<int>("UserSessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("IpBytes")
                        .HasColumnName("IP")
                        .HasColumnType("varbinary(16)")
                        .HasMaxLength(16);

                    b.Property<DateTime>("LastActivity")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Successful")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserSessionId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersSessions");
                });

            modelBuilder.Entity("Emailit.Models.AttachedFile", b =>
                {
                    b.HasOne("Emailit.Models.FileData", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emailit.Models.Message", "Message")
                        .WithMany("AttachedFiles")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Emailit.Models.BranchOffice", b =>
                {
                    b.HasOne("Emailit.Models.User", "Manager")
                        .WithOne("ManagedBranchOffice")
                        .HasForeignKey("Emailit.Models.BranchOffice", "ManagerId");
                });

            modelBuilder.Entity("Emailit.Models.Department", b =>
                {
                    b.HasOne("Emailit.Models.BranchOffice", "BranchOffice")
                        .WithMany("Departments")
                        .HasForeignKey("BranchOfficeId");

                    b.HasOne("Emailit.Models.User", "Manager")
                        .WithOne("ManagedDepartment")
                        .HasForeignKey("Emailit.Models.Department", "ManagerId");
                });

            modelBuilder.Entity("Emailit.Models.Message", b =>
                {
                    b.HasOne("Emailit.Models.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId");
                });

            modelBuilder.Entity("Emailit.Models.ReceivedMessage", b =>
                {
                    b.HasOne("Emailit.Models.Message", "Message")
                        .WithMany("Recipients")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emailit.Models.User", "User")
                        .WithMany("ReceivedMessages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Emailit.Models.ReceivedMessageState", b =>
                {
                    b.HasOne("Emailit.Models.Message", "Message")
                        .WithMany("States")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emailit.Models.User", "User")
                        .WithMany("ReceivedMessagesState")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emailit.Models.ReceivedMessage", "ReceivedMessage")
                        .WithMany("States")
                        .HasForeignKey("UserId", "MessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Emailit.Models.User", b =>
                {
                    b.HasOne("Emailit.Models.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId");

                    b.HasOne("Emailit.Models.Job", "Job")
                        .WithMany("Users")
                        .HasForeignKey("JobId");
                });

            modelBuilder.Entity("Emailit.Models.UserModification", b =>
                {
                    b.HasOne("Emailit.Models.User", "ModifiedUser")
                        .WithMany("ModificationsReceived")
                        .HasForeignKey("ModifiedUserId");

                    b.HasOne("Emailit.Models.User", "Modifier")
                        .WithMany("ModificationsMadeBy")
                        .HasForeignKey("ModifierId");
                });

            modelBuilder.Entity("Emailit.Models.UserRole", b =>
                {
                    b.HasOne("Emailit.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emailit.Models.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Emailit.Models.UserSession", b =>
                {
                    b.HasOne("Emailit.Models.User", "User")
                        .WithMany("UserSessions")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}