using Burcat.API.Development;
using Burcat.API.Market.Trading;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API.Market.Passes
{
    [BurcatIdentity("79c142f1-a4f7-4be3-b348-76a082c1c5db")]
    [BurcatUnique(nameof(Name))]
    public class BattlePass : BurcatObject, IGood
    {
        [Length(4, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image> Icon { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullDateEqualOrOverNow))]
        public DateTime? EndTime { get; set; }

        public BattlePass(string name, Image icon, decimal price) { Name = name; Icon = icon; Price = price; }
        public BattlePass(string name, Image icon, decimal price, string description) : this(name, icon, price) { Description = description; }
        public BattlePass(string name, Image icon, decimal price, DateTime endTime) : this(name, icon, price) { EndTime = endTime; }
        public BattlePass(string name, Image icon, decimal price, string description, DateTime endTime) : this(name, icon, price, description) { EndTime = endTime; }

        public ListSet<Mission> GetMissions() => InterfaceOptions.UseProvider(provider => (ListSet<Mission>)[.. provider.Get<Mission>().Where(m => m.Pass == this)]);

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Name, Icon, Price];
    }

    [BurcatIdentity("e60f92f3-646e-4cba-9c86-1f5ce25a5a7d")]
    [BurcatUnique(nameof(Pass), nameof(Amount))]
    public class BattlePassCurrencyPrice : BurcatObject, ICurrencyPrice
    {
        public BurcatIdentifier<BattlePass> Pass { get; }
        public BurcatIdentifier<Currency> Currency { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
        public int Amount { get; set; }

        BurcatIdentifier<IGood> ICurrencyPrice.About => Pass.Downcast<IGood>();

        public BattlePassCurrencyPrice(BurcatIdentifier<BattlePass> pass, BurcatIdentifier<Currency> currency, int amount) { Pass = pass; Currency = currency; Amount = amount; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Pass, Currency, Amount];
    }

    [BurcatIdentity("464eb155-86f0-4a82-90df-c5e02fc97148")]
    public class BattlePassEntry : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<BattlePass> Pass { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrOverNow))]
        public DateTime ObtainedIn { get; }

        public BattlePassEntry(BurcatIdentifier<Member> owner, BurcatIdentifier<BattlePass> pass, DateTime? obtainedIn) { Owner = owner; Pass = pass; ObtainedIn = obtainedIn ?? DateTime.Now; }

        public ListSet<Mission> GetAvaliableMissions()
        {
            return InterfaceOptions.UseProvider(provider => (ListSet<Mission>)[.. 
                from mission in provider.Get<Mission>().Where(m => m.Pass == Pass)
                let dependencies = provider.Get<MissionDependency>().Where(d => d.Mission == mission).Select(d => d.DependsOn)
                let completions = provider.Get<MissionCompletion>().Where(c => c.Entry == this).Select(c => c.Mission)
                where dependencies.All(d => completions.Contains(d))
                select mission
            ]);
        }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Pass, ObtainedIn];
    }
}
