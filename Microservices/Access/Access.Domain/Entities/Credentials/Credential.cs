using Access.Domain.Entities.Credentials.ValueObjects;
using Access.Domain.Entities.Employees;
using Access.Domain.Exceptions;

namespace Access.Domain.Entities.Credentials
{
    public sealed class Credential
    {
        public const int CodeMaxLength = 50;

        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public Guid EmployeeId { get; private set; }
        public Employee Employee { get; private set; } = null!;
        public CredentialStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? BlockedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        public bool IsActive => Status == CredentialStatus.Active;

        private Credential()
        {
        }

        public Credential(string code, Guid employeeId)
        {
            ApplyCodeRules(code);
            ApplyEmployeeIdRules(employeeId);

            Id = Guid.CreateVersion7();
            Code = code.Trim().ToUpperInvariant();
            EmployeeId = employeeId;
            Status = CredentialStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public void Block()
        {
            ApplyActiveRules();

            Status = CredentialStatus.Blocked;
            BlockedAt = DateTime.UtcNow;
        }

        public void Revoke()
        {
            ApplyNotRevokedRules();

            Status = CredentialStatus.Revoked;
            RevokedAt = DateTime.UtcNow;
        }

        // La unicidad del código se verifica en el caso de uso y en la base de datos.
        internal static void ApplyCodeRules(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BussinesRuleException("El código de la credencial es requerido.");
            }

            if (code.Trim().Length > CodeMaxLength)
            {
                throw new BussinesRuleException($"El código de la credencial debe tener máximo {CodeMaxLength} caracteres.");
            }
        }

        private static void ApplyEmployeeIdRules(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                throw new BussinesRuleException("El empleado de la credencial es requerido.");
            }
        }

        private void ApplyActiveRules()
        {
            if (!IsActive)
            {
                throw new BussinesRuleException("Solo se puede bloquear una credencial activa.");
            }
        }

        private void ApplyNotRevokedRules()
        {
            if (Status == CredentialStatus.Revoked)
            {
                throw new BussinesRuleException("La credencial ya se encuentra revocada.");
            }
        }
    }
}
