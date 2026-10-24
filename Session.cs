using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Transactions;
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
        [AtomicAction]
        public static Session? GetSession(string token) => (from s in InterfaceOptions.Provider.Get<Session>()
            join owner in InterfaceOptions.Provider.Get<Member>() on (Guid)s.Owner equals owner.Identifier
            where s.Token == token && !owner.IsBanned && (s.EndTime == null || s.EndTime > DateTime.Now)
            select s).FirstOrDefault();

        public BurcatIdentifier<Member> Owner { get; }
        [Length(24, 24)]
        public string Token { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullDateEqualOrOverNow))]
        public DateTime? EndTime { get; set; }

        public Session(BurcatIdentifier<Member> owner, string token, DateTime? endTime = null) { Owner = owner; Token = token; EndTime = endTime; }

        public abstract void Logout();

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => InterfaceOptions.TryFind(Owner) is Member owner && !owner.IsBanned;
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Token, EndTime];
    }
}
