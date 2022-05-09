using Emailit.Data.Configurations;
using Emailit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _Configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _Configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BranchOffice> BranchOffices { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserSession> UsersSessions { get; set; }
        public DbSet<UserModification> UsersModifications { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<UserRole> UsersRoles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FileData> FilesData { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        public DbSet<ReceivedMessage> ReceivedMessages { get; set; }
        public DbSet<ReceivedMessageState> ReceivedMessagesStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BranchOfficeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(_Configuration));
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ReceivedMessageConfiguration());
            modelBuilder.ApplyConfiguration(new ReceivedMessageStateConfiguration());
            modelBuilder.ApplyConfiguration(new AttachedFileConfiguration());
        }

    }
}
