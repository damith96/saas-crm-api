using Microsoft.AspNetCore.Mvc.Filters;
using VertexCRM.Data;
using VertexCRM.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace VertexCRM.Filters;

public sealed class CompanyAccessFilter : IAsyncActionFilter
{
    private readonly ITenantContext _tenantContext;
    private readonly ApplicationDbContext _context;
    public CompanyAccessFilter(ITenantContext tenantContext,
        ApplicationDbContext context)
    {
        _tenantContext = tenantContext;
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var companyIdValue = context.HttpContext.Request.RouteValues["companyId"]?.ToString();

        if (!int.TryParse(companyIdValue, out var companyId))
        {
            context.Result = new BadRequestObjectResult(
                "Invalid company ID.");
            return;
        }

        var userId = _tenantContext.UserId;
        var tenantId = _tenantContext.TenantId;

        if (!userId.HasValue || !tenantId.HasValue)
        {
            context.Result = new ForbidResult();
            return;
        }

        var hasAccess = await _context.UserCompanies
            .AnyAsync(uc =>
                uc.CompanyId == companyId &&
                uc.UserId == userId.Value &&
                uc.IsActive &&
                uc.User.IsActive &&
                uc.User.TenantId == tenantId.Value &&
                uc.Company.TenantId == tenantId.Value &&
                !uc.Company.IsDeleted);

        if (!hasAccess)
        {
            context.Result = new NotFoundResult();
            return;
        }

        await next();
    }
}