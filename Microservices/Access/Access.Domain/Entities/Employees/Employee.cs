using System;
using System.Collections.Generic;
using System.Text;
using Access.Domain.Entities.Areas;
using Access.Domain.Entities.Employees.ValueObjects;
using Access.Domain.Exceptions;

namespace Access.Domain.Entities.Employees
{
    public sealed class Employee
    {
        public const int NameMinLength = 2;
        public const int NameMaxLength = 100;
        public const int PositionMinLength = 2;
        public const int PositionMaxLength = 100;

        public Guid Id { get; private set; }
        public DocumentNumber DocumentNumber { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public string Position { get; private set; } = null!;
        public Guid AreaId { get; private set; }
        public Area Area { get; private set; } = null!;
        public EmploymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

        public bool IsActive => Status == EmploymentStatus.Active;

        private Employee()
        {

        }

        public Employee(DocumentNumber documentNumber,
                        string firstName,
                        string lastName,
                        Email email,
                        string position,
                        Guid areaId)
        {
            ApplyDocumentNumberRules(documentNumber);
            ApplyFirstNameRules(firstName);
            ApplyLastNameRules(lastName);
            ApplyEmailRules(email);
            ApplyPositionRules(position);
            ApplyAreaIdRules(areaId);

            Id = Guid.CreateVersion7();
            DocumentNumber = documentNumber;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email;
            Position = position.Trim();
            AreaId = areaId;
            Status = EmploymentStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        // La identificación no se actualiza: identifica al empleado de forma única.
        public void Update(string firstName,
                           string lastName,
                           Email email,
                           string position,
                           Guid areaId)
        {
            ApplyActiveRules();
            ApplyFirstNameRules(firstName);
            ApplyLastNameRules(lastName);
            ApplyEmailRules(email);
            ApplyPositionRules(position);
            ApplyAreaIdRules(areaId);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email;
            Position = position.Trim();
            AreaId = areaId;
        }

        public void Deactivate()
        {
            if (Status == EmploymentStatus.Inactive)
            {
                throw new BussinesRuleException("El empleado ya se encuentra inactivo.");
            }

            Status = EmploymentStatus.Inactive;
            DeactivatedAt = DateTime.UtcNow;
        }

        private void ApplyActiveRules()
        {
            if (Status == EmploymentStatus.Inactive)
            {
                throw new BussinesRuleException("No se puede modificar un empleado inactivo.");
            }
        }

        private static void ApplyDocumentNumberRules(DocumentNumber documentNumber)
        {
            if (documentNumber is null)
            {
                throw new BussinesRuleException("El número de identificación es requerido.");
            }
        }

        private static void ApplyFirstNameRules(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new BussinesRuleException("Los nombres del empleado son requeridos.");
            }

            if (firstName.Trim().Length < NameMinLength || firstName.Trim().Length > NameMaxLength)
            {
                throw new BussinesRuleException($"Los nombres del empleado deben tener entre {NameMinLength} y {NameMaxLength} caracteres.");
            }
        }

        private static void ApplyLastNameRules(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new BussinesRuleException("Los apellidos del empleado son requeridos.");
            }

            if (lastName.Trim().Length < NameMinLength || lastName.Trim().Length > NameMaxLength)
            {
                throw new BussinesRuleException($"Los apellidos del empleado deben tener entre {NameMinLength} y {NameMaxLength} caracteres.");
            }
        }

        private static void ApplyEmailRules(Email email)
        {
            if (email is null)
            {
                throw new BussinesRuleException("El correo electrónico es requerido.");
            }
        }

        private static void ApplyPositionRules(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                throw new BussinesRuleException("El cargo del empleado es requerido.");
            }

            if (position.Trim().Length < PositionMinLength || position.Trim().Length > PositionMaxLength)
            {
                throw new BussinesRuleException($"El cargo del empleado debe tener entre {PositionMinLength} y {PositionMaxLength} caracteres.");
            }
        }

        private static void ApplyAreaIdRules(Guid areaId)
        {
            if (areaId == Guid.Empty)
            {
                throw new BussinesRuleException("El área del empleado es requerida.");
            }
        }
    }
}
