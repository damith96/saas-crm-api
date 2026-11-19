using Microsoft.EntityFrameworkCore;
using VertexCRM.Data;
using VertexCRM.DTOs.Activity;
using VertexCRM.Entities;
using VertexCRM.Interfaces;

namespace VertexCRM.Services;

public class ActivityService : IActivityService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public ActivityService(ApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<ActivityResponseDTO> CreateActivity(int companyId, CreateActivityRequestDTO request)
    {
        await EnsureContactBelongsToCompany(request.ContactId, companyId);
        await EnsureDealBelongsToCompany(request.DealId, companyId);

        var userId = _tenantContext.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Authenticated user context is unavailable. ");
        }

        var activity = new Activity
        {
            CompanyId = companyId,
            ContactId = request.ContactId,
            DealId = request.DealId,
            Type = request.Type!.Value,
            Subject = request.Subject,
            Description = request.Description,
            ActivityDate = request.ActivityDate!.Value,
            CreatedByUserId = _tenantContext.UserId!.Value,
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        return MapToResponse(activity);
    }

    public async Task DeleteActivity(int companyId, int activityId)
    {
        var activity = await _context.Activities.FirstOrDefaultAsync(t => t.Id == activityId && t.CompanyId == companyId);
        if (activity is null)
        {
            throw new KeyNotFoundException("Activity not found.");
        }

        activity.IsDeleted = true;
        // contact.DeletedOn = DateTimeOffset.UtcNow;
        // contact.DeletedByUserId = _tenantContext.UserId;

        await _context.SaveChangesAsync();

    }

    public async Task<ActivityResponseDTO> GetActivityById(int companyId, int activityId)
    {
        var activity = await _context.Activities
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Id == activityId &&
                c.CompanyId == companyId);

        if (activity is null)
        {
            throw new KeyNotFoundException("Activity not found.");
        }

        return MapToResponse(activity);
    }

    public async Task<List<ActivityResponseDTO>> GetActivities(int companyId)
    {
        var activities = await _context.Activities
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId)
            .OrderByDescending(a => a.ActivityDate)
            .ToListAsync();

        return [.. activities.Select(MapToResponse)];
    }

    public async Task<ActivityResponseDTO> UpdateActivity(int companyId, int activityId, UpdateActivityRequestDTO request)
    {
        var activity = await _context.Activities
            .FirstOrDefaultAsync(a =>
                a.Id == activityId &&
                a.CompanyId == companyId);

        if (activity is null)
        {
            throw new KeyNotFoundException("Activity not found.");
        }

        await EnsureContactBelongsToCompany(request.ContactId, companyId);
        await EnsureDealBelongsToCompany(request.DealId, companyId);

        activity.ContactId = request.ContactId;
        activity.DealId = request.DealId;
        activity.Type = request.Type!.Value;
        activity.Subject = request.Subject;
        activity.Description = request.Description;
        activity.ActivityDate = request.ActivityDate!.Value;

        await _context.SaveChangesAsync();

        return MapToResponse(activity);
    }

    private async Task EnsureContactBelongsToCompany(int? contactId, int companyId)
    {
        if (!contactId.HasValue) return;
        var exists = await _context.Contacts.AnyAsync(c => c.Id == contactId && c.CompanyId == companyId);
        if (!exists)
        {
            throw new KeyNotFoundException("Contact not found.");
        }
    }

    private async Task EnsureDealBelongsToCompany(int? dealId, int companyId)
    {
        if (!dealId.HasValue) return;

        var exists = await _context.Deals.AnyAsync(c => c.Id == dealId && c.CompanyId == companyId);

        if (!exists)
        {
            throw new KeyNotFoundException("Deal not found.");
        }
    }

    private static ActivityResponseDTO MapToResponse(Activity activity)
    {
        return new ActivityResponseDTO
        {
            Id = activity.Id,
            CompanyId = activity.CompanyId,
            ContactId = activity.ContactId,
            DealId = activity.DealId,
            Type = activity.Type,
            Subject = activity.Subject,
            Description = activity.Description,
            ActivityDate = activity.ActivityDate,
            CreatedByUserId = activity.CreatedByUserId,
        };
    }
}