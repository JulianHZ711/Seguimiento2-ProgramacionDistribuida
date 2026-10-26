using Access.Domain.Entities.AccessLogs;
using Access.Domain.Entities.AccessLogs.ValueObjects;
using Access.Domain.Entities.Credentials;
using Access.Domain.Exceptions;

namespace Access.Tests.Domain
{
    [TestClass]
    public sealed class AccessLogTests
    {
        [TestMethod]
        [DataRow(MovementType.Entry)]
        [DataRow(MovementType.Exit)]
        public void Constructor_RecordsAuthorizedMovementWithIdentitiesAndUtcTime(MovementType movementType)
        {
            Guid employeeId = Guid.NewGuid();
            Guid credentialId = Guid.NewGuid();
            Guid pointId = Guid.NewGuid();
            DateTime before = DateTime.UtcNow;

            AccessLog log = new("  abc-123  ", pointId, movementType, AccessResult.Authorized,
                employeeId: employeeId, credentialId: credentialId);

            Assert.AreNotEqual(Guid.Empty, log.Id);
            Assert.AreEqual("ABC-123", log.CredentialCode);
            Assert.AreEqual(employeeId, log.EmployeeId);
            Assert.AreEqual(credentialId, log.CredentialId);
            Assert.AreEqual(pointId, log.AccessPointId);
            Assert.AreEqual(movementType, log.MovementType);
            Assert.AreEqual(AccessResult.Authorized, log.Result);
            Assert.IsNull(log.Reason);
            Assert.AreEqual(DateTimeKind.Utc, log.OccurredAt.Kind);
            Assert.IsTrue(log.OccurredAt >= before && log.OccurredAt <= DateTime.UtcNow);
        }

        [TestMethod]
        public void Constructor_PreservesRejectedAttemptWithUnknownCredential()
        {
            AccessLog log = new("  desconocida  ", Guid.NewGuid(), MovementType.Entry,
                AccessResult.Rejected, "  Credencial no encontrada.  ");

            Assert.AreEqual(AccessResult.Rejected, log.Result);
            Assert.AreEqual("DESCONOCIDA", log.CredentialCode);
            Assert.AreEqual("Credencial no encontrada.", log.Reason);
            Assert.IsNull(log.EmployeeId);
            Assert.IsNull(log.CredentialId);
        }

        [TestMethod]
        public void Constructor_PreservesRejectedAttemptWithKnownCredential()
        {
            Guid employeeId = Guid.NewGuid();
            Guid credentialId = Guid.NewGuid();

            AccessLog log = new("ABC-123", Guid.NewGuid(), MovementType.Exit,
                AccessResult.Rejected, "Credencial bloqueada.", employeeId, credentialId);

            Assert.AreEqual(AccessResult.Rejected, log.Result);
            Assert.AreEqual(employeeId, log.EmployeeId);
            Assert.AreEqual(credentialId, log.CredentialId);
            Assert.AreEqual("Credencial bloqueada.", log.Reason);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_RequiresReasonForRejection(string? reason)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.NewGuid(), MovementType.Entry, AccessResult.Rejected, reason));
        }

        [TestMethod]
        public void Constructor_EnforcesReasonLength()
        {
            AccessLog log = new("ABC-123", Guid.NewGuid(), MovementType.Entry,
                AccessResult.Rejected, new string('A', AccessLog.ReasonMaxLength));

            Assert.AreEqual(AccessLog.ReasonMaxLength, log.Reason!.Length);
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.NewGuid(), MovementType.Entry, AccessResult.Rejected,
                    new string('A', AccessLog.ReasonMaxLength + 1)));
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public void Constructor_RequiresBothIdentitiesForAuthorization(bool hasEmployee, bool hasCredential)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.NewGuid(), MovementType.Entry, AccessResult.Authorized,
                    employeeId: hasEmployee ? Guid.NewGuid() : null,
                    credentialId: hasCredential ? Guid.NewGuid() : null));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Constructor_RejectsEmptyOptionalIdentities(bool emptyEmployee)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.NewGuid(), MovementType.Entry, AccessResult.Rejected, "Rechazado.",
                    employeeId: emptyEmployee ? Guid.Empty : null,
                    credentialId: emptyEmployee ? null : Guid.Empty));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(99)]
        public void Constructor_RejectsUndefinedMovementTypes(int movementType)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.NewGuid(), (MovementType)movementType, AccessResult.Rejected, "Rechazado."));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(99)]
        public void Constructor_RejectsUndefinedResults(int result)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.NewGuid(), MovementType.Entry, (AccessResult)result, "Rechazado."));
        }

        [TestMethod]
        public void Constructor_RejectsEmptyAccessPointId()
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog("ABC-123", Guid.Empty, MovementType.Entry, AccessResult.Rejected, "Rechazado."));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_RequiresPresentedCode(string? code)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog(code!, Guid.NewGuid(), MovementType.Entry, AccessResult.Rejected, "Rechazado."));
        }

        [TestMethod]
        public void Constructor_UsesTheSameCodeLengthLimitAsCredential()
        {
            AccessLog log = new(new string('A', Credential.CodeMaxLength), Guid.NewGuid(),
                MovementType.Entry, AccessResult.Rejected, "Credencial no encontrada.");

            Assert.AreEqual(Credential.CodeMaxLength, log.CredentialCode.Length);
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessLog(new string('A', Credential.CodeMaxLength + 1), Guid.NewGuid(),
                    MovementType.Entry, AccessResult.Rejected, "Credencial no encontrada."));
        }
    }
}
