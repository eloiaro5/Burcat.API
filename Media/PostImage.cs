using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Transactions;

namespace Burcat.API.Media
{
    [BurcatIdentity("ba3bfef2-9340-4d7b-a3e3-eb36021fb26d")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateImagePost))]
    public class PostImage : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public BurcatIdentifier<Faction> Faction { get; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public BurcatIdentifier<Image>? Image { get; private set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        /// <summary>Reconstructs an image post, allowing a cleared image reference after deletion.</summary>
        public PostImage(BurcatIdentifier<Member> owner, BurcatIdentifier<Image>? image, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, string? tags = null, DateTime postDate = default) { Owner = owner; Politeness = politeness; Faction = faction; Title = title; Message = message; Image = image; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

        /// <inheritdoc />
        [AtomicAction]
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessageDeletion))]
        public void Delete()
        {
            Title = " ";
            Message = " ";
            Tags = null;
            Image = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> actor && Owner == actor && InterfaceOptions.Find(Faction).CanPost(actor);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => false;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Image, Politeness, Faction, Title, Message, Tags, PostDate];
    }
}
