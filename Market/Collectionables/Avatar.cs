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

namespace Burcat.API.Market.Collectionables
{
    [BurcatIdentity("8955e229-76ac-4d43-94d6-b123dab35302")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    public class Avatar : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Owner { get; }
        [Length(1, 32)]
        public string Name { get; set; }

        public Avatar(BurcatIdentifier<Member> owner, string name) { Owner = owner; Name = name; }

        public ListSet<Collocation> GetCollocations() => [.. from c in InterfaceOptions.GetSingleUse<Collocation>() where c.Owner == this select c];

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Name];
    }

    [BurcatIdentity("dc90dd01-2f06-4f1c-aaa5-d6d5fad39d95")]
    [BurcatUnique(nameof(Association), nameof(Name))]
    public class Item : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<CardLevel> Association { get; }
        [Length(4, 64)]
        public string Name { get; }
        public BurcatIdentifier<Image> Representation { get; set; }
        public string? Description { get; }

        public Item(BurcatIdentifier<CardLevel> association, string name, BurcatIdentifier<Image> representation) { Association = association; Name = name; Representation = representation; }
        public Item(BurcatIdentifier<CardLevel> association, string name, BurcatIdentifier<Image> representation, string description) : this(association, name, representation) { Description = description; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.Showrunner) == DevelopStatusType.Showrunner;
        public override object?[] GetBurcatConstructionValues() => [Association, Name, Representation];
    }

    [BurcatIdentity("1579c49c-fe06-47bb-aaa6-18e234f40798")]
    [BurcatUnique(nameof(Owner), nameof(Item))]
    public class Collocation : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Avatar> Owner { get; }
        public BurcatIdentifier<Item> Item { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int AxisX { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int AxisY { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int AxisZ { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Width { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Height { get; set; }

        public Collocation(BurcatIdentifier<Avatar> owner, BurcatIdentifier<Item> item, int axisX, int axisY, int axisZ, int width, int height) { Owner = owner; Item = item; AxisX = axisX; AxisY = axisY; AxisZ = axisZ; Width = width; Height = height; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (from a in InterfaceOptions.GetSingleUse<Avatar>() where a == Owner select true).Any();
        public override object?[] GetBurcatConstructionValues() => [Owner, Item, AxisX, AxisY, AxisZ, Width, Height];
    }
}
