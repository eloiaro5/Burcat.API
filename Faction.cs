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
        public static BurcatList<Faction> GetFactions(string? search) => InterfaceOptions.UseProvider(provider =>
        {
            IQueryable<Faction> factions = from f in provider.Get<Faction>() select f;
            IEnumerable<string> searchValues = GetSearchValues(search);
            if (searchValues.Any()) factions = factions.Where(f => searchValues.Any(v => f.Name.Contains(v) || (f.Description != null && f.Description.Contains(v))));
            return (BurcatList<Faction>)[.. factions.Take(100)];
        });

        public static BurcatList<Faction> GetFactionsFor(Member member, string? search)
        {
            Politeness tolerance = member.Tolerance is BurcatIdentifier<Politeness> tID ? InterfaceOptions.Find(tID) : API.Politeness.GetNewMaximum(member);

            return InterfaceOptions.UseProvider(provider =>
            {
                IQueryable<Faction> factions =
                from f in provider.Get<Faction>()
                join p in provider.Get<Politeness>() on (Guid)f.Tolerance equals p.Identifier
                join r in provider.Get<FactionRole>() on f.Identifier equals (Guid)r.Faction
                join m in provider.Get<FactionMembership>() on new { Role = (Guid?)r.Identifier, Member = member.Identifier } equals new { Role = (Guid?)m.Role, Member = (Guid)m.Member }
                where p.Language <= tolerance.Language && p.Racism <= tolerance.Racism && p.Sexuality <= tolerance.Sexuality && p.Violence <= tolerance.Violence
                select f;

                IEnumerable<string> searchValues = GetSearchValues(search);
                if (searchValues.Any()) factions = factions.Where(f => searchValues.Any(v => f.Name.Contains(v) || (f.Description != null && f.Description.Contains(v))));
                return (BurcatList<Faction>)[.. factions.Take(100)];
            });
        }

        private static IEnumerable<string> GetSearchValues(string? search) => search?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct() ?? [];

        public BurcatIdentifier<Member> Owner { get; }
        [Length(8, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image>? Icon { get; set; }
        public BurcatIdentifier<Politeness> Tolerance { get; set; }
        public string? Description { get; set; }

        public Faction(BurcatIdentifier<Member> owner, string name, BurcatIdentifier<Image>? icon, BurcatIdentifier<Politeness> tolerance, string? description = null) { Owner = owner; Name = name; Icon = icon; Tolerance = tolerance; Description = description; }

        public SortedListSet<Member> GetMembers() => InterfaceOptions.UseProvider(provider => (SortedListSet<Member>)[..
            (from m in provider.Get<FactionMembership>()
            join a in provider.Get<Member>() on (Guid)m.Member equals a.Identifier
            where m.Faction == this select a).AsEnumerable().Concat([InterfaceOptions.Find(Owner)])]);

        public SortedListSet<Member> GetBans() => InterfaceOptions.UseProvider(provider => (SortedListSet<Member>)[..
            from b in provider.Get<FactionBan>()
            join a in provider.Get<Member>() on (Guid)b.Member equals a.Identifier
            where b.Faction == this select a]);

        bool IInterfaceObject.ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == this && r.CanManageGroup == true select true).Any()));

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Icon, Tolerance, Description];
    }

    [BurcatIdentity("33e00867-4213-4102-94b6-916d8c940992")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    public class FactionMembership : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<FactionRole>? Role { get; set; }

        public FactionMembership(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member, BurcatIdentifier<FactionRole>? role = null) { Faction = faction; Member = member; Role = role; }

        bool IInterfaceObject.ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Member == member && !InterfaceOptions.UseProvider(provider => (from b in provider.Get<FactionBan>() where b.Faction == Faction && b.Member == Member select true).Any());
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Member == member || InterfaceOptions.Find(Faction).Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageUsers == true select true).Any()));

        public override object?[] GetBurcatConstructionValues() => [Faction, Member, Role];
    }

    [BurcatIdentity("83b81c67-a184-4c6d-9656-0f22c306af9e")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    public class FactionBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }

        public FactionBan(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member) { Faction = faction; Member = member; }

        bool IInterfaceObject.ShouldCreate(BurcatIdentifier<Member>? member) => ((IInterfaceObject)this).ShouldManage(member);
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (InterfaceOptions.Find(Faction).Owner == member || InterfaceOptions.UseProvider(provider => (from m in provider.Get<FactionMembership>() join r in provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageBans == true select true).Any()));

        public override object?[] GetBurcatConstructionValues() => [Faction, Member];
    }
}
