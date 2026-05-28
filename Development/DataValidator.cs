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
        public static ValidationResult? ValidatePermissionGrantPermission(PermissionGrant grant)
        {
            //if (!grant.Application.Requirements.Customizable && grant.Granted != grant.Application.Requirements) return new("Cannot grant diferent permisions to an application that doesn't allow its requirements to be customized.");
            //else return ValidationResult.Success;
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateApplicationCollaboration(ApplicationCollaboration collaboration)
        {
            //if (collaboration.With.Equals(collaboration.Application.Creator)) return new("Cannot create an application collaboration with the application owner.");
            //else return ValidationResult.Success;
            return ValidationResult.Success;
        }
    }
}
