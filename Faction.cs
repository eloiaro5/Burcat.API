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
    public class Faction : BurcatObject, IInterfaceObject
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

        [AtomicAction]
        public bool CanManageRoles(BurcatIdentifier<Member> member) => Owner == member || (from membership in InterfaceOptions.Provider.Get<FactionMembership>()
             join role in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)membership.Role equals role.Identifier
             where membership.Member == member && role.Faction == this && role.CanManageRoles
             select true).Any();

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Owner == member || (from m in InterfaceOptions.Provider.Get<FactionMembership>() join r in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == this && r.CanManageFaction == true select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Icon, Description];
    }

    [BurcatIdentity("33e00867-4213-4102-94b6-916d8c940992")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateFactionRoleAssignment))]
    public class FactionMembership : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<FactionRole>? Role { get; set; }

        public FactionMembership(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member, BurcatIdentifier<FactionRole>? role = null) { Faction = faction; Member = member; Role = role; }

        /// <summary>Returns whether the action is allowed or this membership's member owns the faction.</summary>
        /// <remarks>The faction is only looked up when <paramref name="action"/> is false.</remarks>
        [AtomicAction]
        public bool CanPerformAction(bool action) => action || InterfaceOptions.Find(Faction).Owner == Member;

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Member == member && !(from b in InterfaceOptions.Provider.Get<FactionBan>() where b.Faction == Faction && b.Member == Member select true).Any();
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => member is not null && (Member == member || InterfaceOptions.Find(Faction).Owner == member || (from m in InterfaceOptions.Provider.Get<FactionMembership>() join r in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageUsers == true select true).Any());
        public bool ShouldManage(BurcatIdentifier<Member>? member)
        {
            if (member is null) return false;

            Faction faction = InterfaceOptions.Find(Faction);
            if (faction.Owner == member) return true;

            FactionMembership? manager = faction.GetMembership(member.Value);
            FactionRole? managerRole = manager?.Role is BurcatIdentifier<FactionRole> managerRoleID ? InterfaceOptions.TryFind(managerRoleID) : null;
            if (managerRole is null || managerRole.Faction != Faction) return false;
            if (!managerRole.CanManageRoles || faction.Owner == Member) return false;

            FactionMembership? current = InterfaceOptions.TryFind<FactionMembership>(this);
            if (current is null || current.Role == Role) return false;

            if (Role is BurcatIdentifier<FactionRole> destinationID)
            {
                FactionRole? destination = InterfaceOptions.TryFind(destinationID);
                if (destination is null || destination.Faction != Faction || destination.Level <= managerRole.Level) return false;
            }
            if (current.Role is not BurcatIdentifier<FactionRole> currentRoleID) return true;

            FactionRole? currentRole = InterfaceOptions.TryFind(currentRoleID);
            return currentRole is not null && currentRole.Faction == Faction && currentRole.Level > managerRole.Level;
        }

        public override object?[] GetBurcatConstructionValues() => [Faction, Member, Role];
    }

    [BurcatIdentity("83b81c67-a184-4c6d-9656-0f22c306af9e")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    public class FactionBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }

        public FactionBan(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member) { Faction = faction; Member = member; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member);
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (InterfaceOptions.Find(Faction).Owner == member || (from m in InterfaceOptions.Provider.Get<FactionMembership>() join r in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageUsers == true select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Faction, Member];
    }
}
