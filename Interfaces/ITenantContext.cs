namespace VertexCRM.Interfaces;

public interface ITenantContext
{
    public int? UserId { get; }
    public int? TenantId { get; }
    public bool IsPlatformAdmin { get; }
}