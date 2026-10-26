using Access.Domain.Entities.AccessPoints;
using Access.Domain.Entities.AccessPoints.ValueObjects;
using Access.Domain.Exceptions;

namespace Access.Tests.Domain
{
    [TestClass]
    public sealed class AccessPointTests
    {
        [TestMethod]
        [DataRow(AccessPointType.Reception)]
        [DataRow(AccessPointType.Parking)]
        [DataRow(AccessPointType.Warehouse)]
        [DataRow(AccessPointType.RestrictedArea)]
        public void Constructor_AcceptsSupportedTypesAndStartsEnabled(AccessPointType type)
        {
            AccessPoint point = new("  Acceso principal  ", type);

            Assert.AreNotEqual(Guid.Empty, point.Id);
            Assert.AreEqual("Acceso principal", point.Name);
            Assert.AreEqual(type, point.Type);
            Assert.IsTrue(point.IsEnabled);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(" A ")]
        public void Constructor_RejectsInvalidNames(string? name)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() => new AccessPoint(name!, AccessPointType.Reception));
        }

        [TestMethod]
        public void Constructor_EnforcesNameLengthBoundaries()
        {
            _ = new AccessPoint(new string('A', AccessPoint.NameMinLength), AccessPointType.Reception);
            _ = new AccessPoint(new string('A', AccessPoint.NameMaxLength), AccessPointType.Reception);

            Assert.ThrowsExactly<BussinesRuleException>(() =>
                new AccessPoint(new string('A', AccessPoint.NameMaxLength + 1), AccessPointType.Reception));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(99)]
        public void Constructor_RejectsUndefinedTypes(int type)
        {
            Assert.ThrowsExactly<BussinesRuleException>(() => new AccessPoint("Recepción", (AccessPointType)type));
        }

        [TestMethod]
        public void EnableAndDisable_ChangeAvailabilityAndRejectRepeatedTransitions()
        {
            AccessPoint point = new("Bodega", AccessPointType.Warehouse, isEnabled: false);

            Assert.IsFalse(point.IsEnabled);
            Assert.ThrowsExactly<BussinesRuleException>(() => point.Disable());
            Assert.IsFalse(point.IsEnabled);

            point.Enable();

            Assert.IsTrue(point.IsEnabled);
            Assert.ThrowsExactly<BussinesRuleException>(() => point.Enable());
            Assert.IsTrue(point.IsEnabled);

            point.Disable();

            Assert.IsFalse(point.IsEnabled);
        }
    }
}
