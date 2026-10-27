using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Utilities.Results
{
    public enum ErrorType
    {
        None = 0,
        Validation = 1,
        Business = 2,
        Unexpected = 3,
        NotFound = 4,
    }
}
