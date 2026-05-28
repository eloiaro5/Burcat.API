using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        public static ValidationResult? ValidateDateEqualOrOverNow(DateTime date) => date < DateTime.Now ? new($"The date must be equal or over {nameof(DateTime.Now)}.") : ValidationResult.Success;
        public static ValidationResult? ValidateDateDefaultOrEqualOrOverNow(DateTime date) => date != default && date < DateTime.Now ? new($"The date must be the default value, or equal or over {nameof(DateTime.Now)}.") : ValidationResult.Success;
        public static ValidationResult? ValidateNullDateEqualOrOverNow(DateTime? date) => date is not null && date < DateTime.Now ? new($"The date must be null or, equal or over {nameof(DateTime.Now)}.") : ValidationResult.Success;
        public static ValidationResult? ValidateDateEqualOrUnderNow(DateTime date) => date > DateTime.Now ? new($"The date must be equal or under {nameof(DateTime.Now)}.") : ValidationResult.Success;
        public static ValidationResult? ValidateDateDefaultOrEqualOrUnderNow(DateTime date) => date != default && date > DateTime.Now ? new($"The date must be default value, or equal or under {nameof(DateTime.Now)}.") : ValidationResult.Success;
        public static ValidationResult? ValidateNullDateEqualOrUnderNow(DateTime? date) => date is not null && date > DateTime.Now ? new($"The date must be null or, equal or under {nameof(DateTime.Now)}.") : ValidationResult.Success;

        public static ValidationResult? ValidateNumberEqualOrOverZero(decimal value) => value < 0 ? new($"The number must be equal or over 0.") : ValidationResult.Success;
        public static ValidationResult? ValidateNullNumberEqualOrOverZero(decimal? value) => value is not null && value < 0 ? new($"The number must be null or, equal or over 0.") : ValidationResult.Success;
        public static ValidationResult? ValidateNumberOverZero(decimal value) => value < 0 ? new($"The number must be equal or over 0.") : ValidationResult.Success;
        public static ValidationResult? ValidateNullNumberOverZero(decimal? value) => value is not null && value < 0 ? new($"The number must be null or, equal or over 0.") : ValidationResult.Success;

        public static ValidationResult? ValidateDecimalBetweenZeroAndOne(decimal value) => value < 0 || value > 1 ? new($"The decimal must be between 0 and 1.") : ValidationResult.Success;
        public static ValidationResult? ValidateNullDecimalBetweenZeroAndOne(decimal? value) => value is not null && (value < 0 || value > 1) ? new($"The decimal must be null or between 0 and 1.") : ValidationResult.Success;

        public static ValidationResult? ValidatePseudonym(Pseudonym pseudonym)
        {
            Politeness politeness = InterfaceOptions.Find(pseudonym.Politeness);
            int expected = InterfaceOptions.TryFind<Pseudonym>(pseudonym) is null ? 0 : 1;
            return (from pt in InterfaceOptions.GetSingleUse<Pseudonym>() join p in InterfaceOptions.GetSingleUse<Politeness>() on (Guid)pt.Politeness equals p.Identifier where pseudonym.Owner == p.Owner select p)
            .AsEnumerable().Count(p => p == politeness) > expected
            ? new("Only can exist one pseudonym with each politness combination.") : ValidationResult.Success;
        }

        public static ValidationResult? ValidateIconography(IIconography icon)
        {
            Politeness politeness = InterfaceOptions.Find(InterfaceOptions.Find(icon.Icon).Politeness);
            int expected = InterfaceOptions.TryFind<IIconography>(new(icon.Identifier)) is null ? 0 : 1;
            return (from ic in InterfaceOptions.GetSingleUse<IIconography>() join i in InterfaceOptions.GetSingleUse<Image>() on (Guid)ic.Icon equals i.Identifier join p in InterfaceOptions.GetSingleUse<Politeness>() on (Guid)i.Politeness equals p.Identifier where (Guid)icon.Owner == (Guid)p.Owner select p)
            .AsEnumerable().Count(p => p == politeness) > expected
            ? new("Only can exist one icon with each politness combination.") : ValidationResult.Success;
        }
        public static ValidationResult? ValidateImageData(byte[] data)
        {
            bool isPNG = data[0] == (byte)0x89 && data[1] == (byte)0x50 && data[2] == (byte)0x4E && data[3] == (byte)0x47;
            int width = (data[16] << 24) | (data[17] << 16) | (data[18] << 8) | data[19];
            int height = (data[20] << 24) | (data[21] << 16) | (data[22] << 8) | data[23];

            return isPNG && width <= 1024 && height <= 1024 ? ValidationResult.Success : new("Images must be a PNG with a maximum dimensions of 1024x1024.");
        }
    }
}
