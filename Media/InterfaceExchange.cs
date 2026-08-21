using Burcat.API.Media;
using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burcat.API
{
    public abstract partial class InterfaceExchange
    {
        public abstract BurcatList<PostSearchResponse> DoCommentSearch(Member? member, Member? target, string? search, Pagination pagination);
        public abstract BurcatList<PostSearchResponse> DoCommentFeed(Member member, Pagination pagination);
        public abstract BurcatList<PostSearchResponse> DoPostSearch(Member? member, Faction? target, string? search, Pagination pagination);
        public abstract BurcatList<PostSearchResponse> DoPostFeed(Member member, Pagination pagination);

        public abstract PostSearchResponse? GetPost(Guid identifier);
        public abstract UserProfileResponse? GetUserProfile(Member? viewer, Guid identifier);
    }

    [BurcatIdentity("05af82d3-ad91-40ff-8808-43b94874e5a7")]
    public readonly struct Pagination : IBurcatObject
    {
        public Guid Identifier { get => Guid.Empty; set => throw new InvalidOperationException(); }
        public Guid Revision { get => Guid.Empty; set => throw new InvalidOperationException(); }

        public int Offset { get; }
        public int Limit { get; }

        public Pagination() { Offset = 0; Limit = 25; }
        public Pagination(int offset, int limit) { Offset = offset; Limit = limit; }

        public BurcatField[] GetBurcatFields() => [];
        public void SetBurcatFields(BurcatField[] fields) { }

        public IBurcatObject?[] GetBurcatConstructionValues() => BurcatTranslator.ObjectsTranslate([Offset, Limit]);
    }

    [BurcatIdentity("69cb4a43-12fa-437d-8eac-21887aec6e2b")]
    public class PostSearchResponse : BurcatObject
    {
        public Member Owner { get; }
        public IMessage Message { get; }
        public Politeness Politeness { get; }
        public BurcatList<PostReaction> Reactions { get; }
        public BurcatList<PostSearchResponse> Responses { get; }

        public PostSearchResponse(Member owner, IMessage message, Politeness politeness, BurcatList<PostReaction> reactions, BurcatList<PostSearchResponse> responses) : base(Guid.Empty) { Owner = owner; Message = message; Politeness = politeness; Reactions = reactions; Responses = responses; }

        public override object?[] GetBurcatConstructionValues() => [Owner, Message, Politeness, Reactions, Responses];
    }

    public class PostRepostResponse : BurcatObject
    {
        public Faction Faction { get; }
        public Image Icon { get; }

        public PostRepostResponse(Faction faction, Image icon) { Faction = faction; Icon = icon; }

        public override object?[] GetBurcatConstructionValues() => [Faction, Icon];
    }

    [BurcatIdentity("f4176ce0-4fd9-4292-a4a2-3f9062762e37")]
    public class UserProfileResponse : BurcatObject
    {
        public Member Member { get; }
        public Pseudonym? Pseudonym { get; }
        public Image? Icon { get; }
        public MemberFollow? Follow { get; }
        public MemberBan? Ban { get; }
        public BurcatList<Faction> Factions { get; }
        public BurcatList<PostSearchResponse> Comments { get; }
        public BurcatList<PostSearchResponse> Posts { get; }

        public UserProfileResponse(Member member, Pseudonym? pseudonym, Image? icon, MemberFollow? follow, MemberBan? ban, BurcatList<Faction> factions, BurcatList<PostSearchResponse> comments, BurcatList<PostSearchResponse> posts) : base(Guid.Empty)
        {
            Member = member;
            Pseudonym = pseudonym;
            Icon = icon;
            Follow = follow;
            Ban = ban;
            Factions = factions;
            Comments = comments;
            Posts = posts;
        }

        public override object?[] GetBurcatConstructionValues() => [Member, Pseudonym, Icon, Follow, Ban, Factions, Comments, Posts];
    }
}
