using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.Common;

public enum ResultStatus
{
    Ok,
    Error,
    Forbidden,
    Unauthorized,
    Invalid,
    NotFound,
    Conflict,
    CriticalError,
    Unavailable
}
