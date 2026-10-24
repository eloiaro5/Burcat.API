using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("a4e8d20d-4832-42e2-a86c-f94a1c27c0e1")]
    [BurcatUnique(nameof(Post), nameof(With))]
    public class MessageCollaboration : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<IPost> Post { get; }
        public BurcatIdentifier<Member> With { get; }

        public MessageCollaboration(BurcatIdentifier<IPost> about, BurcatIdentifier<Member> with) { Post = about; With = with; }

        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Queries.Where(InterfaceOptions.Provider, post => post.Identifier == Post.Value && (Guid)post.Owner == member.Value.Value).Any();
        public override object?[] GetBurcatConstructionValues() => [Post, With];
    }
}
