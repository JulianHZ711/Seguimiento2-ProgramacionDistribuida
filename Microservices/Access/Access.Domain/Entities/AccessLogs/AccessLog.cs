using Access.Domain.Entities.AccessLogs.ValueObjects;
using Access.Domain.Entities.AccessPoints;
using Access.Domain.Entities.Credentials;
using Access.Domain.Entities.Employees;
using Access.Domain.Exceptions;

namespace Access.Domain.Entities.AccessLogs
{
    public sealed class AccessLog
    {
        public const int ReasonMaxLength = 500;

        public Guid Id { get; private set; }
        public string CredentialCode { get; private set; } = null!;
        public Guid? CredentialId { get; private set; }
        public Credential? Credential { get; private set; }
        public Guid? EmployeeId { get; private set; }
        public Employee? Employee { get; private set; }
        public Guid AccessPointId { get; private set; }
        public AccessPoint AccessPoint { get; private set; } = null!;
        public MovementType MovementType { get; private set; }
        public AccessResult Result { get; private set; }
        public string? Reason { get; private set; }
        public DateTime OccurredAt { get; private set; }

        private AccessLog()
        {
        }

        // El caso de uso determina el resultado; el registro conserva también los rechazos.
        public AccessLog(string credentialCode,
                         Guid accessPointId,
                         MovementType movementType,
                         AccessResult result,
                         string? reason = null,
                         Guid? employeeId = null,
                         Guid? credentialId = null)
        {
            Credentials.Credential.ApplyCodeRules(credentialCode);
            ApplyAccessPointIdRules(accessPointId);
            ApplyMovementTypeRules(movementType);
            ApplyResultRules(result);
            ApplyIdentityRules(employeeId, credentialId, result);
            ApplyReasonRules(reason, result);

            Id = Guid.CreateVersion7();
            CredentialCode = credentialCode.Trim().ToUpperInvariant();
            AccessPointId = accessPointId;
            MovementType = movementType;
            Result = result;
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            EmployeeId = employeeId;
            CredentialId = credentialId;
            OccurredAt = DateTime.UtcNow;
        }

        private static void ApplyAccessPointIdRules(Guid accessPointId)
        {
            if (accessPointId == Guid.Empty)
            {
                throw new BussinesRuleException("El punto de acceso es requerido.");
            }
        }

        private static void ApplyMovementTypeRules(MovementType movementType)
        {
            if (!Enum.IsDefined(movementType))
            {
                throw new BussinesRuleException("El tipo de movimiento no es válido.");
            }
        }

        private static void ApplyResultRules(AccessResult result)
        {
            if (!Enum.IsDefined(result))
            {
                throw new BussinesRuleException("El resultado del acceso no es válido.");
            }
        }

        private static void ApplyIdentityRules(Guid? employeeId, Guid? credentialId, AccessResult result)
        {
            if (employeeId == Guid.Empty || credentialId == Guid.Empty)
            {
                throw new BussinesRuleException("Los identificadores del empleado y la credencial no pueden estar vacíos.");
            }

            if (result == AccessResult.Authorized && (employeeId is null || credentialId is null))
            {
                throw new BussinesRuleException("Un acceso autorizado debe identificar al empleado y a su credencial.");
            }
        }

        private static void ApplyReasonRules(string? reason, AccessResult result)
        {
            if (result == AccessResult.Rejected && string.IsNullOrWhiteSpace(reason))
            {
                throw new BussinesRuleException("El motivo del rechazo es requerido.");
            }

            if (reason is not null && reason.Trim().Length > ReasonMaxLength)
            {
                throw new BussinesRuleException($"El motivo debe tener máximo {ReasonMaxLength} caracteres.");
            }
        }
    }
}
