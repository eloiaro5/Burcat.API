using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Market.Trading
{
    [BurcatIdentity("15e3ab3f-0a5a-41f8-8839-6b6dcd8144f0")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateBid))]
    [BurcatUnique(nameof(MadeBy), nameof(MadeTo))]
    public class Bid : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> MadeBy { get; }
        public BurcatIdentifier<Auction> MadeTo { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
        public decimal Amount { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime MadeIn { get; }

        public Bid(BurcatIdentifier<Member> madeBy, BurcatIdentifier<Auction> madeTo, decimal amount, DateTime? madeIn = null) { MadeBy = madeBy; MadeTo = madeTo; Amount = amount; MadeIn = madeIn ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && MadeBy == member;
        public override object?[] GetBurcatConstructionValues() => [MadeBy, MadeTo, Amount, MadeIn];
    }

    [BurcatIdentity("a52b142b-7e74-4c96-80a6-10758ddd75fc")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateCurrencyBid))]
    [BurcatUnique(nameof(MadeBy), nameof(MadeTo), nameof(MadeWith))]
	public class CurrencyBid : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> MadeBy { get; }
        public BurcatIdentifier<Auction> MadeTo { get; }
        public BurcatIdentifier<Currency> MadeWith { get; }
		[BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
		public int Amount { get; }
		[BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
		public DateTime MadeIn { get; }

		public CurrencyBid(BurcatIdentifier<Member> madeBy, BurcatIdentifier<Auction> madeTo, BurcatIdentifier<Currency> madeWith, int amount, DateTime? madeIn) { MadeBy = madeBy; MadeTo = madeTo; MadeWith = madeWith; Amount = amount; MadeIn = madeIn ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && MadeBy == member;
        public override object?[] GetBurcatConstructionValues() => [MadeBy, MadeTo, MadeWith, Amount, MadeIn];
    }
}
