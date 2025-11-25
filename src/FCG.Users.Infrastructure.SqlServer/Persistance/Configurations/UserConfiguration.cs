using FCG.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.SqlServer.Persistance.Configurations
{
    [ExcludeFromCodeCoverage]
    public class UserConfiguration : BaseConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("Users");

            builder.OwnsOne(u => u.Name, nameBuilder =>
            {
                nameBuilder.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(255)
                    .IsRequired();
            });

            builder.OwnsOne(u => u.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired();

                emailBuilder.HasIndex(e => e.Value)
                    .IsUnique();
            });

            builder.OwnsOne(u => u.Password, passwordBuilder =>
            {
                passwordBuilder.Property(p => p.Value)
                    .HasColumnName("Password")
                    .HasMaxLength(255)
                    .IsRequired();
            });

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}