using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("e53065c7-8839-4844-b32e-aa747c5da38c")]
    [BurcatUnique(nameof(Message))]
    public class MessageDeletion : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Requester { get; }
        public BurcatIdentifier<IMessage> Message { get; }
        public DateTime DeletionDate { get; }

        public MessageDeletion(BurcatIdentifier<Member> requester, BurcatIdentifier<IMessage> message) { Requester = requester; Message = message; DeletionDate = DateTime.Now; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member)
        {
            if (member is null || Requester != member) return false;
            IMessage? message = InterfaceOptions.TryFindMessage(Message.Value);
            if (message is null) return false;
            if (Burcat.API.Development.DevOps.HasGrade(member, Burcat.API.Development.DevOpsGrade.Moderator)) return true;
            if (message.Owner == Requester) return true;
            if (message is not IPost post) return false;
            if (InterfaceOptions.Find(post.Faction).Owner == Requester) return true;

            return (from membership in InterfaceOptions.Provider.Get<FactionMembership>()
                 join role in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)membership.Role equals role.Identifier
                 where membership.Member == member && membership.Faction == post.Faction && role.Faction == post.Faction && role.CanManagePosts
                 select true).Any();
        }

        public override object?[] GetBurcatConstructionValues() => [Requester, Message];
    }
}
