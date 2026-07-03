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

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from p in provider.Get<IPost>() where p.Identifier == Post.Value && p.Owner == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Post, Payment];
    }

    [BurcatIdentity("2ba6721c-c62f-40e7-a911-bdadcafe27ff")]
    [BurcatUnique(nameof(Member), nameof(Message))]
    public class PostReaction : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<IMessage> Message { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePostReactionReaction))]
        public string Reaction { get; set; }

        public PostReaction(BurcatIdentifier<Member> member, BurcatIdentifier<IMessage> message, string reaction) { Member = member; Message = message; Reaction = reaction; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Member == member;

        public override object?[] GetBurcatConstructionValues() => [Member, Message, Reaction];
    }

    [BurcatIdentity("a4e8d20d-4832-42e2-a86c-f94a1c27c0e1")]
    [BurcatUnique(nameof(Post), nameof(With))]
    public class PostCollaboration : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IPost> Post { get; }
        public BurcatIdentifier<Member> With { get; }

        public PostCollaboration(BurcatIdentifier<IPost> about, BurcatIdentifier<Member> with) { Post = about; With = with; }

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from p in provider.Get<IPost>() where p.Identifier == (Guid)Post.Value && p.Owner == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Post, With];
    }
}
