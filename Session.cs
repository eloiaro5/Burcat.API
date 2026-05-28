using BurcatProtocol;
using BurcatProtocol.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API
{
    [BurcatIdentity("77f5026a-c00d-4199-9031-b3c17a0d4845")]
    [BurcatUnique(nameof(Token))]
    public abstract class Session : BurcatObject, IInterfaceObject
    {
        public static Session? GetSession(string token) => (from s in InterfaceOptions.GetSingleUse<Session>() where s.Token == token select s).FirstOrDefault();

        public BurcatIdentifier<Member> Owner { get; }
        [Length(24, 24)]
        public string Token { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullDateEqualOrOverNow))]
        public DateTime? EndTime { get; set; }

        public Session(BurcatIdentifier<Member> owner, string token, DateTime? endTime = null) { Owner = owner; Token = token; EndTime = endTime; }

        public abstract void Logout();

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Token, EndTime];
    }
}
