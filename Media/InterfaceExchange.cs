using Burcat.API.Media;
using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burcat.API
{
    public abstract partial class InterfaceExchange
    {
        public abstract BurcatList<PostSearchResponse> DoPostSearch(Member? member, string? search);
        public abstract BurcatList<PostSearchResponse> DoCommentSearch(Member? member, string? search);
        public abstract PostSearchResponse? GetPost(Guid identifier);
        public abstract UserProfileResponse? GetUserProfile(Member? viewer, Guid identifier);
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
        public BurcatList<Faction> Factions { get; }
        public BurcatList<PostSearchResponse> Comments { get; }
        public BurcatList<PostSearchResponse> Posts { get; }

        public UserProfileResponse(Member member, BurcatList<Faction> factions, BurcatList<PostSearchResponse> comments, BurcatList<PostSearchResponse> posts) : base(Guid.Empty)
        {
            Member = member;
            Factions = factions;
            Comments = comments;
            Posts = posts;
        }

        public override object?[] GetBurcatConstructionValues() => [Member, Factions, Comments, Posts];
    }
}
