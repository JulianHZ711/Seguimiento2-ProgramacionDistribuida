using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Utilities.Results
{
    public sealed record Error(string Code, string Description, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    }
}
