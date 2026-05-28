using Burcat.API.Development;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API.Market.Passes
{
    [BurcatIdentity("0910efc3-18a0-4bc6-9887-15af70e666aa")]
    [BurcatUnique(nameof(Pass), nameof(Name))]
    public abstract class Mission : BurcatObject, IProfile
    {
        public BurcatIdentifier<BattlePass> Pass { get; }
        [Length(4, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image> Icon { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public decimal Prize { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullNumberEqualOrOverZero))]
        public int? CompletionTimes { get; set; }
        public string? Description { get; set; }

        public Mission(BurcatIdentifier<BattlePass> pass, string name, BurcatIdentifier<Image> icon, decimal prize) { Pass = pass; Name = name; Icon = icon; Prize = prize; }
        public Mission(BurcatIdentifier<BattlePass> pass, string name, BurcatIdentifier<Image> icon, decimal prize, int completionTimes) : this(pass, name, icon, prize) { CompletionTimes = completionTimes; }
        public Mission(BurcatIdentifier<BattlePass> pass, string name, BurcatIdentifier<Image> icon, decimal prize, string description) : this(pass, name, icon, prize) { Description = description; }
        public Mission(BurcatIdentifier<BattlePass> pass, string name, BurcatIdentifier<Image> icon, decimal prize, int completionTimes, string description) : this(pass, name, icon, prize, completionTimes) { Description = description; }

        public abstract bool Complete(BattlePassEntry entry);

        //public ListSet<MissionCurrencyPrize> GetPrizes() => new([.. InterfaceConfiguration.Provider.GetQueryable<MissionCurrencyPrize>(this).Where(p => p.Mission == this)]);
        //public ListSet<Mission> GetDependencies() => [.. InterfaceConfiguration.Provider.GetQueryable<MissionDependency>(this).Where(d => d.Mission == this).Select(d => d.DependsOn)]);
        //public ListSet<MissionCompletion> GetCompletions() => [.. InterfaceConfiguration.Provider.GetQueryable<MissionCompletion>(this).Where(c => c.Mission == this)]);

        //public ListSet<Image> GetIcons() => [.. InterfaceConfiguration.Provider.GetQueryable<MissionIcononography>(this).Where(i => i.Mission == this).Select(i => i.Icon)]);
        //public Image? GetIcon(Politeness tolerance) => GetIcons().OrderBy(i => i.Politeness).FirstOrDefault(i => i.Politeness <= tolerance);
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Pass, Name, Icon, Prize];
    }

    [BurcatIdentity("0e36deac-bd96-4af4-bfbd-dc39cff4511a")]
    [BurcatUnique(nameof(Mission), nameof(Currency))]
    public class MissionCurrencyPrize : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Mission> Mission { get; }
        public BurcatIdentifier<Currency> Currency { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Prize { get; set; }

        public MissionCurrencyPrize(BurcatIdentifier<Mission> mission, BurcatIdentifier<Currency> currency, int prize) { Mission = mission; Currency = currency; Prize = prize; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Mission, Currency, Prize];
    }

    [BurcatIdentity("e1c46f4f-57cf-4f54-bb01-17cef00c3955")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMissionDependency))]
    [BurcatUnique(nameof(Mission), nameof(DependsOn))]
    public class MissionDependency : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Mission> Mission { get; }
        public BurcatIdentifier<Mission> DependsOn { get; }

        public MissionDependency(BurcatIdentifier<Mission> mission, BurcatIdentifier<Mission> dependsOn) { Mission = mission; DependsOn = dependsOn; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Mission, DependsOn];
    }

    [BurcatIdentity("8b79299f-0a4a-47d6-a690-cb8fdcaedc41")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    public class MissionIcononography : BurcatObject, IIconography
    {
        public BurcatIdentifier<Mission> Owner { get; }
        public BurcatIdentifier<Image> Icon { get; }
        [Length(1, 32)]
        public string Name { get; set; }

        BurcatIdentifier<IProfile> IIconography.Owner => Owner.Downcast<IProfile>();

        public MissionIcononography(BurcatIdentifier<Mission> owner, BurcatIdentifier<Image> icon, string name) { Owner = owner; Icon = icon; Name = name; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Owner, Icon, Name];
    }

    [BurcatIdentity("6c1981af-92cb-41a1-a107-3abfe317d47f")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMissionCompletion))]
    [BurcatUnique(nameof(Entry), nameof(Mission), nameof(Step))]
    public class MissionCompletion : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<BattlePassEntry> Entry { get; }
        public BurcatIdentifier<Mission> Mission { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Step { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime CompletedIn { get; }

        public MissionCompletion(BurcatIdentifier<BattlePassEntry> entry, BurcatIdentifier<Mission> mission, int step, DateTime completedIn) { Entry = entry; Mission = mission; Step = step; CompletedIn = completedIn; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Entry, Mission, Step, CompletedIn];
    }
}
