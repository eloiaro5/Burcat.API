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

        public SortedListSet<Member> GetMembersWithRole() => InterfaceOptions.UseProvider(provider => (SortedListSet<Member>)[..
            from fm in provider.Get<FactionMembership>()
            join m in provider.Get<Member>() on (Guid)fm.Member equals m.Identifier
            where fm.Role == this select m]);

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (InterfaceOptions.Find(Faction).Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.Level <= Level && r.CanManageRoles == true select true).Any()));

        public override object?[] GetBurcatConstructionValues() => [Faction, Level, Name];
    }
}
