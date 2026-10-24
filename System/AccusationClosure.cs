using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using BurcatProtocol.Transactions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Burcat.API.System
{
    [BurcatIdentity("e1e24664-95d2-4936-8a9d-a8d23cc3d441")]
    [BurcatUnique(nameof(Report), nameof(ClosedIn))]
    public class AccusationClosure : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IReport> Report { get; }
        public BurcatIdentifier<Member> ClosedBy { get; }
        public string Motive { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime ClosedIn { get; }

        public AccusationClosure(BurcatIdentifier<IReport> report, BurcatIdentifier<Member> closedBy, string motive, DateTime? closedIn) { Report = report; ClosedBy = closedBy; Motive = motive; ClosedIn = closedIn ?? DateTime.Now; }

        [AtomicAction]
        public SortedListSet<AccusationBan> GetBans() => (SortedListSet<AccusationBan>)[.. from b in InterfaceOptions.Provider.Get<AccusationBan>() where b.Closure == this select b];

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ClosedBy == member && ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member);

        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevOps.GetGrade(memberID) is DevOpsGrade type && (type & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        public override object?[] GetBurcatConstructionValues() => [Report, ClosedBy, Motive, ClosedIn];
    }
}
