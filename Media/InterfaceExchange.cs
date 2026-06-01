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
    }

    [BurcatIdentity("69cb4a43-12fa-437d-8eac-21887aec6e2b")]
    public class PostSearchResponse : BurcatObject
    {
        public Member Owner { get; }
        public IPost Post { get; }
        public BurcatList<PostReaction> Reactions { get; }
        public BurcatList<PostSearchResponse> Responses { get; }

        public PostSearchResponse(Member owner, IPost post, BurcatList<PostReaction> reactions, BurcatList<PostSearchResponse> responses) : base(Guid.Empty) { Owner = owner; Post = post; Reactions = reactions; Responses = responses; }

        public override object?[] GetBurcatConstructionValues() => [Owner, Post, Reactions, Responses];
    }
}
