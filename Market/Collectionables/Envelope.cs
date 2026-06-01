using Burcat.API.Development;
using Burcat.API.Market.Passes;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Market.Collectionables
{

    [BurcatIdentity("ce4a896d-0fdb-4b3c-9f3a-dc9866191c40")]
    [BurcatUnique(nameof(Name))]
    public class Envelope : BurcatObject, IGood
    {
        [Length(4, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image> Icon { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullDateEqualOrOverNow))]
        public DateTime? EndTime { get; set; }

        public Envelope(string name, BurcatIdentifier<Image> icon, decimal price) { Name = name; Icon = icon; Price = price; }
        public Envelope(string name, BurcatIdentifier<Image> icon, decimal price, string description) : this(name, icon, price) { Description = description; }
        public Envelope(string name, BurcatIdentifier<Image> icon, decimal price, DateTime endTime) : this(name, icon, price) { EndTime = endTime; }
        public Envelope(string name, BurcatIdentifier<Image> icon, decimal price, string description, DateTime endTime) : this(name, icon, price, description) { EndTime = endTime; }

        public ListSet<EnvelopeCurrencyPrice> GetPrices() => InterfaceOptions.UseProvider(provider => (ListSet<EnvelopeCurrencyPrice>)[.. from p in provider.Get<EnvelopeCurrencyPrice>() where p.Envelope == this select p]);
        public BurcatList<Probability> GetProbabilities() => InterfaceOptions.UseProvider(provider => (BurcatList<Probability>)[.. from p in provider.Get<Probability>() where p.Envelope == this select p]);

        public BurcatList<CardLevel> RollCards()
        {
            return new();
            //Random rnd = new();
            //return [.. from p in GetProbabilities() where p.Chance >= (decimal)rnd.NextDouble() select p.CardLevel]);
        }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Name, Icon, Price];
    }

    [BurcatIdentity("d3ac08a4-6090-4b70-8b6d-c1fbff9d1812")]
    [BurcatUnique(nameof(Envelope), nameof(Currency))]
    public class EnvelopeCurrencyPrice : BurcatObject, ICurrencyPrice
    {
        public BurcatIdentifier<Envelope> Envelope { get; }
        public BurcatIdentifier<Currency> Currency { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
        public int Amount { get; set; }

        BurcatIdentifier<IGood> ICurrencyPrice.About => Envelope.Downcast<IGood>();

        public EnvelopeCurrencyPrice(BurcatIdentifier<Envelope> envelope, BurcatIdentifier<Currency> currency, int amount) { Envelope = envelope; Currency = currency; Amount = amount; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Envelope, Currency, Amount];
    }
}
