using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API
{
    [BurcatIdentity("e4d6606a-59c7-444d-8742-ed6b20230cc6")]
    [BurcatUnique(nameof(From), nameof(To))]
    public class MemberFollow : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> From { get; }
        public BurcatIdentifier<Member> To { get; }

        public MemberFollow(BurcatIdentifier<Member> from, BurcatIdentifier<Member> to) { From = from; To = to; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && From == member;

        public override object?[] GetBurcatConstructionValues() => [From, To];
    }
}
