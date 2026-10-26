using Access.Domain.Entities.Credentials;
using Access.Domain.Entities.Credentials.ValueObjects;
using Access.Domain.Exceptions;

namespace Access.Tests.Domain
{
    [TestClass]
    public sealed class CredentialTests
    {
        [TestMethod]
        public void Constructor_NormalizesCodeAndStartsActive()
        {
            Guid employeeId = Guid.NewGuid();
            DateTime before = DateTime.UtcNow;

            Credential credential = new("  abc-123  ", employeeId);

            Assert.AreNotEqual(Guid.Empty, credential.Id);
            Assert.AreEqual("ABC-123", credential.Code);
            Assert.AreEqual(employeeId, credential.EmployeeId);
            Assert.AreEqual(CredentialStatus.Active, credential.Status);
            Assert.IsTrue(credential.IsActive);
            Assert.IsNull(credential.BlockedAt);
            Assert.IsNull(credential.RevokedAt);
            Assert.AreEqual(DateTimeKind.Utc, credential.CreatedAt.Kind);
            Assert.IsTrue(credential.CreatedAt >= before && credential.CreatedAt <= DateTime.UtcNow);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_RejectsMissingCode(string? code)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() => new Credential(code!, Guid.NewGuid()));
        }

        [TestMethod]
        public void Constructor_EnforcesCodeLength()
        {
            Credential credential = new(new string('A', Credential.CodeMaxLength), Guid.NewGuid());

            Assert.AreEqual(Credential.CodeMaxLength, credential.Code.Length);
            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new Credential(new string('A', Credential.CodeMaxLength + 1), Guid.NewGuid()));
        }

        [TestMethod]
        public void Constructor_RejectsEmptyEmployeeId()
        {
            Assert.ThrowsExactly<BussinesRuleException>(() => new Credential("ABC-123", Guid.Empty));
        }

        [TestMethod]
        public void Block_PreventsUseAndRejectsRepeatedBlocking()
        {
            Credential credential = new("ABC-123", Guid.NewGuid());

            credential.Block();
            DateTime? blockedAt = credential.BlockedAt;

            Assert.AreEqual(CredentialStatus.Blocked, credential.Status);
            Assert.IsFalse(credential.IsActive);
            Assert.IsNotNull(blockedAt);
            Assert.ThrowsExactly<BussinesRuleException>(() => credential.Block());
            Assert.AreEqual(blockedAt, credential.BlockedAt);
            Assert.AreEqual(CredentialStatus.Blocked, credential.Status);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Revoke_IsFinalForActiveAndBlockedCredentials(bool blockFirst)
        {
            Credential credential = new("ABC-123", Guid.NewGuid());
            if (blockFirst)
            {
                credential.Block();
            }
            DateTime? blockedAt = credential.BlockedAt;

            credential.Revoke();
            DateTime? revokedAt = credential.RevokedAt;

            Assert.AreEqual(CredentialStatus.Revoked, credential.Status);
            Assert.IsFalse(credential.IsActive);
            Assert.IsNotNull(revokedAt);
            Assert.AreEqual(blockedAt, credential.BlockedAt);
            Assert.ThrowsExactly<BussinesRuleException>(() => credential.Block());
            Assert.ThrowsExactly<BussinesRuleException>(() => credential.Revoke());
            Assert.AreEqual(CredentialStatus.Revoked, credential.Status);
            Assert.AreEqual(revokedAt, credential.RevokedAt);
        }
    }
}
