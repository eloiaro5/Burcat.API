using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.System
{
    [BurcatUnique(nameof(Accused), nameof(Reporter))]
    public interface IReport : IInterfaceObject
    {
        /// <summary>The reported message, faction, member, or application. The property name is retained for existing stored reports.</summary>
        BurcatIdentifier<IReport.IReportable> Accused { get; }
        BurcatIdentifier<Member> Reporter { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrOverNow))]
        DateTime SetIn { get; }

        public interface IReportable : IInterfaceObject { }
    }
}
