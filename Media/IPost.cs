using BurcatProtocol;
using BurcatProtocol.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API.Media
{
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
