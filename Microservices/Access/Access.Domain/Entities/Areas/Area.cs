using System;
using System.Collections.Generic;
using System.Text;
using Access.Domain.Entities.Employees;
using Access.Domain.Exceptions;

namespace Access.Domain.Entities.Areas
{
    public sealed class Area
    {
        public const int NameMinLength = 2;
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;

        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public ICollection<Employee> Employees { get; private set; } = new List<Employee>();

        private Area()
        {

        }

        public Area(string name, string? description = null)
        {
            ApplyNameRules(name);
            ApplyDescriptionRules(description);

            Id = Guid.CreateVersion7();
            Name = name.Trim();
            Description = description?.Trim();
        }

        public void Update(string name, string? description = null)
        {
            ApplyNameRules(name);
            ApplyDescriptionRules(description);

            Name = name.Trim();
            Description = description?.Trim();
        }

        private static void ApplyNameRules(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BussinesRuleException("El nombre del área es requerido.");
            }

            if (name.Trim().Length < NameMinLength || name.Trim().Length > NameMaxLength)
            {
                throw new BussinesRuleException($"El nombre del área debe tener entre {NameMinLength} y {NameMaxLength} caracteres.");
            }
        }

        private static void ApplyDescriptionRules(string? description)
        {
            if (description is not null && description.Trim().Length > DescriptionMaxLength)
            {
                throw new BussinesRuleException($"La descripción del área debe tener máximo {DescriptionMaxLength} caracteres.");
            }
        }
    }
}
