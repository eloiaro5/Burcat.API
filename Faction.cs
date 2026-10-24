using Burcat.API.Market.Passes;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using BurcatProtocol.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API
{
    [BurcatIdentity("5065983a-333d-43a8-81fb-88fe068c6918")]
    [BurcatUnique(nameof(Name))]
    public class Faction : BurcatObject, IInterfaceObject, Burcat.API.System.IReport.IReportable
    {
        public BurcatIdentifier<Member> Owner { get; }
        [Length(8, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image>? Icon { get; set; }
        public string? Description { get; set; }

        public Faction(BurcatIdentifier<Member> owner, string name, BurcatIdentifier<Image>? icon, string? description = null) { Owner = owner; Name = name; Icon = icon; Description = description; }

        [AtomicAction]
        public SortedListSet<Member> GetMembers() => (SortedListSet<Member>)[..
            (from m in InterfaceOptions.Provider.Get<FactionMembership>()
            join a in InterfaceOptions.Provider.Get<Member>() on (Guid)m.Member equals a.Identifier
            where m.Faction == this select a).AsEnumerable().Concat([InterfaceOptions.Find(Owner)])];

        [AtomicAction]
        public SortedListSet<Member> GetBans() => (SortedListSet<Member>)[..
            from b in InterfaceOptions.Provider.Get<FactionBan>()
            join a in InterfaceOptions.Provider.Get<Member>() on (Guid)b.Member equals a.Identifier
            where b.Faction == this select a];

        [AtomicAction]
        public FactionMembership? GetMembership(BurcatIdentifier<Member> member) => InterfaceOptions.Provider.Get<FactionMembership>().FirstOrDefault(m => m.Faction == this && m.Member == member);

        [AtomicAction]
        public bool IsMember(BurcatIdentifier<Member> member) => Owner == member || GetMembership(member) is not null;

        /// <summary>Determines whether a member owns the faction or has an assigned role in it and may create posts.</summary>
        [AtomicAction]
        public bool CanPost(BurcatIdentifier<Member> member) => Owner == member || (from membership in InterfaceOptions.Provider.Get<FactionMembership>()
             join role in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)membership.Role equals role.Identifier
             where membership.Member == member && membership.Faction == this && role.Faction == this
             select true).Any();

        [AtomicAction]
        public bool CanManageRoles(BurcatIdentifier<Member> member) => Owner == member || (from membership in InterfaceOptions.Provider.Get<FactionMembership>()
             join role in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)membership.Role equals role.Identifier
             where membership.Member == member && role.Faction == this && role.CanManageRoles
             select true).Any();

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => false;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Owner == member || (from m in InterfaceOptions.Provider.Get<FactionMembership>() join r in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == this && r.CanManageFaction == true select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Icon, Description];
    }
}
