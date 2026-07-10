using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Services;

public interface IContentService<T> where T : PublishableEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IList<T>> GetAllAsync(bool includeDrafts = false);
    Task<IList<T>> GetByStatusAsync(ContentStatus status);
    Task<IList<T>> GetPendingReviewAsync();
    Task<IList<T>> GetPublishedAsync();
    Task<T> CreateAsync(T entity, string userId);
    Task<T> UpdateAsync(T entity, string userId);
    Task DeleteAsync(int id, string userId);
    Task RequestPublishAsync(int id, string userId);
    Task ApproveAsync(int id, string reviewerId, string? notes = null);
    Task RejectAsync(int id, string reviewerId, string notes);
    Task PublishToWebAsync(int id, string reviewerId);
    Task UnpublishAsync(int id, string userId);
    Task ArchiveAsync(int id, string userId);
}

public class ContentService<T> : IContentService<T> where T : PublishableEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public ContentService(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public virtual async Task<IList<T>> GetAllAsync(bool includeDrafts = false)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);

        if (!includeDrafts)
        {
            query = query.Where(e => e.Status != ContentStatus.Draft);
        }

        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
    }

    public virtual async Task<IList<T>> GetByStatusAsync(ContentStatus status)
    {
        return await _dbSet
            .Where(e => e.Status == status && !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public virtual async Task<IList<T>> GetPendingReviewAsync()
    {
        return await _dbSet
            .Where(e => e.Status == ContentStatus.PendingReview && !e.IsDeleted)
            .OrderByDescending(e => e.PublishRequestedAt)
            .ToListAsync();
    }

    public virtual async Task<IList<T>> GetPublishedAsync()
    {
        return await _dbSet
            .Where(e => e.IsPublishedToWeb && !e.IsDeleted)
            .OrderByDescending(e => e.PublishedToWebAt)
            .ToListAsync();
    }

    public virtual async Task<T> CreateAsync(T entity, string userId)
    {
        entity.CreatedById = userId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.Status = ContentStatus.Draft;
        entity.IsActive = true;

        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity, string userId)
    {
        entity.UpdatedById = userId;
        entity.UpdatedAt = DateTime.UtcNow;

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(int id, string userId)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedById = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            await HomeContentService.TouchCacheVersionAsync(_context, userId);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task RequestPublishAsync(int id, string userId)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            if (!entity.IntendedForWeb)
            {
                throw new InvalidOperationException("This item is not marked as intended for the website. Edit it and tick 'Intended for website' before requesting publication.");
            }
            entity.PublishRequested = true;
            entity.PublishRequestedAt = DateTime.UtcNow;
            entity.Status = ContentStatus.PendingReview;
            entity.UpdatedById = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task ApproveAsync(int id, string reviewerId, string? notes = null)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.Status = ContentStatus.Approved;
            entity.ReviewedById = reviewerId;
            entity.ReviewedAt = DateTime.UtcNow;
            entity.ReviewNotes = notes;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task RejectAsync(int id, string reviewerId, string notes)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.Status = ContentStatus.Rejected;
            entity.ReviewedById = reviewerId;
            entity.ReviewedAt = DateTime.UtcNow;
            entity.ReviewNotes = notes;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task PublishToWebAsync(int id, string reviewerId)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            if (!entity.IntendedForWeb)
            {
                throw new InvalidOperationException("This item is not marked as intended for the website and cannot be published to the web.");
            }
            entity.Status = ContentStatus.Published;
            entity.IsPublishedToWeb = true;
            entity.PublishedToWebAt = DateTime.UtcNow;
            entity.PublishedById = reviewerId;
            entity.ReviewedById = reviewerId;
            entity.ReviewedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await HomeContentService.TouchCacheVersionAsync(_context, reviewerId);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task UnpublishAsync(int id, string userId)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.IsPublishedToWeb = false;
            entity.Status = ContentStatus.Draft;
            entity.UpdatedById = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            await HomeContentService.TouchCacheVersionAsync(_context, userId);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task ArchiveAsync(int id, string userId)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.Status = ContentStatus.Archived;
            entity.IsPublishedToWeb = false;
            entity.UpdatedById = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            await HomeContentService.TouchCacheVersionAsync(_context, userId);
            await _context.SaveChangesAsync();
        }
    }
}
