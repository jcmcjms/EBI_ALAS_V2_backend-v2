using Alas.Api.Features.Notifications;
using Alas.Api.Features.Notifications.Domain;
using Alas.Api.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Tests;

public class NotificationServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly NotificationService _service;
    private readonly DateTimeOffset _fixedTime = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        var timeProvider = new FakeTimeProvider(_fixedTime);
        _service = new NotificationService(_context, timeProvider);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task CreateAsync_ShouldPersistNotification()
    {
        await _service.CreateAsync(1, "Test Title", "Test Description", "/test");

        var notification = await _context.Notifications.SingleAsync();
        notification.UserId.Should().Be(1);
        notification.Title.Should().Be("Test Title");
        notification.Description.Should().Be("Test Description");
        notification.Link.Should().Be("/test");
        notification.IsRead.Should().BeFalse();
        notification.CreatedAt.Should().Be(_fixedTime);
        notification.Type.Should().Be(NotificationTypes.System);
    }

    [Fact]
    public async Task CreateAsync_ShouldUseExplicitActionType()
    {
        await _service.CreateAsync(1, "Ready for Approval", "desc", null, NotificationTypes.Action);
        var notification = await _context.Notifications.SingleAsync();
        notification.Type.Should().Be(NotificationTypes.Action);
    }

    [Fact]
    public async Task CreateAsync_ShouldUseExplicitApplicationType()
    {
        await _service.CreateAsync(1, "Application Submitted", "desc", null, NotificationTypes.Application);
        var notification = await _context.Notifications.SingleAsync();
        notification.Type.Should().Be(NotificationTypes.Application);
    }

    [Fact]
    public async Task CreateAsync_ShouldUseExplicitMessageType()
    {
        await _service.CreateAsync(1, "Status Update: ForApproval", "desc", null, NotificationTypes.Message);
        var notification = await _context.Notifications.SingleAsync();
        notification.Type.Should().Be(NotificationTypes.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldDefaultToSystemTypeWhenNotProvided()
    {
        await _service.CreateAsync(1, "Random Title", "desc");
        var notification = await _context.Notifications.SingleAsync();
        notification.Type.Should().Be(NotificationTypes.System);
    }

    [Fact]
    public async Task CreateBatchAsync_ShouldPersistMultipleNotifications()
    {
        var drafts = new List<NotificationDraft>
        {
            new(1, "Title 1", "Desc 1", null),
            new(2, "Title 2", "Desc 2", "/link", NotificationTypes.Action)
        };

        await _service.CreateBatchAsync(drafts);

        var count = await _context.Notifications.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task CreateBatchAsync_EmptyList_ShouldNotThrow()
    {
        await _service.CreateBatchAsync([]);

        var count = await _context.Notifications.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnOrderedByCreatedAt()
    {
        _context.Notifications.AddRange(
            new Notification { UserId = 1, Title = "Old", Description = "d", CreatedAt = _fixedTime.AddDays(-1) },
            new Notification { UserId = 1, Title = "New", Description = "d", CreatedAt = _fixedTime },
            new Notification { UserId = 2, Title = "Other", Description = "d", CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var result = await _service.GetUserNotificationsAsync(1);

        result.Should().HaveCount(2);
        result[0].Title.Should().Be("New");
        result[1].Title.Should().Be("Old");
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldRespectLimit()
    {
        for (var i = 0; i < 30; i++)
            _context.Notifications.Add(new Notification { UserId = 1, Title = $"N{i}", Description = "d", CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var result = await _service.GetUserNotificationsAsync(1, limit: 5);

        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetInboxAsync_ShouldFilterByUnreadStatus()
    {
        _context.Notifications.AddRange(
            new Notification { UserId = 1, Title = "Read", Description = "d", IsRead = true, CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "Unread", Description = "d", IsRead = false, CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var unreadQuery = new InboxQuery(1, 10, "unread", null, null);
        var result = await _service.GetInboxAsync(1, unreadQuery);

        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Unread");
        result.UnreadCount.Should().Be(1);
    }

    [Fact]
    public async Task GetInboxAsync_ShouldFilterByType()
    {
        _context.Notifications.AddRange(
            new Notification { UserId = 1, Title = "A", Description = "d", Type = NotificationTypes.Action, CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "B", Description = "d", Type = NotificationTypes.System, CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var query = new InboxQuery(1, 10, "all", NotificationTypes.Action, null);
        var result = await _service.GetInboxAsync(1, query);

        result.Items.Should().HaveCount(1);
        result.Items[0].Type.Should().Be(NotificationTypes.Action);
    }

    [Fact]
    public async Task GetInboxAsync_ShouldSearchByTitleAndDescription()
    {
        _context.Notifications.AddRange(
            new Notification { UserId = 1, Title = "Loan Approved", Description = "desc", CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "System Alert", Description = "Your loan was returned", CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "Other", Description = "nothing", CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var query = new InboxQuery(1, 10, "all", null, "loan");
        var result = await _service.GetInboxAsync(1, query);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetInboxAsync_ShouldReturnTotalAndUnreadCounts()
    {
        _context.Notifications.AddRange(
            new Notification { UserId = 1, Title = "A", Description = "d", IsRead = true, CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "B", Description = "d", IsRead = false, CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "C", Description = "d", IsRead = false, CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var query = new InboxQuery(1, 10, "all", null, null);
        var result = await _service.GetInboxAsync(1, query);

        result.TotalCount.Should().Be(3);
        result.UnreadCount.Should().Be(2);
    }

    [Fact]
    public async Task MarkReadAsync_WhenNotificationExists_ShouldMarkRead()
    {
        var notification = new Notification { UserId = 1, Title = "T", Description = "d", CreatedAt = _fixedTime };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var result = await _service.MarkReadAsync(1, notification.Id);

        result.Should().BeTrue();
        var updated = await _context.Notifications.FindAsync(notification.Id);
        updated!.IsRead.Should().BeTrue();
        updated.ReadAt.Should().Be(_fixedTime);
    }

    [Fact]
    public async Task MarkReadAsync_WhenNotOwned_ShouldReturnFalse()
    {
        var notification = new Notification { UserId = 1, Title = "T", Description = "d", CreatedAt = _fixedTime };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var result = await _service.MarkReadAsync(999, notification.Id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkReadAsync_WhenAlreadyRead_ShouldNotUpdateReadAt()
    {
        var notification = new Notification
        {
            UserId = 1, Title = "T", Description = "d",
            IsRead = true, ReadAt = _fixedTime.AddDays(-1), CreatedAt = _fixedTime
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var result = await _service.MarkReadAsync(1, notification.Id);

        result.Should().BeTrue();
        var updated = await _context.Notifications.FindAsync(notification.Id);
        updated!.ReadAt.Should().Be(_fixedTime.AddDays(-1));
    }

    [Fact]
    public async Task MarkAllReadAsync_ShouldMarkAllUnreadAsRead()
    {
        _context.Notifications.AddRange(
            new Notification { UserId = 1, Title = "A", Description = "d", IsRead = false, CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "B", Description = "d", IsRead = false, CreatedAt = _fixedTime },
            new Notification { UserId = 1, Title = "C", Description = "d", IsRead = true, CreatedAt = _fixedTime },
            new Notification { UserId = 2, Title = "D", Description = "d", IsRead = false, CreatedAt = _fixedTime });
        await _context.SaveChangesAsync();

        var changed = await _service.MarkAllReadAsync(1);

        changed.Should().Be(2);
        var user1Unread = await _context.Notifications
            .Where(n => n.UserId == 1 && !n.IsRead)
            .CountAsync();
        user1Unread.Should().Be(0);
        var user2Unread = await _context.Notifications
            .Where(n => n.UserId == 2 && !n.IsRead)
            .CountAsync();
        user2Unread.Should().Be(1);
    }

    [Fact]
    public void AddNotificationForDeferredSave_ShouldAddToContextWithoutSaving()
    {
        _service.AddNotificationForDeferredSave(1, "Deferred", "desc", "/link", NotificationTypes.Action);

        _context.Notifications.Local.Should().HaveCount(1);
        var notification = _context.Notifications.Local.Single();
        notification.Title.Should().Be("Deferred");
        notification.IsRead.Should().BeFalse();
        notification.Type.Should().Be(NotificationTypes.Action);
    }

    private sealed class FakeTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;
        public FakeTimeProvider(DateTimeOffset now) => _now = now;
        public override DateTimeOffset GetUtcNow() => _now;
    }
}