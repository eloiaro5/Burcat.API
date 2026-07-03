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
    public enum DevOpsGrade : int
    {
        Client = 1,
        Provider = 2,
        Moderator = 4,
        Showrunner = 8,
        System = 16
    }

    [BurcatIdentity("8e8e6a23-1262-433e-b946-331ec4f0410e")]
    [BurcatUnique(nameof(Member))]
    public class DevOps : BurcatObject, IInterfaceObject
    {
        public static DevOpsGrade GetGrade(BurcatIdentifier<Member>? member) => member is null ? 0 : InterfaceOptions.UseProvider(provider => (from d in provider.Get<DevOps>() where d.Member == member select d.Type).FirstOrDefault());

        public BurcatIdentifier<Member> Member { get; }
        public DevOpsGrade Type { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int Priority { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int QueriesPerSecond { get; set; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateNumberEqualOrOverZero))]
        public int AvaliableCurrencyCreations { get; set; }

        public DevOps(BurcatIdentifier<Member> member, DevOpsGrade type) { Member = member; Type = type; }

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && GetGrade(memberID) is DevOpsGrade type && (type & DevOpsGrade.System) == DevOpsGrade.System;
        public override object?[] GetBurcatConstructionValues() => [Member, Type];
    }
}
