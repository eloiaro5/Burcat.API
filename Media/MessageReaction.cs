using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("2ba6721c-c62f-40e7-a911-bdadcafe27ff")]
    [BurcatUnique(nameof(Member), nameof(Message))]
    public class MessageReaction : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<IMessage> Message { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePostReactionReaction))]
        public string Reaction { get; set; }

        public MessageReaction(BurcatIdentifier<Member> member, BurcatIdentifier<IMessage> message, string reaction) { Member = member; Message = message; Reaction = reaction; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Member == member;

        public override object?[] GetBurcatConstructionValues() => [Member, Message, Reaction];
    }
}
