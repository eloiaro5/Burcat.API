using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using BurcatProtocol.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API
{
    [BurcatIdentity("372d2430-436a-437d-a0f6-f34880edcf5a")]
    [BurcatUnique(nameof(Level))]
    [BurcatUnique(nameof(Name))]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateFactionRole))]
    public class FactionRole : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Level { get; }
        [Length(1, 32)]
        public string Name { get; set; }
        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        /// <summary>
        /// Identifies whether a member is able to create posts.
        /// </summary>
        public bool CanPost { get; set; }
        /// <summary>
        /// Identifies whether a member is able to remove posts.
        /// </summary>
        public bool CanManagePosts { get; set; }
        /// <summary>
        /// Identifies whether a member is able to remove or ban members.
        /// </summary>
        public bool CanManageUsers { get; set; }
        /// <summary>
        /// Identifies whether a member is able to change role settings and assign roles.
        /// </summary>
        public bool CanManageRoles { get; set; }
        /// <summary>
        /// Identifies whether a member is able to change faction's settings.
        /// </summary>
        public bool CanManageFaction { get; set; }
        /// <summary>
        /// Identifies whether this role is set when a member joins the faction.
        /// </summary>
        public bool InitialRole { get; set; }

        public FactionRole(BurcatIdentifier<Faction> faction, int level, string name) { Faction = faction; Level = level; Name = name; }

        [AtomicAction]
        public ListSet<Member> GetMembersWithRole() => (ListSet<Member>)[..
            from fm in InterfaceOptions.Provider.Get<FactionMembership>()
            join m in InterfaceOptions.Provider.Get<Member>() on (Guid)fm.Member equals m.Identifier
            where fm.Role == this select m];

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (InterfaceOptions.Find(Faction).Owner == member || (from m in InterfaceOptions.Provider.Get<FactionMembership>() join r in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.Level < Level && r.CanManageRoles == true select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Faction, Level, Name];
    }
}
