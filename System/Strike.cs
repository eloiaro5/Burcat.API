using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API.System
{
    [BurcatIdentity("f458be78-37eb-4c0c-8e81-71278fd3d59e")]
    public enum StrikeSeverity : int
    {
        Low,
        Medium,
        High
    }

    [BurcatIdentity("9fa16afe-7f4c-47dc-81b0-84121e47906d")]
    [BurcatUnique(nameof(ResponseTo))]
    public class Strike : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<DevelopStatus> SetBy { get; }
        public BurcatIdentifier<Member> SetTo { get; }
        public StrikeSeverity Severity { get; }
        public string Reason { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrOverNow))]
        public DateTime SetIn { get; }
        public BurcatIdentifier<IReport>? ResponseTo { get; }

        public Strike(BurcatIdentifier<DevelopStatus> setBy, BurcatIdentifier<Member> setTo, StrikeSeverity severity, string reason, DateTime setIn, BurcatIdentifier<IReport>? responseTo = null) { SetBy = setBy; SetTo = setTo; Severity = severity; Reason = reason; SetIn = setIn; ResponseTo = responseTo; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Moderator) == DevelopStatusType.Moderator;
        public override object?[] GetBurcatConstructionValues() => [SetBy, SetTo, Severity, Reason, SetIn];
    }
}
