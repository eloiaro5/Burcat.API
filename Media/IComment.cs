using BurcatProtocol;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("51a09ab4-b108-4308-9af1-a5ade442bf95")]
    public interface IComment : IMessage
    {
        [Length(1, 2048)]
        public string Message { get; }
        BurcatIdentifier<IMessage>? ResponseTo { get; }
    }
}
