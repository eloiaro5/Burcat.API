using Burcat.API.Development;
using Burcat.API.Market;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using BurcatProtocol.Transactions;
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
        [AtomicAction]
        public static Member? GetMember(string username) => (from a in InterfaceOptions.Provider.Get<Member>() where a.Email == username || a.Username == username select a).FirstOrDefault();

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
        /// <summary>
        /// Identifies the user as potentially undesired, preserving additional information for proper investigation.
        /// </summary>
        public bool IsUnderInvestigation { get; protected set; }

        public bool IsRealNameConfidential { get; set; }
        public bool IsBirthDayConfidential { get; set; }

        public Member(string email, string username) { Email = email; Username = username; }

        public abstract void Register(string password);
        public abstract void VerifyEmail(string code);
        public abstract void UpdatePassword(string oldPassword, string newPassword);

        public abstract Session? Login(string password);
        public abstract void LogoutAllSessions();

        /// <summary>Gets the follow relationship from this member to <paramref name="target" />, if one exists.</summary>
        [AtomicAction]
        public MemberFollow? GetFollow(BurcatIdentifier<Member> target) => InterfaceOptions.Provider.Get<MemberFollow>().FirstOrDefault(follow => follow.From == this && follow.To == target);

        /// <summary>Gets the ban relationship from this member to <paramref name="target" />, if one exists.</summary>
        [AtomicAction]
        public MemberBan? GetBan(BurcatIdentifier<Member> target) => InterfaceOptions.Provider.Get<MemberBan>().FirstOrDefault(ban => ban.From == this && ban.To == target);

        /// <summary>Determines whether this member follows <paramref name="target" />.</summary>
        [AtomicAction]
        public bool IsFollowing(BurcatIdentifier<Member> target) => GetFollow(target) is not null;

        /// <summary>Determines whether this member has banned <paramref name="target" />.</summary>
        [AtomicAction]
        public bool HasBanned(BurcatIdentifier<Member> target) => GetBan(target) is not null;

        [AtomicAction]
        public Pseudonym? GetPseudonym(Politeness tolerance) => (
            from ps in InterfaceOptions.Provider.Get<Pseudonym>()
            join p in InterfaceOptions.Provider.Get<Politeness>() on (Guid)ps.Politeness equals p.Identifier
            where (Guid)ps.Owner == Identifier orderby p descending
            select new { Pseudonym = ps, Politeness = p}).AsEnumerable().FirstOrDefault(pseudonym => pseudonym.Politeness <= tolerance)?.Pseudonym;

        [AtomicAction]
        public Image? GetIcon(Politeness tolerance) => (
            from fi in InterfaceOptions.Provider.Get<MemberIconography>()
            join i in InterfaceOptions.Provider.Get<Image>() on (Guid)fi.Icon equals i.Identifier
            join p in InterfaceOptions.Provider.Get<Politeness>() on (Guid)i.Politeness equals p.Identifier
            where (Guid)fi.Owner == Identifier orderby p descending
            select new { Image = i, Politeness = p }).AsEnumerable().FirstOrDefault(pseudonym => pseudonym.Politeness <= tolerance)?.Image;

        [AtomicAction]
        public BurcatList<IPost> GetPosts()
        {
            Politeness tolerance = Tolerance is BurcatIdentifier<Politeness> tID ? InterfaceOptions.Find(tID) : Politeness.GetNewMaximum(this);
            return [.. GetPosts<MessagePost>(tolerance).Concat<IPost>(GetPosts<RepostPost>(tolerance)).Concat(GetPosts<ImagePost>(tolerance)).Take(100)];
        }

        private IEnumerable<TPost> GetPosts<TPost>(Politeness tolerance) where TPost : BurcatObject, IPost
            => (from post in InterfaceOptions.Provider.Get<TPost>()
                join politeness in InterfaceOptions.Provider.Get<Politeness>() on (Guid)post.Politeness equals politeness.Identifier
                where (Guid)post.Owner != Identifier
                select new { Post = post, Politeness = politeness })
                .AsEnumerable().Where(value => value.Politeness <= tolerance).Select(value => value.Post);

        [AtomicAction]
        public ListSet<Faction> GetPostingFactions()
        {
            IBurcatQueryProvider provider = InterfaceOptions.Provider;
            IEnumerable<Faction> ownedFactions =
                from faction in provider.Get<Faction>()
                where faction.Owner == this
                select faction;

            IEnumerable<Faction> postableFactions =
                from membership in provider.Get<FactionMembership>()
                join role in provider.Get<FactionRole>() on (Guid?)membership.Role equals role.Identifier
                join faction in provider.Get<Faction>() on (Guid)membership.Faction equals faction.Identifier
                where membership.Member == this && role.CanPost
                select faction;

            return (ListSet<Faction>)[.. ownedFactions
                .Concat(postableFactions)
                .GroupBy(faction => faction.Identifier)
                .Select(group => group.First())
                .OrderBy(faction => faction.Name)];
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => false;
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => Identifier == member?.Value;

        public override object?[] GetBurcatConstructionValues() => [Email, Username];
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

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Icon, Name];
    }
}
