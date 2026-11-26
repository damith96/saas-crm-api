using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VertexCRM.Data;
using VertexCRM.DTOs.Common;
using VertexCRM.DTOs.Company;
using VertexCRM.Entities;
using VertexCRM.Interfaces;

namespace VertexCRM.Services;

public sealed class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CompanyService(ApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<List<AllowedCompanyResponseDTO>> GetAllowedCompanies()
    {
        var userId = _tenantContext.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Authenticated user context is unavailable. ");
        }


        var companies = await _context.UserCompanies
            .Where(uc => uc.UserId == userId)
            .Select(uc => new AllowedCompanyResponseDTO
            {
                Id = uc.Company.Id,
                Name = uc.Company.Name,
                Description = uc.Company.Description
            })
            .ToListAsync();

        return companies;
    }

    public async Task<PaginatedResponseDTO<CompanyListResponseDTO>> GetCompanies(PaginationRequestDTO request)
    {
        var query = _context.Companies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(c => c.Name.Contains(search) || (c.Description != null && c.Description.Contains(search)));
        }

        // Operational status filtering
        // if (!string.IsNullOrWhiteSpace(request.Status) &&
        //     Enum.TryParse<CompanyStatus>(
        //         request.Status,
        //         true,
        //         out var status))
        // {
        //     query = query.Where(company =>
        //         company.Status == status);
        // }

        var totalRecords = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var companies = await query
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CompanyListResponseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();

        return new PaginatedResponseDTO<CompanyListResponseDTO>
        {
            Data = companies,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }


    public async Task<CompanyListResponseDTO> CreateCompanyAsync(
        CreateCompanyRequestDTO request)
    {
        var tenantId = _tenantContext.TenantId;

        var companyName = request.Name.Trim();

        var companyExists = await _context.Companies
            .AnyAsync(c => c.Name == companyName);

        if (companyExists)
        {
            throw new InvalidOperationException(
                "A company with this name already exists.");
        }

        var company = new Company
        {
            Name = companyName,
            Description = request.Description?.Trim(),
            TenantId = tenantId.Value
        };

        _context.Companies.Add(company);

        await _context.SaveChangesAsync();

        return new CompanyListResponseDTO
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description
        };
    }

    public async Task<CompanyListResponseDTO?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CompanyListResponseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CompanyListResponseDTO?> UpdateCompanyAsync(
    int id,
    UpdateCompanyRequestDTO request)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
            return null;

        var companyName = request.Name.Trim();

        var duplicateExists = await _context.Companies
            .AnyAsync(c => c.Id != id && c.Name == companyName);

        if (duplicateExists)
        {
            throw new InvalidOperationException(
                "A company with this name already exists.");
        }

        company.Name = companyName;
        company.Description = request.Description?.Trim();

        await _context.SaveChangesAsync();

        return new CompanyListResponseDTO
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description
        };
    }

    public async Task<bool> DeleteCompany(int id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
        {
            return false;
        }

        company.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}