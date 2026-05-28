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
        public static BurcatList<Faction> GetFactions(string? search) => [.. InterfaceExchange.DoFactionSearch(from f in InterfaceOptions.GetSingleUse<Faction>() select f, search)];
        public static BurcatList<Faction> GetFactionsFor(Member member, string? search)
        {
            Politeness tolerance = member.Tolerance is BurcatIdentifier<Politeness> tID ? InterfaceOptions.Find(tID) : API.Politeness.GetNewMaximum(member);

            return [.. InterfaceExchange.DoFactionSearch(
            from f in InterfaceOptions.GetSingleUse<Faction>()
            join p in InterfaceOptions.GetSingleUse<Politeness>() on (Guid)f.Tolerance equals p.Identifier
            join r in InterfaceOptions.GetSingleUse<FactionRole>() on f.Identifier equals (Guid)r.Faction
            join mr in InterfaceOptions.GetSingleUse<MemberFactionRole>() on new { Role = r.Identifier, Member = member.Identifier } equals new {Role = (Guid)mr.Role, Member = (Guid)mr.Member }
            where p.Language <= tolerance.Language && p.Racism <= tolerance.Racism && p.Sexuality <= tolerance.Sexuality && p.Violence <= tolerance.Violence
            select f
            , search)];
        }

        public BurcatIdentifier<Member> Owner { get; }
        [Length(8, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image>? Icon { get; set; }
        public BurcatIdentifier<Politeness> Tolerance { get; set; }
        public string? Description { get; set; }

        public Faction(BurcatIdentifier<Member> owner, string name, BurcatIdentifier<Image>? icon, BurcatIdentifier<Politeness> tolerance, string? description = null) { Owner = owner; Name = name; Icon = icon; Tolerance = tolerance; Description = description; }

        public SortedListSet<Member> GetMembers() => [..
            from r in InterfaceOptions.GetSingleUse<FactionRole>()
            join fr in InterfaceOptions.GetSingleUse<MemberFactionRole>() on r.Identifier equals (Guid)fr.Role into roles
            from fr in roles
            join a in InterfaceOptions.GetSingleUse<Member>() on (Guid)fr.Member equals a.Identifier
            where r.Faction == this select a];

        public SortedListSet<Member> GetBans() => [..
            from b in InterfaceOptions.GetSingleUse<FactionBan>()
            join a in InterfaceOptions.GetSingleUse<Member>() on (Guid)b.Member equals a.Identifier
            where b.Faction == this select a];

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (Owner == member || (from ar in InterfaceOptions.GetSingleUse<MemberFactionRole>() join fr in InterfaceOptions.GetSingleUse<FactionRole>() on (Guid)ar.Role equals fr.Identifier where ar.Member == member && fr.Faction == this && fr.CanManageGroup == true select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Icon, Tolerance, Description];
    }

    [BurcatIdentity("83b81c67-a184-4c6d-9656-0f22c306af9e")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    public class FactionBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }

        public FactionBan(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member) { Faction = faction; Member = member; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from ar in InterfaceOptions.GetSingleUse<MemberFactionRole>() join fr in InterfaceOptions.GetSingleUse<FactionRole>() on (Guid)ar.Role equals fr.Identifier where ar.Member == member && fr.Faction == Faction && fr.CanManageBans == true select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Faction, Member];
    }
}
