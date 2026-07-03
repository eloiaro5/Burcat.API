using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API
{
    [BurcatIdentity("cd6b4144-c132-4945-a890-68307486a4d9")]
    [BurcatUnique(nameof(From), nameof(To))]
    public class MemberBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> From { get; }
        public BurcatIdentifier<Member> To { get; }

        public MemberBan(BurcatIdentifier<Member> from, BurcatIdentifier<Member> to) { From = from; To = to; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && From == member;

        public override object?[] GetBurcatConstructionValues() => [From, To];
    }
}
