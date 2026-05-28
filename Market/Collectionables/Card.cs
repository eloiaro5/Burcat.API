using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using static Burcat.API.ICollaboration;

namespace Burcat.API.Market.Collectionables
{
    [BurcatIdentity("36715c43-48ba-4593-a9ff-7fc025d6b54e")]
    [BurcatUnique(nameof(Name))]
    public class Card : BurcatObject, IInterfaceObject
    {
        [Length(4, 64)]
        public string Name { get; }
        public string? Description { get; set; }

        public Card(string name) { Name = name; }
        public Card(string name, string description) : this(name) { Description = description; }

        //public ListSet<Member> GetCollaborations() => [.. from c in InterfaceConfiguration.Provider.GetQueryable<CardCollaboration>(this) where c.Card == this select c.With]);

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Name];
    }

    [BurcatIdentity("6c661e07-5423-4477-92a4-0c59187da5b1")]
    [BurcatUnique(nameof(Card), nameof(Level))]
    public abstract class CardLevel : BurcatObject, IAsset
    {
        public BurcatIdentifier<Card> Card { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Level { get; }
        public BurcatIdentifier<Image> Icon { get; set; }
        public string? Description { get; set; }

        public CardLevel(BurcatIdentifier<Card> card, int level, BurcatIdentifier<Image> icon) { Card = card; Level = level; Icon = icon; }
        public CardLevel(BurcatIdentifier<Card> card, int level, BurcatIdentifier<Image> icon, string description) : this(card, level, icon) { Description = description; }

        //public ListSet<Item> GetItems() => [.. from i in InterfaceConfiguration.Provider.GetQueryable<Item>(this) where i.Association.Card == Card && i.Association.Level <= Level select i]);

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Card, Level, Icon];
    }

    [BurcatIdentity("7858d88c-7a80-44a9-bc40-8ddaa55e9322")]
    [BurcatUnique(nameof(Owner), nameof(Card))]
    public class CardLevelOwnership : BurcatObject, IOwnership
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<CardLevel> Card { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberOverZero))]
        public int Amount { get; set; }

        BurcatIdentifier<IAsset> IOwnership.About => Card.Downcast<IAsset>();

        public CardLevelOwnership(BurcatIdentifier<Member> owner, BurcatIdentifier<CardLevel> cardLevel, int amount) { Owner = owner; Card = cardLevel; Amount = amount; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Card, Amount];
    }

    [BurcatIdentity("cd52c8e4-c52c-4528-b4ab-2152f7087ee3")]
    [BurcatUnique(nameof(Envelope), nameof(CardLevel))]
    public class Probability : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Envelope> Envelope { get; }
        public BurcatIdentifier<CardLevel> CardLevel { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDecimalBetweenZeroAndOne))]
        public decimal Chance { get; set; }

        public Probability(BurcatIdentifier<Envelope> envelope, BurcatIdentifier<CardLevel> cardLevel, decimal chance) { Envelope = envelope; CardLevel = cardLevel; Chance = chance; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Envelope, CardLevel, Chance];
    }


    [BurcatIdentity("793ea249-cebc-4b2f-89ad-34d6ad190fa7")]
    [BurcatUnique(nameof(Card), nameof(With))]
    public class CardCollaboration : BurcatObject, ICollaboration
    {
        public BurcatIdentifier<Card> Card { get; }
        public BurcatIdentifier<Member> With { get; }

        BurcatIdentifier<ICollaborative> ICollaboration.About => Card.Downcast<ICollaborative>();

        public CardCollaboration(BurcatIdentifier<Card> card, BurcatIdentifier<Member> with) { Card = card; With = with; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Card, With];
    }
}
