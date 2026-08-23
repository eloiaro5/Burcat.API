using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API
{
    [BurcatIdentity("ec442cda-a779-461d-914a-a97f3ab24a61")]
    public enum DisturbingThemesPoliteness
    {
        [Description("Contains no distressing themes or emotionally intense material.")] NoDisturbingThemes = 0,
        [Description("Contains brief fear, grief, or interpersonal conflict.")] MildFearGriefOrConflict = 1,
        [Description("Sustains themes of horror, abuse, death, or severe distress.")] SustainedHorrorAbuseOrDeathThemes = 2,
        [Description("Discusses self-harm, suicide, or abuse in significant detail.")] DetailedSelfHarmSuicideOrAbuseThemes = 3,
        [Description("Graphically depicts or glamorizes deeply disturbing themes.")] GraphicOrGlamorizedDisturbingThemes = 4
    }

    [BurcatIdentity("efcbf76f-985e-471f-9435-a5dba5a7f88f")]
    public enum GamblingPoliteness
    {
        [Description("Contains no gambling references or mechanics.")] NoGambling = 0,
        [Description("Briefly references gambling or uses simulated chance mechanics.")] CasualReferencesOrSimulatedGambling = 1,
        [Description("Shows real-money or equivalent gambling without emphasis.")] DepictedGambling = 2,
        [Description("Frequently depicts or positively portrays gambling.")] FrequentOrGlamorizedGambling = 3,
        [Description("Focuses on addiction, severe losses, or gambling-related harm.")] GraphicProblemGamblingThemes = 4
    }

    [BurcatIdentity("af700ecb-d1bf-4444-b208-4dd11b839147")]
    public enum LanguagePoliteness
    {
        [Description("Contains no profanity or degrading language.")] ZeroProfanity = 0,
        [Description("Contains isolated mild swear words.")] OccasionalMildProfanity = 1,
        [Description("Uses mild swear words frequently.")] RepeatedMildProfanity = 2,
        [Description("Contains isolated strong swear words.")] OccasionalStrongProfanity = 3,
        [Description("Uses strong swear words frequently.")] RepeatedStrongProfanity = 4,
        [Description("Uses sustained insults, humiliation, or degrading language.")] AggressiveOrDegradingLanguage = 5
    }

    [BurcatIdentity("9682bf8f-e6c8-4774-88e0-ad1ac6e4a6f7")]
    public enum RacismPoliteness
    {
        [Description("Contains no hateful, prejudiced, or discriminatory material.")] NoHateOrDiscrimination = 0,
        [Description("Contains isolated stereotypes or insensitive remarks.")] MildStereotypesOrInsensitiveLanguage = 1,
        [Description("Repeatedly expresses prejudice or portrays groups as lesser.")] RepeatedPrejudiceOrDehumanizingLanguage = 2,
        [Description("Openly promotes hostility or discrimination against a group.")] ExplicitHateOrDiscrimination = 3,
        [Description("Celebrates or strongly endorses hateful or discriminatory behavior.")] GlorificationOfHateOrDiscrimination = 4
    }

    [BurcatIdentity("437f77b6-7da1-43d6-a883-ef10402e5428")]
    public enum SexualityPoliteness
    {
        [Description("Contains no sexual references, suggestiveness, or nudity.")] NoSexualContent = 0,
        [Description("Contains romance, flirting, or mild sexual implications.")] RomanceOrMildSuggestiveness = 1,
        [Description("Contains strong suggestiveness or non-explicit nudity.")] StrongSuggestivenessOrNonExplicitNudity = 2,
        [Description("Contains explicit sexual descriptions or full nudity.")] ExplicitSexualLanguageOrNudity = 3,
        [Description("Graphically depicts consensual sexual activity between adults.")] GraphicConsensualAdultSexualContent = 4
    }

    [BurcatIdentity("2785f52a-48d0-4aec-8c7c-6381ea97cf15")]
    public enum SubstanceUsePoliteness
    {
        [Description("Contains no references to alcohol, tobacco, or recreational drugs.")] NoSubstanceUse = 0,
        [Description("Mentions substances without showing their use.")] ReferencesToAlcoholTobaccoOrDrugs = 1,
        [Description("Shows recreational substance use without emphasis or praise.")] DepictedRecreationalUse = 2,
        [Description("Frequently depicts or positively portrays substance use.")] FrequentOrGlamorizedUse = 3,
        [Description("Graphically portrays addiction, withdrawal, or overdose.")] GraphicAddictionOrOverdoseThemes = 4
    }

    [BurcatIdentity("15b4d9da-877e-4af8-ae2d-f5521e16e285")]
    public enum ViolencePoliteness
    {
        [Description("Contains no violence, threats, or visible injury.")] NoViolence = 0,
        [Description("Contains mild threats or unrealistic violence without injury detail.")] MildThreatsOrCartoonViolence = 1,
        [Description("Depicts realistic violence without graphic injury detail.")] NonGraphicRealisticViolence = 2,
        [Description("Shows detailed injury, blood, or violent acts.")] GraphicViolenceOrInjury = 3,
        [Description("Focuses on extreme gore, torture, or prolonged cruelty.")] ExtremeGoreOrCruelty = 4
    }

    [BurcatIdentity("1a314713-3676-400b-83bd-97930eb22f17")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    public class Politeness : BurcatObject, IInterfaceObject, IComparable<Politeness>
    {
        public static Politeness GetNewMinimum(BurcatIdentifier<Member> owner, string? name = null)
        {
            Politeness politeness = new(owner, name ?? string.Empty, LanguagePoliteness.ZeroProfanity, RacismPoliteness.NoHateOrDiscrimination, SexualityPoliteness.NoSexualContent, ViolencePoliteness.NoViolence, SubstanceUsePoliteness.NoSubstanceUse, GamblingPoliteness.NoGambling, DisturbingThemesPoliteness.NoDisturbingThemes);
            return politeness;
        }
        public static Politeness GetNewAverage(BurcatIdentifier<Member> owner, string? name = null)
        {
            Politeness politeness = new(owner, name ?? string.Empty, LanguagePoliteness.OccasionalStrongProfanity, RacismPoliteness.RepeatedPrejudiceOrDehumanizingLanguage, SexualityPoliteness.StrongSuggestivenessOrNonExplicitNudity, ViolencePoliteness.NonGraphicRealisticViolence, SubstanceUsePoliteness.DepictedRecreationalUse, GamblingPoliteness.DepictedGambling, DisturbingThemesPoliteness.SustainedHorrorAbuseOrDeathThemes);
            return politeness;
        }
        public static Politeness GetNewMaximum(BurcatIdentifier<Member> owner, string? name = null)
        {
            Politeness politeness = new(owner, name ?? string.Empty, LanguagePoliteness.AggressiveOrDegradingLanguage, RacismPoliteness.GlorificationOfHateOrDiscrimination, SexualityPoliteness.GraphicConsensualAdultSexualContent, ViolencePoliteness.ExtremeGoreOrCruelty, SubstanceUsePoliteness.GraphicAddictionOrOverdoseThemes, GamblingPoliteness.GraphicProblemGamblingThemes, DisturbingThemesPoliteness.GraphicOrGlamorizedDisturbingThemes);
            return politeness;
        }

        public static Politeness? GetByName(BurcatIdentifier<Member> owner, string name) => InterfaceOptions.UseProvider(provider => (from p in provider.Get<Politeness>() where p.Owner == owner && p.Name == name select p).FirstOrDefault());

        public BurcatIdentifier<Member> Owner { get; }
        [Length(1, 64)]
        public string Name { get; set; }
        public LanguagePoliteness Language { get; set; }
        public RacismPoliteness Racism { get; set; }
        public SexualityPoliteness Sexuality { get; set; }
        public ViolencePoliteness Violence { get; set; }
        public SubstanceUsePoliteness SubstanceUse { get; set; }
        public GamblingPoliteness Gambling { get; set; }
        public DisturbingThemesPoliteness DisturbingThemes { get; set; }
        public bool IsHided { get; set; }

        public Politeness(BurcatIdentifier<Member> owner, string name, LanguagePoliteness language, RacismPoliteness racism, SexualityPoliteness sexuality, ViolencePoliteness violence, SubstanceUsePoliteness substanceUse, GamblingPoliteness gambling, DisturbingThemesPoliteness disturbingThemes) { Owner = owner; Name = string.IsNullOrWhiteSpace(name) ? Identifier.ToString().Replace("-", null) : name; Language = language; Racism = racism; Sexuality = sexuality; Violence = violence; SubstanceUse = substanceUse; Gambling = gambling; DisturbingThemes = disturbingThemes; }

        /// <summary>Hides this politeness while preserving it for existing references.</summary>
        public BurcatException? Hide()
        {
            if (IsHided) return new("The politeness is already hidden.");

            Name = Identifier.ToString();
            IsHided = true;
            Revision = GuidExtensions.GenerateRandom();
            return (BurcatException?)null;//return BurcatChat.RelayCouple(this);
        }

        public int CompareTo(Politeness? other)
        {
            if (this == other) return 0;
            else if (this < other) return -1;
            else return 1;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            else if (obj is Politeness other) return this == other;
            else return false;
        }
        public override int GetHashCode() => HashCode.Combine(Language, Racism, Sexuality, Violence, SubstanceUse, Gambling, DisturbingThemes);

        public bool ShouldCreate(BurcatIdentifier<Member>? member) =>ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => false;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Language, Racism, Sexuality, Violence, SubstanceUse, Gambling, DisturbingThemes];

        public static bool operator ==(Politeness? a, Politeness? b) => (a is null && b is null) || (a is not null && b is not null && a.Language == b.Language && a.Racism == b.Racism && a.Sexuality == b.Sexuality && a.Violence == b.Violence && a.SubstanceUse == b.SubstanceUse && a.Gambling == b.Gambling && a.DisturbingThemes == b.DisturbingThemes);
        public static bool operator !=(Politeness? a, Politeness? b) => !(a == b);
        public static bool operator >(Politeness? a, Politeness? b) => a != b && a >= b;
        public static bool operator <(Politeness? a, Politeness? b) => a != b && a <= b;
        public static bool operator >=(Politeness? a, Politeness? b) => b <= a;
        public static bool operator <=(Politeness? a, Politeness? b) => a == b || b is null || (a is not null && a.Language <= b.Language && a.Racism <= b.Racism && a.Sexuality <= b.Sexuality && a.Violence <= b.Violence && a.SubstanceUse <= b.SubstanceUse && a.Gambling <= b.Gambling && a.DisturbingThemes <= b.DisturbingThemes);
    }
}
