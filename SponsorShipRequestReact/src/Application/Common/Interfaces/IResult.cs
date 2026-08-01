using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace TechAssessment.Application.Common.Interfaces;

public interface IResult
{
    ResultStatus Status { get; }

    ObservableCollection<string> Errors { get; }

    ObservableCollection<ValidationError> ValidationErrors { get; }

    Type ValueType { get; }

    object GetValue();
}
