using Burcat.API.Market.Passes;
using Burcat.API.Media;
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

        public SortedListSet<Member> GetMembers() => InterfaceOptions.UseProvider(provider => (SortedListSet<Member>)[..
            (from m in provider.Get<FactionMembership>()
            join a in provider.Get<Member>() on (Guid)m.Member equals a.Identifier
            where m.Faction == this select a).AsEnumerable().Concat([InterfaceOptions.Find(Owner)])]);

        public SortedListSet<Member> GetBans() => InterfaceOptions.UseProvider(provider => (SortedListSet<Member>)[..
            from b in provider.Get<FactionBan>()
            join a in provider.Get<Member>() on (Guid)b.Member equals a.Identifier
            where b.Faction == this select a]);

        public FactionMembership? GetMembership(BurcatIdentifier<Member> member) => InterfaceOptions.UseProvider(provider =>
            provider.Get<FactionMembership>().FirstOrDefault(m => m.Faction == this && m.Member == member));

        public bool IsMember(BurcatIdentifier<Member> member) => Owner == member || GetMembership(member) is not null;

        public BurcatException? Join(BurcatIdentifier<Member> member) => InterfaceOptions.UseProvider(provider =>
        {
            if (IsMember(member)) return new("The member has already joined this faction.");
            else if (provider.Get<FactionBan>().Any(b => b.Faction == this && b.Member == member)) return new ("The member is banned from this faction.");
            else
            {
                FactionRole? initialRole = provider.Get<FactionRole>().FirstOrDefault(r => r.Faction == this && r.InitialRole);
                BurcatIdentifier<FactionRole>? initialRoleIdentifier = initialRole is null ? null : new(initialRole.Identifier);
                return (BurcatException?)null;//return BurcatChat.RelayCouple(new FactionMembership(this, member, initialRoleIdentifier));
            }
        });

        public BurcatException? Leave(BurcatIdentifier<Member> member)
        {
            if (Owner == member) return new("The faction owner cannot leave the faction.");
            else if (GetMembership(member) is not FactionMembership membership) return new("The member has not joined this faction.");
            else return (BurcatException?)null;//return BurcatChat.RelayDecouple(membership);
        }

        public bool CanManageRoles(BurcatIdentifier<Member> member) => Owner == member || InterfaceOptions.UseProvider(provider =>
            (from membership in provider.Get<FactionMembership>()
             join role in provider.Get<FactionRole>() on (Guid?)membership.Role equals role.Identifier
             where membership.Member == member && role.Faction == this && role.CanManageRoles
             select true).Any());

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == this && r.CanManageGroup == true select true).Any()));

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Icon, Description];
    }

    [BurcatIdentity("33e00867-4213-4102-94b6-916d8c940992")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    public class FactionMembership : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<FactionRole>? Role { get; set; }

        public FactionMembership(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member, BurcatIdentifier<FactionRole>? role = null) { Faction = faction; Member = member; Role = role; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Member == member && !InterfaceOptions.UseProvider(provider => (from b in provider.Get<FactionBan>() where b.Faction == Faction && b.Member == Member select true).Any());
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Member == member || InterfaceOptions.Find(Faction).Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageUsers == true select true).Any()));

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
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (InterfaceOptions.Find(Faction).Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageBans == true select true).Any()));

        public override object?[] GetBurcatConstructionValues() => [Faction, Member];
    }
}
