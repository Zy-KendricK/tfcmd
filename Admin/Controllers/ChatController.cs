using Admin.Infrastructure;
using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Admin.Controllers;

/// <summary>
/// JSON endpoints for the messenger dock (Beehive buddy-chat) so platform
/// users can message each other. Uses the existing Message/Conversation entities.
/// </summary>
[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private const string DefaultAvatar = "/wp-content/uploads/2020/10/avatar.png";

    /// <summary>
    /// Buddy list: every other active user, with unread counts and pseudo presence.
    /// </summary>
    [HttpGet]
    [SkipAuditLog]
    public async Task<IActionResult> Buddies()
    {
        var me = CurrentUserId;
        var onlineThreshold = DateTime.UtcNow.AddMinutes(-15);

        var users = await _userManager.Users
            .Where(u => u.Id != me && u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.DisplayName ?? u.UserName)
            .Select(u => new
            {
                id = u.Id,
                name = u.DisplayName ?? u.UserName,
                avatar = u.AvatarUrl ?? DefaultAvatar,
                online = u.LastLoginAt != null && u.LastLoginAt > onlineThreshold
            })
            .ToListAsync();

        var unread = await _context.Messages
            .Where(m => m.RecipientId == me && !m.IsRead && !m.RecipientDeleted && !m.IsDeleted)
            .GroupBy(m => m.SenderId)
            .Select(g => new { senderId = g.Key, count = g.Count() })
            .ToListAsync();

        var unreadMap = unread.ToDictionary(x => x.senderId, x => x.count);

        return Json(users.Select(u => new
        {
            u.id,
            u.name,
            u.avatar,
            u.online,
            unread = unreadMap.TryGetValue(u.id, out var c) ? c : 0
        }));
    }

    /// <summary>
    /// Conversation history with one buddy; marks their messages to me as read.
    /// </summary>
    [HttpGet]
    [SkipAuditLog]
    public async Task<IActionResult> History(string userId, int take = 50)
    {
        var me = CurrentUserId;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        var messages = await _context.Messages
            .Where(m => !m.IsDeleted &&
                ((m.SenderId == me && m.RecipientId == userId && !m.SenderDeleted) ||
                 (m.SenderId == userId && m.RecipientId == me && !m.RecipientDeleted)))
            .OrderByDescending(m => m.CreatedAt)
            .Take(take)
            .Select(m => new
            {
                id = m.Id,
                senderId = m.SenderId,
                content = m.Content,
                sentAt = m.CreatedAt
            })
            .ToListAsync();

        await MarkReadAsync(userId, me);

        return Json(messages.OrderBy(m => m.sentAt));
    }

    public class SendRequest
    {
        public string RecipientId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sends a message, creating the 1:1 conversation when necessary.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send([FromBody] SendRequest request)
    {
        var me = CurrentUserId;
        var content = request.Content?.Trim();

        if (string.IsNullOrEmpty(request.RecipientId) || string.IsNullOrEmpty(content))
        {
            return BadRequest(new { error = "Message is empty." });
        }

        if (content.Length > 2000)
        {
            content = content[..2000];
        }

        var recipient = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.RecipientId && u.IsActive && !u.IsDeleted);
        if (recipient == null)
        {
            return NotFound(new { error = "Recipient not found." });
        }

        var conversation = await GetOrCreateConversationAsync(me, recipient.Id);

        var message = new Message
        {
            SenderId = me,
            RecipientId = recipient.Id,
            Content = content,
            ConversationId = conversation.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedById = me
        };
        _context.Messages.Add(message);

        conversation.LastMessageAt = message.CreatedAt;
        conversation.MessageCount += 1;

        var recipientParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == recipient.Id);
        if (recipientParticipant != null)
        {
            recipientParticipant.UnreadCount += 1;
        }

        await _context.SaveChangesAsync();

        return Json(new
        {
            id = message.Id,
            senderId = message.SenderId,
            content = message.Content,
            sentAt = message.CreatedAt
        });
    }

    /// <summary>
    /// Polling endpoint: unread totals per buddy and new messages since a given id.
    /// </summary>
    [HttpGet]
    [SkipAuditLog]
    public async Task<IActionResult> Updates(long sinceId = 0)
    {
        var me = CurrentUserId;

        var unread = await _context.Messages
            .Where(m => m.RecipientId == me && !m.IsRead && !m.RecipientDeleted && !m.IsDeleted)
            .GroupBy(m => m.SenderId)
            .Select(g => new { senderId = g.Key, count = g.Count() })
            .ToListAsync();

        var newMessages = await _context.Messages
            .Where(m => m.RecipientId == me && m.Id > sinceId && !m.RecipientDeleted && !m.IsDeleted)
            .OrderBy(m => m.Id)
            .Take(100)
            .Select(m => new
            {
                id = m.Id,
                senderId = m.SenderId,
                content = m.Content,
                sentAt = m.CreatedAt
            })
            .ToListAsync();

        var lastId = await _context.Messages
            .Where(m => m.RecipientId == me)
            .MaxAsync(m => (long?)m.Id) ?? 0;

        return Json(new { unread, messages = newMessages, lastId = Math.Max(lastId, sinceId) });
    }

    /// <summary>
    /// Marks a buddy's messages as read (called when their chat window is focused).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SkipAuditLog]
    public async Task<IActionResult> MarkRead([FromBody] MarkReadRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            return BadRequest();
        }

        await MarkReadAsync(request.UserId, CurrentUserId);
        return Json(new { ok = true });
    }

    public class MarkReadRequest
    {
        public string UserId { get; set; } = string.Empty;
    }

    private async Task MarkReadAsync(string senderId, string recipientId)
    {
        var unreadMessages = await _context.Messages
            .Where(m => m.SenderId == senderId && m.RecipientId == recipientId && !m.IsRead)
            .ToListAsync();

        if (unreadMessages.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
            message.ReadAt = now;
        }

        var conversationIds = unreadMessages
            .Where(m => m.ConversationId.HasValue)
            .Select(m => m.ConversationId!.Value)
            .Distinct()
            .ToList();

        if (conversationIds.Count > 0)
        {
            var participants = await _context.ConversationParticipants
                .Where(p => conversationIds.Contains(p.ConversationId) && p.UserId == recipientId)
                .ToListAsync();
            foreach (var participant in participants)
            {
                participant.UnreadCount = 0;
                participant.LastReadAt = now;
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Conversation> GetOrCreateConversationAsync(string userA, string userB)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Participants)
            .Where(c => !c.IsGroupConversation && !c.IsDeleted)
            .Where(c => c.Participants.Any(p => p.UserId == userA) &&
                        c.Participants.Any(p => p.UserId == userB))
            .FirstOrDefaultAsync();

        if (conversation != null)
        {
            return conversation;
        }

        conversation = new Conversation
        {
            IsGroupConversation = false,
            CreatedAt = DateTime.UtcNow,
            CreatedById = userA
        };
        conversation.Participants.Add(new ConversationParticipant { UserId = userA, CreatedAt = DateTime.UtcNow });
        conversation.Participants.Add(new ConversationParticipant { UserId = userB, CreatedAt = DateTime.UtcNow });

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return conversation;
    }
}
