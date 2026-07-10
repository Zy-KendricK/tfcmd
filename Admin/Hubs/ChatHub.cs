using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Admin.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        await base.OnConnectedAsync();
    }

    public async Task<object> SendMessage(string recipientId, string content)
    {
        var senderId = GetCurrentUserId();
        var trimmed = content?.Trim();

        if (string.IsNullOrWhiteSpace(recipientId) || string.IsNullOrWhiteSpace(trimmed))
        {
            throw new HubException("Message is empty.");
        }

        if (trimmed.Length > 2000)
        {
            trimmed = trimmed[..2000];
        }

        var recipient = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == recipientId && u.IsActive && !u.IsDeleted);
        if (recipient == null)
        {
            throw new HubException("Recipient not found.");
        }

        var conversation = await GetOrCreateConversationAsync(senderId, recipient.Id);

        var message = new Message
        {
            SenderId = senderId,
            RecipientId = recipient.Id,
            Content = trimmed,
            ConversationId = conversation.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedById = senderId
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

        var payload = new
        {
            id = message.Id,
            senderId = message.SenderId,
            content = message.Content,
            sentAt = message.CreatedAt
        };

        await Clients.Group($"user-{recipient.Id}").SendAsync("ReceiveMessage", payload);
        return payload;
    }

    public async Task MarkAsRead(string senderId)
    {
        if (string.IsNullOrWhiteSpace(senderId))
        {
            return;
        }

        await MarkReadAsync(senderId, GetCurrentUserId());
    }

    private string GetCurrentUserId()
    {
        return Context.UserIdentifier
            ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new HubException("User is not authenticated.");
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
