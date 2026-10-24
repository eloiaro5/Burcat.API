using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Transactions;

namespace Burcat.API.Media
{
    [BurcatIdentity("b0076bd1-c2b1-41f5-91ab-225e450ba3d0")]
    public class PostMessage : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public BurcatIdentifier<Faction> Faction { get; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        public PostMessage(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, string? tags = null, DateTime postDate = default) { Owner = owner; Politeness = politeness; Faction = faction ; Title = title; Message = message; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

        /// <inheritdoc />
        [AtomicAction]
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessageDeletion))]
        public void Delete()
        {
            Title = " ";
            Message = " ";
            Tags = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> actor && Owner == actor && InterfaceOptions.Find(Faction).CanPost(actor);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => false;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Faction, Title, Message, Tags, PostDate];
    }
}
