using Microsoft.EntityFrameworkCore;
using VertexCRM.Data;
using VertexCRM.Interfaces;

public class CompanyAccessService : ICompanyAccessService
{
    private readonly ITenantContext _tenantContext;
    private readonly ApplicationDbContext _context;

    public CompanyAccessService(
        ITenantContext tenantContext,
        ApplicationDbContext context)
    {
        _tenantContext = tenantContext;
        _context = context;
    }

    public async Task<bool> CanAccessCompanyAsync(int companyId)
    {
        if (_tenantContext.IsPlatformAdmin)
        {
            return true;
        }

        var userId = _tenantContext.UserId;

        if (userId is null)
        {
            return false;
        }

        var hasAccess = await _context.UserCompanies
            .AnyAsync(uc =>
                uc.CompanyId == companyId &&
                uc.UserId == userId.Value && !uc.Company.IsDeleted);

        return hasAccess;
    }
}