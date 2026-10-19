using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using BurcatProtocol.Transactions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Media
{
    [BurcatIdentity("f9397374-3ff7-4a06-8e62-6f9eae16f080")]
    public interface IMessage : IInterfaceObject
    {
        BurcatIdentifier<Member> Owner { get; }
        BurcatIdentifier<Politeness> Politeness { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        DateTime PostDate { get; }

        /// <summary>Clears message content and media references while preserving identity and conversation metadata.</summary>
        /// <exception cref="ValidationException">The owner is under investigation.</exception>
        public void Delete();
    }

    [BurcatIdentity("51a09ab4-b108-4308-9af1-a5ade442bf95")]
    public interface IComment : IMessage
    {
        [Length(1, 2048)]
        public string Message { get; }
        BurcatIdentifier<IMessage>? ResponseTo { get; }
    }

    [BurcatIdentity("c155eea2-7cbe-4c58-95f0-88f8dc63c10d")]
    public interface IPost : IMessage
    {
        BurcatIdentifier<Faction> Faction { get; }
        [Length(1, 128)]
        public string Title { get; }
        [Length(1, 8129)]
        public string Message { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePostTags))]
        public string? Tags { get; set; }
    }

    [BurcatIdentity("4d7150b8-eeb4-4fa9-9107-1c13af0d1daf")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessageComment))]
    public class MessageComment : BurcatObject, IComment
    {
        public BurcatIdentifier<Member> Owner { get; }
        [Length(1, 2048)]
        public string Message { get; private set; }
        public BurcatIdentifier<Image>? Image { get; private set; }
        public BurcatIdentifier<Video>? Video { get; private set; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public DateTime PostDate { get; }
        public BurcatIdentifier<IMessage>? ResponseTo { get; }

        public MessageComment(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, string message, DateTime postDate = default, BurcatIdentifier<IMessage>? responseTo = null)
            : this(owner, politeness, message, null, null, postDate, responseTo) { }

        /// <summary>Creates a comment with optional image or video attachment references.</summary>
        public MessageComment(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, string message, BurcatIdentifier<Image>? image, BurcatIdentifier<Video>? video, DateTime postDate = default, BurcatIdentifier<IMessage>? responseTo = null)
        {
            Owner = owner;
            Message = message;
            Politeness = politeness;
            Image = image;
            Video = video;
            PostDate = postDate == default ? DateTime.Now : postDate;
            ResponseTo = responseTo;
        }

        /// <inheritdoc />
        [AtomicAction]
        public void Delete()
        {
            if (DataValidator.ValidateMessageDeletion(this) is ValidationResult validation) throw new ValidationException(validation, null, this);

            Message = " ";
            Image = null;
            Video = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Message, PostDate, ResponseTo];
    }

    [BurcatIdentity("f835b971-a090-44fd-a5e7-5e3c8755cec9")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateRepostPost))]
    public class RepostPost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public BurcatIdentifier<Faction> Faction { get; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public BurcatIdentifier<IPost>? Post { get; private set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        public RepostPost(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, BurcatIdentifier<IPost> post, string? tags = null, DateTime postDate = default)
            : this(owner, politeness, faction, title, message, (BurcatIdentifier<IPost>?)post, tags, postDate) { }

        /// <summary>Reconstructs a repost, allowing a cleared original-post reference after deletion.</summary>
        public RepostPost(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, BurcatIdentifier<IPost>? post, string? tags, DateTime postDate) { Owner = owner; Politeness = politeness; Faction = faction; Title = title; Message = message; Post = post; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

        /// <inheritdoc />
        [AtomicAction]
        public void Delete()
        {
            if (DataValidator.ValidateMessageDeletion(this) is ValidationResult validation) throw new ValidationException(validation, null, this);

            Title = " ";
            Message = " ";
            Tags = null;
            Post = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Faction, Title, Message, Post, Tags, PostDate];
    }

    [BurcatIdentity("b0076bd1-c2b1-41f5-91ab-225e450ba3d0")]
    public class MessagePost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public BurcatIdentifier<Faction> Faction { get; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        public MessagePost(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, string? tags = null, DateTime postDate = default) { Owner = owner; Politeness = politeness; Faction = faction ; Title = title; Message = message; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

        /// <inheritdoc />
        [AtomicAction]
        public void Delete()
        {
            if (DataValidator.ValidateMessageDeletion(this) is ValidationResult validation) throw new ValidationException(validation, null, this);

            Title = " ";
            Message = " ";
            Tags = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Faction, Title, Message, Tags, PostDate];
    }

    [BurcatIdentity("ba3bfef2-9340-4d7b-a3e3-eb36021fb26d")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateImagePost))]
    public class ImagePost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public BurcatIdentifier<Faction> Faction { get; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public BurcatIdentifier<Image>? Image { get; private set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        public ImagePost(BurcatIdentifier<Member> owner, BurcatIdentifier<Image> image, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, string? tags = null, DateTime postDate = default)
            : this(owner, (BurcatIdentifier<Image>?)image, politeness, faction, title, message, tags, postDate) { }

        /// <summary>Reconstructs an image post, allowing a cleared image reference after deletion.</summary>
        public ImagePost(BurcatIdentifier<Member> owner, BurcatIdentifier<Image>? image, BurcatIdentifier<Politeness> politeness, BurcatIdentifier<Faction> faction, string title, string message, string? tags, DateTime postDate) { Owner = owner; Politeness = politeness; Faction = faction; Title = title; Message = message; Image = image; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

        /// <inheritdoc />
        [AtomicAction]
        public void Delete()
        {
            if (DataValidator.ValidateMessageDeletion(this) is ValidationResult validation) throw new ValidationException(validation, null, this);

            Title = " ";
            Message = " ";
            Tags = null;
            Image = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Image, Politeness, Faction, Title, Message, Tags, PostDate];
    }

    //[BurcatIdentity("71c2db78-9c83-49f9-9696-31bc4950a182")]
    //public class VideoPost : BurcatObject, IPost
    //{
    //    public BurcatIdentifier<Member> Owner { get; }
    //    public BurcatIdentifier<Video> Video { get; }
    //    public double AdvertisementRatio { get; set; }
    //    [Length(1, 8129)]
    //    public string Description { get; set; }
    //    public string? Tags { get; set; }
    //    public DateTime PostDate { get; }

    //    BurcatIdentifier<Politeness> IPost.Politeness => (from v in InterfaceOptions.Provider.Get<Video>() where v == Video select v.Politeness).First();
    //    string IPost.Message => Description;

    //    public VideoPost(BurcatIdentifier<Member> owner, BurcatIdentifier<Video> video, string description, string? tags = null, DateTime? postDate = null) { Owner = owner; Video = video; Description = description; Tags = tags; PostDate = postDate ?? DateTime.Now; }

    //    public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
    //    public override object?[] GetBurcatConstructionValues() => [Owner, Video, PostDate];
    //}
}
