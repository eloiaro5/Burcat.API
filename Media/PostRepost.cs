using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Transactions;

namespace Burcat.API.Media
{
    [BurcatIdentity("f835b971-a090-44fd-a5e7-5e3c8755cec9")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateRepostPost))]
    public class PostRepost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public BurcatIdentifier<Faction> Faction { get; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public BurcatIdentifier<IPost> Post { get; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        public PostRepost(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, BurcatIdentifier<IPost> post, string? tags = null, DateTime postDate = default) { Owner = owner; Politeness = politeness; Faction = faction; Title = title; Message = message; Post = post; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

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

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Faction, Title, Message, Post, Tags, PostDate];
    }
}
