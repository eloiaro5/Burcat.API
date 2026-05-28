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
    [BurcatUnique(nameof(Owner), nameof(About))]
    public interface IOwnership : IInterfaceObject
    {
        BurcatIdentifier<Member> Owner { get; }
        BurcatIdentifier<IAsset> About { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        int Amount { get; set; }
    }

    public interface IAsset : IInterfaceObject { }

    [BurcatIdentity("086a0b9e-33aa-438a-9638-7b86c90d1a4e")]
    public class OwnershipTransfer : BurcatObject
    {
        public BurcatIdentifier<IOwnership> Ownership { get; }
        public int Amount { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime TransferDate { get; }

        public OwnershipTransfer(BurcatIdentifier<IOwnership> ownership, int amount, DateTime transferDate) { Ownership = ownership; Amount = amount; TransferDate = transferDate; }
        public OwnershipTransfer(BurcatIdentifier<IOwnership> ownership, int amount) : this(ownership, amount, DateTime.Now) { }

        public override object?[] GetBurcatConstructionValues() => [Ownership, Amount, TransferDate];
    }
}
