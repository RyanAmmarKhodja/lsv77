using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Hubs;
using campus_insider.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly EmailService _emailService;

        public NotificationService(AppDbContext context, IHubContext<NotificationHub> hubContext, EmailService emailService)
        {
            _context = context;
            _hubContext = hubContext;
            _emailService = emailService;
        }

        #region --- Queries ---

        public async Task<PagedResult<NotificationDto>> GetUserNotifications(
            long userId,
            bool? isRead = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (isRead.HasValue)
                query = query.Where(n => n.IsRead == isRead.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<NotificationDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<int> GetUnreadCount(long userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        #endregion

        #region --- Broadcast Notifications ---

        public async Task<ServiceResult> BroadcastNotificationToAllUsers(
            string type,
            string title,
            string message,
            bool sendEmail = false,
            string? actionUrl = null,
            string? actionText = null)
        {
            // Get all active users
            var userIds = await _context.Users
                .Select(u => u.Id)
                .ToListAsync();

            // Create notifications for all users
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                ActionText = actionText,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // Send real-time push to all connected users
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                Type = type,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                ActionText = actionText,
                CreatedAt = DateTime.UtcNow
            });

            // Send emails if requested
            if (sendEmail)
            {
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                foreach (var user in users)
                {
                    await _emailService.SendNotificationEmailAsync(
                        user.Email,
                        user.FirstName,
                        title,
                        message,
                        actionUrl
                    );
                }
            }

            return ServiceResult.Ok();
        }

        #endregion

        #region --- Commands ---

        public async Task<ServiceResult> CreateNotification(
            long userId,
            string type,
            string title,
            string message,
            bool sendEmail = false,
            string? entityType = null,
            long? entityId = null,
            string? actionUrl = null,
            string? actionText = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                EntityType = entityType,
                EntityId = entityId,
                ActionUrl = actionUrl,
                ActionText = actionText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // TODO: Trigger real-time push here (SignalR)
            // await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", MapToDto(notification));
            if (sendEmail)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    await _emailService.SendNotificationEmailAsync(
                        user.Email,
                        user.FirstName,
                        title,
                        message,
                        actionUrl
                    );
                }
            }

            await _hubContext.Clients
            .Group($"user-{userId}")
            .SendAsync("ReceiveNotification", MapToDto(notification));

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> MarkAsRead(long notificationId, long userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return ServiceResult.Fail("Notification not found.");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> MarkAllAsRead(long userId)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, DateTime.UtcNow));

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteNotification(long notificationId, long userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return ServiceResult.Fail("Notification not found.");

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        #endregion

        #region --- Helper Methods ---

        // Loan notifications
        public async Task NotifyLoanApproved(long borrowerId, long loanId, string equipmentName)
        {
            await CreateNotification(
                borrowerId,
                "LOAN_APPROVED",
                "Loan Approved",
                $"Your loan request for '{equipmentName}' has been approved.",
                sendEmail: true, // Send email for important events
                "Loan",
                loanId,
                $"/api/loans/{loanId}",
                "View Loan"
            );
        }

        public async Task NotifyNewLoanRequest(long ownerId, long loanId, string equipmentName, string borrowerName)
        {
            await CreateNotification(
                ownerId,
                "LOAN_REQUEST",
                "New Loan Request",
                $"{borrowerName} wants to borrow your '{equipmentName}'.",
                sendEmail: true, // Notify owner via email
                "Loan",
                loanId,
                $"/api/loans/{loanId}/approve",
                "Review Request"
            );
        }

        public async Task NotifySystemAnnouncement(string title, string message)
        {
            await BroadcastNotificationToAllUsers(
                "SYSTEM_ANNOUNCEMENT",
                title,
                message,
                sendEmail: true // Send email to all users
            );
        }
        #endregion

        #region --- Mapping ---

        private static NotificationDto MapToDto(Notification notification) => new()
        {
            Id = notification.Id,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            ActionUrl = notification.ActionUrl,
            ActionText = notification.ActionText
        };

        #endregion
    }
}