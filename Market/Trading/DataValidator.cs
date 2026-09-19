﻿using Burcat.API.Development;
using Burcat.API.Market;
using Burcat.API.Market.Trading;
using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        public static ValidationResult? ValidateBid(Bid bid)
        {
            return InterfaceOptions.UseProvider(provider =>
            {
                decimal? auctionParticipationPrice = (from a in provider.Get<Auction>() where a == bid.MadeTo select a.ParticipationPrice).First();
                decimal memberLety = (from a in provider.Get<Member>() where a == bid.MadeBy select a.Lety).First();

                if (bid.Amount > memberLety) return new ValidationResult("Cannot create a bid that will leave you under 0.");
                else if (auctionParticipationPrice is decimal pp && bid.Amount < pp) return new ValidationResult("Cannot create a bid lower than the participation price.");
                else return ValidationResult.Success;
            });
        }

        public static ValidationResult? ValidateCurrencyBid(CurrencyBid bid)
        {
            return InterfaceOptions.UseProvider(provider =>
            {
                int currencyOwned = (from o in provider.Get<CurrencyOwnership>() where o.Owner == bid.MadeBy select o.Amount).FirstOrDefault();
                CurrencyParticipationPrice? participationPrice = (from pp in provider.Get<CurrencyParticipationPrice>() where pp.Auction == bid.MadeTo select pp).FirstOrDefault();

                if (bid.Amount > currencyOwned) return new ValidationResult("Cannot create a bid that will leave your currency ownership under 0.");
                else if (participationPrice is CurrencyParticipationPrice pp && bid.Amount < pp.Amount) return new ValidationResult("Cannot create a bid lower than the participation price.");
                else return ValidationResult.Success;
            });
        }

        public static ValidationResult? ValidateAuction(Auction auction)
        {
            return InterfaceOptions.UseProvider(provider =>
            {
                int amountOwned = (from o in provider.Get<IOwnership>() where o.Identifier == auction.MadeOver.Value select o.Amount).FirstOrDefault();

                return auction.Amount > amountOwned ? new ValidationResult("The amount auctioned cannot be over the amount owned.") : ValidationResult.Success;
            });
        }
    }
}
