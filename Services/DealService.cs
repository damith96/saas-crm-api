using Microsoft.EntityFrameworkCore;
using VertexCRM.Interfaces;
using VertexCRM.Data;
using VertexCRM.DTOs.Deals;
using VertexCRM.Entities;
using VertexCRM.Constants;
using VertexCRM.DTOs.Deal;

namespace VertexCRM.Services;

public sealed class DealService : IDealService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DealService(
        ApplicationDbContext context,
        ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    // CREATE DEAL
    public async Task<DealResponseDTO> CreateDeal(
        int companyId,
        CreateDealRequestDTO request)
    {
        await EnsureContactBelongsToCompany(request.ContactId, companyId);

        var deal = new Deal
        {
            CompanyId = companyId,
            ContactId = request.ContactId,
            Title = request.Title,
            Description = request.Description,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            Stage = request.Stage,
            ExpectedCloseDate = request.ExpectedCloseDate,

            ClosedOn = IsClosedStage(request.Stage)
                ? DateTimeOffset.UtcNow
                : null
        };

        _context.Deals.Add(deal);

        await _context.SaveChangesAsync();

        return MapToResponse(deal);
    }

    // GET ALL DEALS FOR A COMPANY
    public async Task<List<DealResponseDTO>> GetDeals(int companyId)
    {
        var deals = await _context.Deals
             .AsNoTracking()
             .Where(d => d.CompanyId == companyId)
             .OrderByDescending(d => d.Id)
             .ToListAsync();

        return deals.Select(MapToResponse).ToList();
    }

    // GET A SINGLE DEAL
    public async Task<DealResponseDTO> GetDealById(
        int companyId,
        int dealId)
    {
        var deal = await _context.Deals
             .AsNoTracking()
             .FirstOrDefaultAsync(d =>
                 d.Id == dealId &&
                 d.CompanyId == companyId);

        if (deal is null)
            throw new KeyNotFoundException("Deal not found.");

        return MapToResponse(deal);
    }

    // UPDATE DEAL
    public async Task<DealResponseDTO> UpdateDeal(
        int companyId,
        int dealId,
        UpdateDealRequestDTO request)
    {
        var deal = await _context.Deals
            .FirstOrDefaultAsync(d =>
                d.Id == dealId &&
                d.CompanyId == companyId);

        if (deal is null)
            throw new KeyNotFoundException("Deal not found.");

        await EnsureContactBelongsToCompany(request.ContactId, companyId);

        deal.ContactId = request.ContactId;
        deal.Title = request.Title;
        deal.Description = request.Description;
        deal.Amount = request.Amount;
        deal.CurrencyCode = request.CurrencyCode;
        deal.ExpectedCloseDate = request.ExpectedCloseDate;

        UpdateDealStage(deal, request.Stage);

        await _context.SaveChangesAsync();

        return MapToResponse(deal);
    }

    // SOFT DELETE DEAL
    public async Task DeleteDeal(int companyId, int dealId)
    {
        var deal = await _context.Deals
             .FirstOrDefaultAsync(d =>
                 d.Id == dealId &&
                 d.CompanyId == companyId);

        if (deal is null)
            throw new KeyNotFoundException("Deal not found.");

        deal.IsDeleted = true;
        deal.DeletedOn = DateTimeOffset.UtcNow;
        deal.DeletedByUserId = _tenantContext.UserId;

        await _context.SaveChangesAsync();
    }

    // CHECK WHETHER CONTACT BELONGS TO THE SAME COMPANY
    private async Task EnsureContactBelongsToCompany(
        int? contactId,
        int companyId)
    {
        if (!contactId.HasValue)
            return;

        var contactExists = await _context.Contacts
            .AnyAsync(c =>
                c.Id == contactId.Value &&
                c.CompanyId == companyId);

        if (!contactExists)
            throw new KeyNotFoundException("Contact not found.");
    }

    // CHECK CLOSED STAGES
    private static bool IsClosedStage(DealStage stage)
    {
        return stage is DealStage.Won or DealStage.Lost;
    }

    // HANDLE STAGE CHANGES
    private static void UpdateDealStage(Deal deal, DealStage newStage)
    {
        var wasClosed = IsClosedStage(deal.Stage);
        var isClosed = IsClosedStage(newStage);

        if (!wasClosed && isClosed)
        {
            deal.ClosedOn = DateTimeOffset.UtcNow;
        }
        else if (wasClosed && !isClosed)
        {
            deal.ClosedOn = null;
        }

        deal.Stage = newStage;
    }

    // MAP ENTITY TO RESPONSE DTO
    private static DealResponseDTO MapToResponse(Deal deal)
    {
        return new DealResponseDTO
        {
            Id = deal.Id,
            CompanyId = deal.CompanyId,
            ContactId = deal.ContactId,
            Title = deal.Title,
            Description = deal.Description,
            Amount = deal.Amount,
            CurrencyCode = deal.CurrencyCode,
            Stage = deal.Stage,
            ExpectedCloseDate = deal.ExpectedCloseDate,
            ClosedOn = deal.ClosedOn
        };
    }
}