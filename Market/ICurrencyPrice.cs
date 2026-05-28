using Burcat.API.Market.Trading;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Burcat.API.Market
{
    [BurcatUnique(nameof(About), nameof(Currency))]
    public interface ICurrencyPrice : IInterfaceObject
    {
        BurcatIdentifier<IGood> About { get; }
        BurcatIdentifier<Currency> Currency { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        int Amount { get; set; }
    }

    public interface IGood : IInterfaceObject { }
}
