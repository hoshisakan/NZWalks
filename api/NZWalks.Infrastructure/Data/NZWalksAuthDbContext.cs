using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NZWalks.Domain.Entities;

namespace NZWalks.Infrastructure.Data
{
    public class NZWalksAuthDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;

        public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .IsRequired();

            var readerRoleId = "62f40933-2814-4672-9481-42259799cc90";
            var writerRoleId = "4321cc91-2cd7-4907-ad04-1649a78d54fd";
            var adminRoleId = "89009c2e-ca38-4ea2-86d2-1db2d5f30449";

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = readerRoleId, ConcurrencyStamp = readerRoleId, Name = "Reader", NormalizedName = "READER" },
                new IdentityRole { Id = writerRoleId, ConcurrencyStamp = writerRoleId, Name = "Writer", NormalizedName = "WRITER" },
                new IdentityRole { Id = adminRoleId, ConcurrencyStamp = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            var adminUser = new IdentityUser
            {
                Id = "4c791d74-5907-40ce-b188-034e6d60fed9",
                UserName = _configuration["Admin:UserName"],
                Email = _configuration["Admin:Email"],
                NormalizedEmail = _configuration["Admin:Email"]?.ToUpper(),
                NormalizedUserName = _configuration["Admin:UserName"]?.ToUpper(),
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null!, _configuration["Admin:Password"] ?? "")
            };
            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUser.Id,
                RoleId = adminRoleId
            });
        }
    }
}
