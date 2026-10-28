using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Utilities.Pagination
{
    // Se aplica en el validador de cada consulta paginada:
    // RuleFor(query => query.Pagination).SetValidator(new PaginationRequestValidator());
    public sealed class PaginationRequestValidator : AbstractValidator<PaginationRequest>
    {
        // Evita que (PageNumber - 1) * PageSize se desborde al calcular cuántos registros saltar.
        public const int MAX_PAGE_NUMBER = int.MaxValue / PaginationRequest.MAX_PAGE_SIZE;

        public PaginationRequestValidator()
        {
            RuleFor(pagination => pagination.PageNumber).InclusiveBetween(1, MAX_PAGE_NUMBER)
                                                        .WithMessage($"El número de página debe estar entre 1 y {MAX_PAGE_NUMBER}.");

            RuleFor(pagination => pagination.PageSize).GreaterThan(0)
                                                      .WithMessage("El tamaño de página debe ser mayor a 0.");
        }
    }
}
