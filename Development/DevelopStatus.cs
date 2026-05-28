using Burcat.API.Market.Collectionables;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Development
{
    [BurcatIdentity("f42879bd-c3d3-4015-8a61-a8a256c92cae")]
    [Flags]
    public enum DevelopStatusType : int
    {
        Client = 1,
        Provider = 2,
        Moderator = 4,
        Showrunner = 8,
        System = 16
    }

    [BurcatIdentity("8e8e6a23-1262-433e-b946-331ec4f0410e")]
    [BurcatUnique(nameof(Member))]
    public class DevelopStatus : BurcatObject, IInterfaceObject
    {
        public static DevelopStatusType? GetDevelopType(BurcatIdentifier<Member> member) => (from d in InterfaceOptions.GetSingleUse<DevelopStatus>() where d.Member == member select d.Type).FirstOrDefault();

        public BurcatIdentifier<Member> Member { get; }
        public DevelopStatusType Type { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Priority { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int QueriesPerSecond { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int AvaliableCurrencyCreations { get; set; }

        public DevelopStatus(BurcatIdentifier<Member> member, DevelopStatusType type) { Member = member; Type = type; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.System) == DevelopStatusType.System;
        public override object?[] GetBurcatConstructionValues() => [Member, Type];
    }
}
