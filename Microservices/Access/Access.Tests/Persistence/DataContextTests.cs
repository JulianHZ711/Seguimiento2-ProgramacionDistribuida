using Access.Domain.Entities.AccessLogs;
using Access.Domain.Entities.AccessLogs.ValueObjects;
using Access.Domain.Entities.AccessPoints;
using Access.Domain.Entities.AccessPoints.ValueObjects;
using Access.Domain.Entities.Areas;
using Access.Domain.Entities.Credentials;
using Access.Domain.Entities.Employees;
using Access.Domain.Entities.Employees.ValueObjects;
using Access.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Access.Tests.Persistence
{
    [TestClass]
    public sealed class DataContextTests
    {
        // Se usa el proveedor real para validar modelo, DDL y traducción, sin conectarse a una base de datos.
        private static DataContext CreateContext()
        {
            return new DataContext(new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer("Server=localhost;Database=AccessModelTests;Integrated Security=true;TrustServerCertificate=true")
                .Options);
        }

        [TestMethod]
        public void SqlServerSchema_ContainsFiveTablesAndRequiredUniqueIndexes()
        {
            using DataContext context = CreateContext();
            string sql = context.Database.GenerateCreateScript();

            foreach (string table in new[] { "Areas", "Employees", "Credentials", "AccessPoints", "AccessLogs" })
            {
                StringAssert.Contains(sql, $"CREATE TABLE [{table}]");
            }
            StringAssert.Contains(sql, "CREATE UNIQUE INDEX [IX_Employees_DocumentNumber] ON [Employees] ([DocumentNumber])");
            StringAssert.Contains(sql, "CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email])");
            StringAssert.Contains(sql, "CREATE UNIQUE INDEX [IX_Credentials_Code] ON [Credentials] ([Code])");
            StringAssert.Contains(sql, "[DocumentNumber] nvarchar(20) NOT NULL");
            StringAssert.Contains(sql, "[Email] nvarchar(150) NOT NULL");
            StringAssert.Contains(sql, "[Code] nvarchar(50) NOT NULL");
            Assert.IsFalse(sql.Contains("ON DELETE CASCADE", StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(5, context.Model.GetEntityTypes().Count());
            Assert.IsFalse(context.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties())
                .Any(property => property.IsShadowProperty()));
        }

        [TestMethod]
        public void EmployeeValueObjects_CanBeConvertedAndComparedInSql()
        {
            using DataContext context = CreateContext();
            Email email = new("  EMPLEADO@EMPRESA.COM  ");
            DocumentNumber document = new("  ab12345  ");
            var entity = context.Model.FindEntityType(typeof(Employee))!;
            var emailConverter = entity.FindProperty(nameof(Employee.Email))!.GetTypeMapping().Converter!;
            var documentConverter = entity.FindProperty(nameof(Employee.DocumentNumber))!.GetTypeMapping().Converter!;

            Assert.AreEqual("empleado@empresa.com", emailConverter.ConvertToProvider(email));
            Assert.AreEqual(email, emailConverter.ConvertFromProvider("empleado@empresa.com"));
            Assert.AreEqual("AB12345", documentConverter.ConvertToProvider(document));
            Assert.AreEqual(document, documentConverter.ConvertFromProvider("AB12345"));

            string sql = context.Employees.Where(employee => employee.Email == email || employee.DocumentNumber == document)
                .ToQueryString();
            StringAssert.Contains(sql, "WHERE");
            StringAssert.Contains(sql, "[Email] =");
            StringAssert.Contains(sql, "[DocumentNumber] =");
        }

        [TestMethod]
        public void UnknownCredentialRejection_CanBeTrackedWithNullReferences()
        {
            using DataContext context = CreateContext();
            AccessLog log = new("DESCONOCIDA", Guid.NewGuid(), MovementType.Entry,
                AccessResult.Rejected, "Credencial no encontrada.");
            Guid originalId = log.Id;

            context.AccessLogs.Add(log);

            Assert.AreEqual(EntityState.Added, context.Entry(log).State);
            Assert.AreEqual(originalId, log.Id);
            Assert.IsFalse(context.Entry(log).Property(item => item.Id).IsTemporary);
            Assert.IsNull(log.EmployeeId);
            Assert.IsNull(log.CredentialId);
            var entity = context.Model.FindEntityType(typeof(AccessLog))!;
            Assert.IsTrue(entity.FindProperty(nameof(AccessLog.EmployeeId))!.IsNullable);
            Assert.IsTrue(entity.FindProperty(nameof(AccessLog.CredentialId))!.IsNullable);
            Assert.IsFalse(entity.FindProperty(nameof(AccessLog.AccessPointId))!.IsNullable);
        }

        [TestMethod]
        public void DeletingTrackedPrincipals_DoesNotDeleteOrUnlinkAccessHistory()
        {
            using DataContext context = CreateContext();
            Area area = new("Logística");
            Employee employee = new(new DocumentNumber("12345678"), "Ana", "Pérez",
                new Email("ana@empresa.com"), "Operadora", area.Id);
            Credential credential = new("ABC-123", employee.Id);
            AccessPoint point = new("Recepción", AccessPointType.Reception);
            AccessLog log = new(credential.Code, point.Id, MovementType.Entry, AccessResult.Authorized,
                employeeId: employee.Id, credentialId: credential.Id);
            context.AttachRange(area, employee, credential, point, log);

            context.Remove(employee);
            context.Remove(credential);
            context.Remove(point);
            context.ChangeTracker.DetectChanges();

            Assert.AreEqual(EntityState.Unchanged, context.Entry(log).State);
            Assert.AreEqual(employee.Id, log.EmployeeId);
            Assert.AreEqual(credential.Id, log.CredentialId);
            Assert.AreEqual(point.Id, log.AccessPointId);
        }

        [TestMethod]
        public void SqlServerDates_RestoreUtcKindIncludingNullableDates()
        {
            using DataContext context = CreateContext();
            DateTime stored = new(2026, 10, 27, 12, 30, 0, DateTimeKind.Unspecified);
            foreach (var property in context.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties())
                .Where(property => property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)))
            {
                var converter = property.GetTypeMapping().Converter!;
                DateTime restored = (DateTime)converter.ConvertFromProvider(stored)!;
                Assert.AreEqual(DateTimeKind.Utc, restored.Kind, property.Name);
                Assert.AreEqual(stored.Ticks, restored.Ticks, property.Name);
                if (property.IsNullable)
                {
                    Assert.IsNull(converter.ConvertFromProvider(null));
                }
            }
        }

        [TestMethod]
        public void AccessHistoryFiltersAndOrdering_TranslateToSqlServer()
        {
            using DataContext context = CreateContext();
            Guid employeeId = Guid.NewGuid();
            DateTime from = new(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            string sql = context.AccessLogs
                .Where(log => log.EmployeeId == employeeId && log.OccurredAt >= from && log.Result == AccessResult.Authorized)
                .OrderByDescending(log => log.OccurredAt).ThenByDescending(log => log.Id)
                .Skip(15).Take(15).ToQueryString();

            StringAssert.Contains(sql, "WHERE");
            StringAssert.Contains(sql, "[EmployeeId] =");
            StringAssert.Contains(sql, "[OccurredAt] >=");
            StringAssert.Contains(sql, "ORDER BY");
            StringAssert.Contains(sql, "OFFSET");
            StringAssert.Contains(sql, "FETCH NEXT");
        }
    }
}
