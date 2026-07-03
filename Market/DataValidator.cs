﻿using Burcat.API.Development;
using Burcat.API.Market;
using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        public static ValidationResult? ValidateCurrency(Currency currency) => InterfaceOptions.UseProvider(provider => (from d in provider.Get<DevOps>() where d == currency.Creator select d.AvaliableCurrencyCreations).First() > 0 ? ValidationResult.Success : new("The creator of the currency needs to be a developer with avaliable currency creation tokens."));
    }
}
