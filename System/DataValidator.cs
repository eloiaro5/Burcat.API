﻿using Burcat.API.Development;
using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        public static ValidationResult? ValidatePolitenessReport(PolitenessReport report)
        {
            IMessage message = InterfaceOptions.Find(report.Message);

            if (message.Owner == report.Reporter) return new("You cannot report your own message.");
            else if (InterfaceOptions.Find(message.Politeness) >= InterfaceOptions.Find(report.ExpectedPoliteness)) return new("The expected politeness must be under the post politeness.");
            else return ValidationResult.Success;
        }
        public static ValidationResult? ValidateCopyrightReport(CopyrightReport report)
        {
            IMessage message = InterfaceOptions.Find(report.Message);

            if (message.Owner == report.Reporter) return new("You cannot report your own message.");
            else return ValidationResult.Success;
        }
    }
}
