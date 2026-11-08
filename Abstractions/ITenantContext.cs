namespace VertexCRM.Abstractions;

public interface ITenantContext
{
    public int? UserId { get; }
    public int? TenantId { get; }
    public bool IsPlatformAdmin { get; }
}