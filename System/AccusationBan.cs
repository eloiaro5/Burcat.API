using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.System
{
    [BurcatIdentity("63d0048a-8bac-410a-8cd2-5d0c82741e35")]
    [BurcatUnique(nameof(Closure), nameof(BannedIn))]
    public class AccusationBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<AccusationClosure> Closure { get; }
        public BurcatIdentifier<Country> BannedIn { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime InEffectSince { get; }

        public AccusationBan(BurcatIdentifier<AccusationClosure> closure, BurcatIdentifier<Country> bannedIn, DateTime? inEffectSince) { Closure = closure; BannedIn = bannedIn; InEffectSince = inEffectSince ?? DateTime.Now; }

        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevOps.GetGrade(memberID) is DevOpsGrade type && (type & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        public override object?[] GetBurcatConstructionValues() => [Closure, BannedIn, InEffectSince];
    }
}
