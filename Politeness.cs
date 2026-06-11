using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API
{
    [BurcatIdentity("af700ecb-d1bf-4444-b208-4dd11b839147")]
    public enum LanguagePoliteness
    {
        ZeroProfanity = 0,
        OcassionalMildProfanity = 1, //Damn, shit, crap,...
        RepeatedMildProfanity = 2,
        OcassionalClearProfanity = 3, //Fuck, Bitch, Hore,...
        RepeatedClearProfanity = 4,
        OcassionalSlurSpeech = 5, //Nigger,...
        RepeatedSlurSpeech = 6
    }

    [BurcatIdentity("9682bf8f-e6c8-4774-88e0-ad1ac6e4a6f7")]
    public enum RacismPoliteness
    {
        ZeroRacismReference = 0,
        MildSterotypingOrCodedLanguage = 1,
        DirectSterotypingWithoutThreats = 2,
        DirectSterotypingWithThreats = 3,
        ExplicitHateSpeech = 4,
        ExtremeRacism = 5
    }

    [BurcatIdentity("437f77b6-7da1-43d6-a883-ef10402e5428")]
    public enum SexualityPoliteness
    {
        ZeroSexualConnotation = 0,
        CasualSexualWordsOrSuggestiveImagery = 1,
        ExplicitSexualLanguageOrPartialNudity = 2,
        PornographicContent = 3,
    }

    [BurcatIdentity("15b4d9da-877e-4af8-ae2d-f5521e16e285")]
    public enum ViolencePoliteness
    {
        ZeroViolence = 0,
        MildOrImpliedViolence = 1,
        ExplicitMildViolenceOrImpliedViolence = 2,
        ExplicitViolenceOrImpliedeExtremeViolence = 3,
        ExtremeViolence = 4
    }

    [BurcatIdentity("1a314713-3676-400b-83bd-97930eb22f17")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    public class Politeness : BurcatObject, IInterfaceObject, IComparable<Politeness>
    {
        public static Politeness GetNewMinimum(BurcatIdentifier<Member> owner, string? name = null)
        {
            Politeness politeness = new(owner, name ?? string.Empty, LanguagePoliteness.ZeroProfanity, RacismPoliteness.ZeroRacismReference, SexualityPoliteness.ZeroSexualConnotation, ViolencePoliteness.ZeroViolence);
            return politeness;
        }
        public static Politeness GetNewAverage(BurcatIdentifier<Member> owner, string? name = null)
        {
            Politeness politeness = new(owner, name ?? string.Empty, LanguagePoliteness.OcassionalClearProfanity, RacismPoliteness.DirectSterotypingWithoutThreats, SexualityPoliteness.CasualSexualWordsOrSuggestiveImagery, ViolencePoliteness.ExplicitMildViolenceOrImpliedViolence);
            return politeness;
        }
        public static Politeness GetNewMaximum(BurcatIdentifier<Member> owner, string? name = null)
        {
            Politeness politeness = new(owner, name ?? string.Empty, LanguagePoliteness.RepeatedSlurSpeech, RacismPoliteness.ExtremeRacism, SexualityPoliteness.PornographicContent, ViolencePoliteness.ExtremeViolence);
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
        public bool IsHided { get; set; }

        public Politeness(BurcatIdentifier<Member> owner, string name, LanguagePoliteness language, RacismPoliteness racism, SexualityPoliteness sexuality, ViolencePoliteness violence) { Owner = owner; Name = string.IsNullOrWhiteSpace(name) ? Identifier.ToString().Replace("-", null) : name; Language = language; Racism = racism; Sexuality = sexuality; Violence = violence; }

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
        public override int GetHashCode() => HashCode.Combine(Language, Racism, Sexuality, Violence);

        bool IInterfaceObject.ShouldCreate(BurcatIdentifier<Member>? member) => ((IInterfaceObject)this).ShouldManage(member);
        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Language, Racism, Sexuality, Violence];

        public static bool operator ==(Politeness? a, Politeness? b) => (a is null && b is null) || (a is not null && b is not null && a.Language == b.Language && a.Racism == b.Racism && a.Sexuality == b.Sexuality && a.Violence == b.Violence);
        public static bool operator !=(Politeness? a, Politeness? b) => !(a == b);
        public static bool operator >(Politeness? a, Politeness? b) => a != b && a >= b;
        public static bool operator <(Politeness? a, Politeness? b) => a != b && a <= b;
        public static bool operator >=(Politeness? a, Politeness? b) => a == b || !(a < b);
        public static bool operator <=(Politeness? a, Politeness? b) => a == b || b is null || (a is not null && b is not null && (a.Language <= b.Language || a.Racism <= b.Racism || a.Sexuality <= b.Sexuality || a.Violence <= b.Violence));
    }
}
