using FCG.Users.Domain.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.SqlServer.Persistance.Configurations
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokenConfiguration : BaseConfiguration<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            base.Configure(builder);

            builder.Property(rt => rt.Token)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(rt => rt.UserId)
                   .IsRequired();

            builder.Property(rt => rt.ExpiresAt)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(rt => rt.RevokedReason)
                   .HasMaxLength(500);

            builder.HasOne(rt => rt.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(rt => rt.UserId);
        }
    }
}
