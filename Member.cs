using Burcat.API.Development;
using Burcat.API.Market;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API
{
    [BurcatIdentity("b83de0f1-71b1-4fdc-9023-7605c87cc896")]
    [BurcatUnique(nameof(Email))]
    [BurcatUnique(nameof(Username))]
    public abstract class Member : BurcatObject, IInterfaceObject, IPublisher, IProfile
    {
        public static Member? GetMember(string username) => (from a in InterfaceOptions.GetSingleUse<Member>() where a.Email == username || a.Username == username select a).FirstOrDefault();

        [EmailAddress]
        public string Email { get; }
        [Length(4, 32)]
        public string Username { get; }
        public string? RealName { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullDateEqualOrUnderNow))]
        public DateTime? BirthDay { get; set; }
        [EmailAddress]
        public string? PublicEmail { get; set; }
        public decimal Lety { get; set; }
        public BurcatIdentifier<Politeness>? Tolerance { get; set; }

        public bool IsEmailVerified { get; protected set; }

        public bool IsRealNameConfidential { get; set; }
        public bool IsBirthDayConfidential { get; set; }

        public Member(string email, string username) { Email = email; Username = username; }

        public abstract void Register(string password);
        public abstract void VerifyEmail(string code);
        public abstract void UpdatePassword(string newPassword);

        public abstract Session? Login(string password);
        public abstract void LogoutAllSessions();

        public Pseudonym? GetPseudonym(Politeness tolerance) => (
            from ps in InterfaceOptions.GetSingleUse<Pseudonym>()
            join p in InterfaceOptions.GetSingleUse<Politeness>() on (Guid)ps.Politeness equals p.Identifier
            where (Guid)ps.Owner == Identifier orderby p descending
            select new { Pseudonym = ps, Politeness = p}).AsEnumerable().FirstOrDefault(pseudonym => pseudonym.Politeness <= tolerance)?.Pseudonym;

        public Image? GetIcon(Politeness tolerance) => (
            from fi in InterfaceOptions.GetSingleUse<MemberIconography>()
            join i in InterfaceOptions.GetSingleUse<Image>() on (Guid)fi.Icon equals i.Identifier
            join p in InterfaceOptions.GetSingleUse<Politeness>() on (Guid)i.Politeness equals p.Identifier
            where (Guid)fi.Owner == Identifier orderby p descending
            select new { Image = i, Politeness = p }).AsEnumerable().FirstOrDefault(pseudonym => pseudonym.Politeness <= tolerance)?.Image;

        public BurcatList<IPost> GetPosts()
        {
            Politeness tolerance = Tolerance is BurcatIdentifier<Politeness> tID ? InterfaceOptions.Find(tID) : Politeness.GetNewMaximum(this);
            return new([..
            (
            from pt in InterfaceOptions.GetSingleUse<IPost>()
            join pl in InterfaceOptions.GetSingleUse<Politeness>() on (Guid)pt.Politeness equals pl.Identifier
            where (Guid)pt.Owner != Identifier
            select new { Post = pt, Politeness = pl }
            ).AsEnumerable().Where(p => p.Politeness <= tolerance).Select(p => p.Post).Take(100)]);
        }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => Identifier == member?.Value;
        public override object?[] GetBurcatConstructionValues() => [Email, Username];
    }

    [BurcatIdentity("cd6b4144-c132-4945-a890-68307486a4d9")]
    [BurcatUnique(nameof(From), nameof(To))]
    public class MemberBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> From { get; }
        public BurcatIdentifier<Member> To { get; }

        public MemberBan(BurcatIdentifier<Member> from, BurcatIdentifier<Member> to) { From = from; To = to; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && From == member;
        public override object?[] GetBurcatConstructionValues() => [From, To];
    }

    [BurcatIdentity("a744e0b1-fe53-4e46-9f6e-18b9b109a9a1")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateIconography))]
    public class MemberIconography : BurcatObject, IInterfaceObject, IIconography
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Image> Icon { get; }
        [Length(1, 32)]
        public string Name { get; set; }

        BurcatIdentifier<IProfile> IIconography.Owner => Owner.Downcast<IProfile>();

        public MemberIconography(BurcatIdentifier<Member> owner, BurcatIdentifier<Image> icon, [Length(1, 32)] string name) { Owner = owner; Icon = icon; Name = name; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Icon, Name];
    }
}
