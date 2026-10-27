using Access.Application.Utilities.Pagination;
using Access.Domain.Entities.AccessLogs;
using Access.Domain.Entities.AccessLogs.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Contracts.Repositories
{
    public interface IAccessLogsRepository : IRepository<AccessLog>
    {
        Task<PaginationResponse<AccessLog>> GetPagedListAsync(PaginationRequest request,
                                                              Guid? employeeId,
                                                              DateTime? from,
                                                              DateTime? to,
                                                              Guid? accessPointId,
                                                              AccessResult? result,
                                                              CancellationToken cancellationToken = default);

        // Último movimiento autorizado del empleado, para validar que entrada y salida sean coherentes.
        Task<AccessLog?> GetLastAuthorizedByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    }
}
