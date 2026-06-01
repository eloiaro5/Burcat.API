using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Media
{
    [BurcatIdentity("f9397374-3ff7-4a06-8e62-6f9eae16f080")]
    public interface IPost : IInterfaceObject
    {
        BurcatIdentifier<Member> Owner { get; }
        [Length(1, 8129)]
        string Message { get; }
        BurcatIdentifier<Politeness> Politeness { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePostTags))]
        string? Tags { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        DateTime PostDate { get; }
    }

    [BurcatIdentity("4d7150b8-eeb4-4fa9-9107-1c13af0d1daf")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessagePost))]
    public class MessagePost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public string Message { get; }
        public BurcatIdentifier<Image>? Image { get; set; }
        public BurcatIdentifier<Video>? Video { get; set; }
        public BurcatIdentifier<Politeness> Politeness { get; set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }
        public BurcatIdentifier<IPost>? ResponseTo { get; }

        public MessagePost(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, string message, string? tags = null, DateTime postDate = default, BurcatIdentifier<IPost>? responseTo = null) { Owner = owner; Message = message; Politeness = politeness; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; ResponseTo = responseTo; }

        bool IInterfaceObject.ShouldCreate(BurcatIdentifier<Member>? member) => ((IInterfaceObject)this).ShouldManage(member);
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Message, Tags, PostDate, ResponseTo];
    }

    [BurcatIdentity("b0076bd1-c2b1-41f5-91ab-225e450ba3d0")]
    public class TitlePost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        [Length(1, 128)]
        public string Title { get; }
        public string Message { get; set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        public TitlePost(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, string title, string message, string? tags = null, DateTime postDate = default) { Owner = owner; Politeness = politeness; Title = title; Message = message; Tags = tags; PostDate = postDate == default ? DateTime.Now : postDate; }

        bool IInterfaceObject.ShouldCreate(BurcatIdentifier<Member>? member) => ((IInterfaceObject)this).ShouldManage(member);
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Title, Message, Tags, PostDate];
    }

    [BurcatIdentity("ba3bfef2-9340-4d7b-a3e3-eb36021fb26d")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateImagePost))]
    public class ImagePost : TitlePost
    {
        public BurcatIdentifier<Image> Image { get; }

        public ImagePost(BurcatIdentifier<Member> owner, BurcatIdentifier<Image> image, BurcatIdentifier<Politeness> politeness, string title, string message, string? tags = null, DateTime postDate = default) : base(owner, politeness, title, message, tags, postDate) { Image = image; }

        public override object?[] GetBurcatConstructionValues() => [Owner, Image, Politeness, Title, Message, Tags, PostDate];
    }

    [BurcatIdentity("71c2db78-9c83-49f9-9696-31bc4950a182")]
    public class VideoPost : BurcatObject, IPost
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Video> Video { get; }
        public double AdvertisementRatio { get; set; }
        [Length(1, 8129)]
        public string Description { get; set; }
        public string? Tags { get; set; }
        public DateTime PostDate { get; }

        BurcatIdentifier<Politeness> IPost.Politeness => InterfaceOptions.UseProvider(provider => (from v in provider.Get<Video>() where v == Video select v.Politeness).First());
        string IPost.Message => Description;

        public VideoPost(BurcatIdentifier<Member> owner, BurcatIdentifier<Video> video, string description, string? tags = null, DateTime? postDate = null) { Owner = owner; Video = video; Description = description; Tags = tags; PostDate = postDate ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Video, PostDate];
    }
}
