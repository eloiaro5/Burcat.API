using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Burcat.API.Media
{
    [BurcatIdentity("e99b127a-871a-4ede-978b-9166551610d9")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateAdvertisementPost))]
    [BurcatUnique(nameof(Post))]
    public class AdvertisementPost : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IPost> Post { get; }
        public decimal Payment { get; set; }
        public BurcatIdentifier<Politeness>? MaximumPoliteness { get; set; }

        public AdvertisementPost(BurcatIdentifier<IPost> post, decimal payment, BurcatIdentifier<Politeness>? maximumPoliteness = null) { Post = post; Payment = payment; MaximumPoliteness = maximumPoliteness; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from p in InterfaceOptions.GetSingleUse<IPost>() where p.Identifier == (Guid)Post.Value && p.Owner == member select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, Payment];
    }

    [BurcatIdentity("bf9c2227-b561-4947-8c78-228ec597d12b")]
    [BurcatUnique(nameof(Post), nameof(Image))]
    public class MessagePostImage : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<MessagePost> Post { get; }
        public BurcatIdentifier<Image> Image { get; }

        public MessagePostImage(BurcatIdentifier<MessagePost> post, BurcatIdentifier<Image> image) { Post = post; Image = image; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from p in InterfaceOptions.GetSingleUse<IPost>() where p.Identifier == (Guid)Post.Value && p.Owner == member select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, Image];
    }

    [BurcatIdentity("0c31c450-2a86-46cf-bb70-96dc95290a88")]
    [BurcatUnique(nameof(Post), nameof(Video))]
    public class MessagePostVideo : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<MessagePost> Post { get; }
        public BurcatIdentifier<Video> Video { get; }

        public MessagePostVideo(BurcatIdentifier<MessagePost> post, BurcatIdentifier<Video> video) { Post = post; Video = video; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from p in InterfaceOptions.GetSingleUse<IPost>() where p.Identifier == (Guid)Post.Value && p.Owner == member select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, Video];
    }

    [BurcatIdentity("9d538f8b-a29b-42cd-88c5-452673e24dfa")]
    [BurcatUnique(nameof(Post), nameof(To))]
    public class PostRepost : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IPost> Post { get; }
        public BurcatIdentifier<Faction> To { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime RepostedIn { get; }

        public PostRepost(BurcatIdentifier<IPost> post, BurcatIdentifier<Faction> to, DateTime? repostedIn = null) { Post = post; To = to; RepostedIn = repostedIn ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from p in InterfaceOptions.GetSingleUse<IPost>() where p.Identifier == (Guid)Post.Value && p.Owner == member select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, To, RepostedIn];
    }

    [BurcatIdentity("2ba6721c-c62f-40e7-a911-bdadcafe27ff")]
    [BurcatUnique(nameof(Member), nameof(Post))]
    public class PostReaction : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<IPost> Post { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePostReactionReaction))]
        public string Reaction { get; set; }

        public PostReaction(BurcatIdentifier<Member> member, BurcatIdentifier<IPost> post, string reaction) { Member = member; Post = post; Reaction = reaction; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Member == member;
        public override object?[] GetBurcatConstructionValues() => [Member, Post, Reaction];
    }

    [BurcatIdentity("a4e8d20d-4832-42e2-a86c-f94a1c27c0e1")]
    [BurcatUnique(nameof(Post), nameof(With))]
    public class PostCollaboration : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IPost> Post { get; }
        public BurcatIdentifier<Member> With { get; }

        public PostCollaboration(BurcatIdentifier<IPost> about, BurcatIdentifier<Member> with) { Post = about; With = with; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from p in InterfaceOptions.GetSingleUse<IPost>() where p.Identifier == (Guid)Post.Value && p.Owner == member select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, With];
    }
}
