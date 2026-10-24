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
    public class MessageAdvertisement : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IPost> Post { get; }
        public decimal Payment { get; set; }
        public BurcatIdentifier<Politeness>? MaximumPoliteness { get; set; }

        public MessageAdvertisement(BurcatIdentifier<IPost> post, decimal payment, BurcatIdentifier<Politeness>? maximumPoliteness = null) { Post = post; Payment = payment; MaximumPoliteness = maximumPoliteness; }

        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Queries.Where(InterfaceOptions.Provider, post => post.Identifier == Post.Value && (Guid)post.Owner == member.Value.Value).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, Payment];
    }
}
