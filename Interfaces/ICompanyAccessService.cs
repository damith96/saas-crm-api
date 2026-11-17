namespace VertexCRM.Interfaces;

public interface ICompanyAccessService
{
    Task<bool> CanAccessCompanyAsync(int companyId);
}