using Burcat.API.Development;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Market
{
    [BurcatIdentity("72527a05-644e-4a02-89c1-f26b171334e8")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateCurrency))]
    [BurcatUnique(nameof(Creator), nameof(Name))]
    public class Currency : BurcatObject, IAsset
    {
        public BurcatIdentifier<DevOps> Creator { get; }
        [Length(4, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image> Icon { get; set; }
        public string? Description { get; set; }

        public Currency(BurcatIdentifier<DevOps> creator, string name, BurcatIdentifier<Image> icon) { Creator = creator; Name = name; Icon = icon; }
        public Currency(BurcatIdentifier<DevOps> creator, string name, BurcatIdentifier<Image> icon, string description) : this(creator, name, icon) { Description = description; }

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from s in provider.Get<DevOps>() where s == Creator && s.Member == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Creator, Name, Icon];
    }

    [BurcatIdentity("a04fa3c5-7426-416b-aa64-bc84aff9dd2e")]
    [BurcatUnique(nameof(Owner), nameof(Currency))]
    public class CurrencyOwnership : BurcatObject, IOwnership
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Currency> Currency { get; }
        public int Amount { get; set; }

        BurcatIdentifier<IAsset> IOwnership.About => Currency.Downcast<IAsset>();

        public CurrencyOwnership(BurcatIdentifier<Member> owner, BurcatIdentifier<Currency> currrency, int amount) { Owner = owner; Currency = currrency; Amount = amount; }

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Currency, Amount];
    }
}
