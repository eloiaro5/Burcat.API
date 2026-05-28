using Burcat.API.Development;
using Burcat.API.Market;
using Burcat.API.Market.Passes;
using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        public static ValidationResult? ValidateMissionDependency(MissionDependency dependency)
        {
            //if (dependency.Mission.Equals(dependency.DependsOn)) return new("A mission cannot depend on itself.");
            //else if (!dependency.Mission.Pass.Equals(dependency.DependsOn.Pass)) return new("A mission cannot depend on a mission of another pass.");
            //else return ValidationResult.Success;
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateMissionCompletion(MissionCompletion completion)
        {
            //if (!completion.Mission.Pass.Equals(completion.Entry.Pass)) return new("A mission completion needs to have both the mission and the entry be about the same pass.");
            //else return ValidationResult.Success;
            return ValidationResult.Success;
        }
    }
}
