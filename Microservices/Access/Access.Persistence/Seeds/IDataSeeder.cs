using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Persistence.Seeds
{
    /// <summary>
    /// Contrato de un seeder de infraestructura.
    /// Cada implementación carga un conjunto de datos iniciales
    /// </summary>
    public interface IDataSeeder
    {
        /// <summary>
        ///  Orden de ejecución; Menor = Primero
        /// </summary>
        int Order { get; }

        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
