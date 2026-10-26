using System;
using System.Collections.Generic;
using System.Text;
using Access.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Access.Domain.Entities.Employees.ValueObjects
{
    public sealed record DocumentNumber
    {
        public const int MinLength = 5;
        public const int MaxLength = 20;

        // Solo letras y números: cédula (numérica), cédula de extranjería o pasaporte (alfanuméricos).
        private static readonly Regex DocumentRegex = new(@"^[A-Za-z0-9]+$", RegexOptions.Compiled);

        public string Value { get; private set; } = null!;

        private DocumentNumber()
        {

        }

        public DocumentNumber(string value)
        {
            ApplyValueRules(value);

            Value = value.Trim().ToUpperInvariant();
        }

        private static void ApplyValueRules(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BussinesRuleException("El número de identificación es requerido.");
            }

            string trimmedValue = value.Trim();

            if (trimmedValue.Length < MinLength || trimmedValue.Length > MaxLength)
            {
                throw new BussinesRuleException($"El número de identificación debe tener entre {MinLength} y {MaxLength} caracteres.");
            }

            if (!DocumentRegex.IsMatch(trimmedValue))
            {
                throw new BussinesRuleException("El número de identificación solo puede contener letras y números, sin puntos ni espacios.");
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
