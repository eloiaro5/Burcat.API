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
        public static ValidationResult? ValidatePolitenessReport(PolitenessReport report) => InterfaceOptions.UseProvider(provider =>
            (from pt in provider.Get<IPost>() join p in provider.Get<Politeness>() on (Guid)pt.Politeness equals p.Identifier where (Guid)report.Post == pt.Identifier select p).First()
            >=
            (from p in provider.Get<Politeness>() where p == report.ExpectedPoliteness select p).First()
            ? new("The expected politeness must be under the post politeness.") : ValidationResult.Success);
    }
}
