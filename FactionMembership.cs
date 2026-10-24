using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Transactions;

namespace Burcat.API
{
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

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is not null && Member == member && Role is null && !(from b in InterfaceOptions.Provider.Get<FactionBan>() where b.Faction == Faction && b.Member == Member select true).Any();
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
}
