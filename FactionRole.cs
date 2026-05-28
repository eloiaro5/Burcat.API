using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API
{
    [BurcatIdentity("372d2430-436a-437d-a0f6-f34880edcf5a")]
    [BurcatUnique(nameof(Name))]
    public class FactionRole : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Level { get; }
        [Length(1, 32)]
        public string Name { get; }
        public bool CanPost { get; set; }
        public bool CanManagePosts { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanManageRoles { get; set; }
        public bool CanManageGroup { get; set; }
        public bool CanManageBans { get; set; }

        public FactionRole(BurcatIdentifier<Faction> faction, int level, string name) { Faction = faction; Level = level; Name = name; }

        public SortedListSet<Member> GetMembersWithRole() => [..
            from r in InterfaceOptions.GetSingleUse<MemberFactionRole>()
            join a in InterfaceOptions.GetSingleUse<Member>() on (Guid)r.Member equals a.Identifier
            where r.Role == this select a];

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from ar in InterfaceOptions.GetSingleUse<MemberFactionRole>() join fr in InterfaceOptions.GetSingleUse<FactionRole>() on (Guid)ar.Role equals fr.Identifier where ar.Member == member && fr.Faction == Faction && fr.Level <= Level && fr.CanManageRoles == true select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Faction, Level, Name];
    }

    [BurcatIdentity("f9ceb1d2-7439-4498-95f3-4758d9902cc6")]
    [BurcatUnique(nameof(Role), nameof(Member))]
    public class MemberFactionRole : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<FactionRole> Role { get; }

        public MemberFactionRole(BurcatIdentifier<Member> member, BurcatIdentifier<FactionRole> role) { Member = member; Role = role; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => true;//member is not null && (from ar in InterfaceOptions.Get<MemberFactionRole>() join fr in InterfaceOptions.Get<FactionRole>() on (Guid)ar.Role equals fr.Identifier where ar.Member == member.Identifier && ar.Faction == Faction && fr.Level <= Level && fr.CanManageRoles == true select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Member, Role];
    }
}
