using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Transactions;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("4d7150b8-eeb4-4fa9-9107-1c13af0d1daf")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessageComment))]
    public class CommentMessage : BurcatObject, IComment
    {
        public BurcatIdentifier<Member> Owner { get; }
        [Length(1, 2048)]
        public string Message { get; private set; }
        public BurcatIdentifier<Image>? Image { get; private set; }
        public BurcatIdentifier<Video>? Video { get; private set; }
        public BurcatIdentifier<Politeness> Politeness { get; }
        public DateTime PostDate { get; }
        public BurcatIdentifier<IMessage>? ResponseTo { get; }

        public CommentMessage(BurcatIdentifier<Member> owner, BurcatIdentifier<Politeness> politeness, string message, BurcatIdentifier<Image>? image = null, BurcatIdentifier<Video>? video = null, DateTime postDate = default, BurcatIdentifier<IMessage>? responseTo = null)
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
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessageDeletion))]
        public void Delete()
        {
            Message = " ";
            Image = null;
            Video = null;
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => false;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Politeness, Message, Image, Video, PostDate, ResponseTo];
    }
}
