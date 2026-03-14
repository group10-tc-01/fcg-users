using FCG.Users.Application.Abstractions.Audit;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Infrastructure.SqlServer.Persistance;
using FCG.Users.Infrastructure.SqlServer.Persistance.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.UnitTests.Infrastructure.Persistance.Interceptors
{
    public class AuditingInterceptorTest
    {
        [Fact]
        public async Task SavingChangesAsync_WithAddedEntity_ShouldCreateAuditTrailWithCreate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserBuilder().Build();

            var sessionProviderMock = new Mock<ICurrentSessionProvider>();
            sessionProviderMock.Setup(x => x.GetUserId()).Returns(userId);

            var loggerMock = new Mock<ILogger<AuditingInterceptor>>();
            var publisherMock = new Mock<IPublisher>();

            var interceptor = new AuditingInterceptor(sessionProviderMock.Object, loggerMock.Object);

            var options = new DbContextOptionsBuilder<FcgUserDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new FcgUserDbContext(options, publisherMock.Object, interceptor);

            // Act
            context.User.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var auditTrails = await context.AuditTrail.ToListAsync();
            Assert.Single(auditTrails);
            Assert.Equal(AuditTrailType.Create, auditTrails[0].TrailType);
            Assert.Equal(user.Id.ToString(), auditTrails[0].PrimaryKey);
            Assert.NotEmpty(auditTrails[0].NewValues);
        }

        [Fact]
        public async Task SavingChangesAsync_WithModifiedEntity_ShouldCreateAuditTrailWithUpdate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserBuilder().Build();

            var sessionProviderMock = new Mock<ICurrentSessionProvider>();
            sessionProviderMock.Setup(x => x.GetUserId()).Returns(userId);

            var loggerMock = new Mock<ILogger<AuditingInterceptor>>();
            var publisherMock = new Mock<IPublisher>();

            var interceptor = new AuditingInterceptor(sessionProviderMock.Object, loggerMock.Object);

            var options = new DbContextOptionsBuilder<FcgUserDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new FcgUserDbContext(options, publisherMock.Object, interceptor);

            // Act - Add user first
            context.User.Add(user);
            await context.SaveChangesAsync();

            // Clear previous audit trails
            context.AuditTrail.RemoveRange(context.AuditTrail);
            await context.SaveChangesAsync();

            // Modify user
            user.Deactivate();
            await context.SaveChangesAsync();

            // Assert
            var auditTrails = await context.AuditTrail.ToListAsync();
            Assert.Single(auditTrails);
            Assert.Equal(AuditTrailType.Update, auditTrails[0].TrailType);
            Assert.Equal(user.Id.ToString(), auditTrails[0].PrimaryKey);
            Assert.NotEmpty(auditTrails[0].NewValues);
            Assert.NotEmpty(auditTrails[0].OldValues);
            Assert.NotEmpty(auditTrails[0].ChangedColumns);
        }

        [Fact]
        public async Task SavingChangesAsync_WithDeletedEntity_ShouldCreateAuditTrailWithDelete()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserBuilder().Build();

            var sessionProviderMock = new Mock<ICurrentSessionProvider>();
            sessionProviderMock.Setup(x => x.GetUserId()).Returns(userId);

            var loggerMock = new Mock<ILogger<AuditingInterceptor>>();
            var publisherMock = new Mock<IPublisher>();

            var interceptor = new AuditingInterceptor(sessionProviderMock.Object, loggerMock.Object);

            var options = new DbContextOptionsBuilder<FcgUserDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new FcgUserDbContext(options, publisherMock.Object, interceptor);

            // Act - Add user first
            context.User.Add(user);
            await context.SaveChangesAsync();

            // Clear previous audit trails
            context.AuditTrail.RemoveRange(context.AuditTrail);
            await context.SaveChangesAsync();

            // Delete user
            context.User.Remove(user);
            await context.SaveChangesAsync();

            // Assert
            var auditTrails = await context.AuditTrail.ToListAsync();
            Assert.Single(auditTrails);
            Assert.Equal(AuditTrailType.Delete, auditTrails[0].TrailType);
            Assert.Equal(user.Id.ToString(), auditTrails[0].PrimaryKey);
            Assert.NotEmpty(auditTrails[0].OldValues);
        }
    }
}