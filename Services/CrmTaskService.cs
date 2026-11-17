using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto.Operators;
using VertexCRM.Constants;
using VertexCRM.Data;
using VertexCRM.DTOs.CrmTask;
using VertexCRM.Entities;
using VertexCRM.Interfaces;

namespace VertexCRM.Services;

public sealed class CrmTaskService : ICrmTaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ICompanyAccessService _companyAccessService;
    private readonly ITenantContext _tenantContext;

    public CrmTaskService(ApplicationDbContext context, ICompanyAccessService companyAccessService, ITenantContext tenantContext)
    {
        _context = context;
        _companyAccessService = companyAccessService;
        _tenantContext = tenantContext;
    }

    public async Task<CrmTaskResponseDTO> CreateCrmTask(int companyId, CreateCrmTaskRequestDTO request)
    {
        await EnsureCompanyAccess(companyId);
        await EnsureContactBelongsToCompany(request.ContactId, companyId);
        await EnsureDealBelongsToCompany(request.DealId, companyId);
        await EnsureAssignedUserAccess(request.AssignedToUserId, companyId);

        var task = new CrmTask
        {
            CompanyId = companyId,
            ContactId = request.ContactId,
            DealId = request.DealId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            AssignedToUserId = request.AssignedToUserId,
            CreatedByUserId = _tenantContext.UserId!.Value,
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task DeleteCrmTask(int companyId, int taskId)
    {
        await EnsureCompanyAccess(companyId);

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.CompanyId == companyId);
        if (task is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        task.IsDeleted = true;
        // contact.DeletedOn = DateTimeOffset.UtcNow;
        // contact.DeletedByUserId = _tenantContext.UserId;

        await _context.SaveChangesAsync();

    }

    public async Task<CrmTaskResponseDTO> GetCrmTaskById(int companyId, int taskId)
    {
        await EnsureCompanyAccess(companyId);

        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Id == taskId &&
                c.CompanyId == companyId);

        if (task is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        return MapToResponse(task);
    }

    public async Task<List<CrmTaskResponseDTO>> GetCrmTasks(int companyId)
    {
        await EnsureCompanyAccess(companyId);

        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.CompanyId == companyId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();

        return [.. tasks.Select(MapToResponse)];
    }

    public async Task<CrmTaskResponseDTO> UpdateCrmTask(int companyId, int taskId, UpdateCrmTaskRequestDTO request)
    {
        await EnsureCompanyAccess(companyId);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(c =>
                c.Id == taskId &&
                c.CompanyId == companyId);

        if (task is null)
        {
            throw new KeyNotFoundException("Contact not found.");
        }

        await EnsureContactBelongsToCompany(request.ContactId, companyId);
        await EnsureDealBelongsToCompany(request.DealId, companyId);
        await EnsureAssignedUserAccess(request.AssignedToUserId, companyId);

        task.ContactId = request.ContactId;
        task.DealId = request.DealId;
        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.AssignedToUserId = request.AssignedToUserId;

        UpdateTaskStatus(task, request.Status);

        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }

    private async Task EnsureCompanyAccess(int companyId)
    {
        var hasAccess = await _companyAccessService
            .CanAccessCompanyAsync(companyId);

        if (!hasAccess)
        {
            // Avoid revealing whether another tenant's company exists.
            throw new KeyNotFoundException("Company not found.");
        }
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

    private async Task EnsureAssignedUserAccess(int? assignedToUserId, int companyId)
    {
        if (!assignedToUserId.HasValue) return;

        var tenantId = _tenantContext.TenantId;

        if (!tenantId.HasValue) throw new UnauthorizedAccessException("Tenant context is unavailable");
        var hasAccess = await _context.UserCompanies.AnyAsync(
            uc => uc.UserId == assignedToUserId &&
            uc.CompanyId == companyId &&
            uc.IsActive &&
            uc.User.TenantId == tenantId.Value &&
            uc.User.IsActive &&
            uc.Company.TenantId == tenantId.Value &&
            !uc.Company.IsDeleted);

        if (!hasAccess)
        {
            throw new KeyNotFoundException("Assingned User is not a member of this company.");
        }
    }

    private static void UpdateTaskStatus(CrmTask task, CrmTaskStatus newStatus)
    {
        if (newStatus == CrmTaskStatus.Completed)
        {
            if (task.Status != CrmTaskStatus.Completed)
                task.CompletedOn = DateTimeOffset.UtcNow;
        }
        else
        {
            task.CompletedOn = null;
        }
    }
    private static CrmTaskResponseDTO MapToResponse(CrmTask task)
    {
        return new CrmTaskResponseDTO
        {
            Id = task.Id,
            CompanyId = task.CompanyId,
            ContactId = task.ContactId,
            DealId = task.DealId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CompletedOn = task.CompletedOn,
            AssignedToUserId = task.AssignedToUserId,
            CreatedByUserId = task.CreatedByUserId,
        };
    }
}
