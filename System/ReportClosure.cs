using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Burcat.API.System
{
    [BurcatIdentity("e1e24664-95d2-4936-8a9d-a8d23cc3d441")]
    [BurcatUnique(nameof(Report), nameof(ClosedIn))]
    public class ReportClosure : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IReport> Report { get; }
        public BurcatIdentifier<Member> ClosedBy { get; }
        public string Motive { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime ClosedIn { get; }

        public ReportClosure(BurcatIdentifier<IReport> report, BurcatIdentifier<Member> closedBy, string motive, DateTime? closedIn) { Report = report; ClosedBy = closedBy; Motive = motive; ClosedIn = closedIn ?? DateTime.Now; }

        public SortedListSet<CountryBan> GetBans() => [.. from b in InterfaceOptions.GetSingleUse<CountryBan>() where b.Closure == this select b];

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Moderator) == DevelopStatusType.Moderator;
        public override object?[] GetBurcatConstructionValues() => [Report, ClosedBy, Motive, ClosedIn];
    }

    [BurcatIdentity("63d0048a-8bac-410a-8cd2-5d0c82741e35")]
    [BurcatUnique(nameof(Closure), nameof(BannedIn))]
    public class CountryBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<ReportClosure> Closure { get; }
        public BurcatIdentifier<Country> BannedIn { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime InEffectSince { get; }

        public CountryBan(BurcatIdentifier<ReportClosure> closure, BurcatIdentifier<Country> bannedIn, DateTime? inEffectSince) { Closure = closure; BannedIn = bannedIn; InEffectSince = inEffectSince ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Moderator) == DevelopStatusType.Moderator;
        public override object?[] GetBurcatConstructionValues() => [Closure, BannedIn, InEffectSince];
    }
}
