using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int? adminId, string adminName, string action,
                      string? targetType = null, string? targetId = null, string? details = null);

        Task<PagedResult<AuditLogDto>> SearchAsync(AuditLogFilterDto filter);
    }
}
