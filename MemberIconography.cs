using BurcatProtocol;
using BurcatProtocol.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API
{
    [BurcatIdentity("a744e0b1-fe53-4e46-9f6e-18b9b109a9a1")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateIconography))]
    public class MemberIconography : BurcatObject, IInterfaceObject, IIconography
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Image> Icon { get; }
        [Length(1, 32)]
        public string Name { get; set; }

        BurcatIdentifier<IProfile> IIconography.Owner => Owner.Downcast<IProfile>();

        public MemberIconography(BurcatIdentifier<Member> owner, BurcatIdentifier<Image> icon, [Length(1, 32)] string name) { Owner = owner; Icon = icon; Name = name; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Icon, Name];
    }
}
