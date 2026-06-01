using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Market.Trading
{
    [BurcatIdentity("3731cc5a-1a7e-483f-8fdb-5362e0f11b96")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateAuction))]
    public class Auction : BurcatObject, IGood
    {
        public BurcatIdentifier<IOwnership> MadeOver { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
        public int Amount { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullNumberOverZero))]
        public decimal? ParticipationPrice { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNullDateEqualOrOverNow))]
        public DateTime? EndTime { get; set; }

        public Auction(BurcatIdentifier<IOwnership> madeOver, int amount) : this(madeOver, amount, null, null) {  }
        public Auction(BurcatIdentifier<IOwnership> madeOver, int amount, decimal? participationPrice = null, DateTime? endTime = null) { MadeOver = madeOver; Amount = amount; ParticipationPrice = participationPrice; EndTime = endTime; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from o in provider.Get<IOwnership>() where o.Identifier == (Guid)MadeOver && o.Owner == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [MadeOver, Amount];
    }

    [BurcatIdentity("6341d222-7d94-400f-83e4-acbd22bc2006")]
    [BurcatUnique(nameof(Auction), nameof(Currency))]
    public class CurrencyParticipationPrice : BurcatObject, ICurrencyPrice
    {
        public BurcatIdentifier<Auction> Auction { get; }
        public BurcatIdentifier<Currency> Currency { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
        public int Amount { get; set; }

        BurcatIdentifier<IGood> ICurrencyPrice.About => Auction.Downcast<IGood>();

        public CurrencyParticipationPrice(Auction auction, Currency currency, int amount) { Auction = auction; Currency = currency; Amount = amount; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from a in provider.Get<Auction>() join o in provider.Get<IOwnership>() on (Guid)a.MadeOver equals o.Identifier where a == Auction && o.Owner == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Auction, Currency, Amount];
    }
}
