using Access.Application.Utilities.Pagination;
using Access.Domain.Entities.Employees;
using Access.Domain.Entities.Employees.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Contracts.Repositories
{
    public interface IEmployeesRepository : IRepository<Employee>
    {
        Task<PaginationResponse<Employee>> GetPagedListAsync(PaginationRequest request,
                                                             Guid? areaId,
                                                             EmploymentStatus? status,
                                                             CancellationToken cancellationToken = default);

        Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByDocumentNumberAsync(DocumentNumber documentNumber, CancellationToken cancellationToken = default);

        Task<bool> ExistsByEmailAsync(Email email, Guid? excludeEmployeeId = null, CancellationToken cancellationToken = default);
    }
}
