using Burcat.API.Development;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace Burcat.API.System
{
    [BurcatIdentity("860d6c84-e3ba-412e-a708-01a08ffd75c0")]
    [BurcatUnique(nameof(Name))]
    public class Country : BurcatObject, IInterfaceObject
    {
        [Length(1, 64)]
        public string Name { get; }
        [Length(2, 2)]
        public string ISO2 { get; set; }
        [Length(3, 3)]
        public string ISO3 { get; set; }

        public Country(string name) { Name = name; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && DevelopStatus.GetDevelopType(memberID) is DevelopStatusType type && (type & DevelopStatusType.System) == DevelopStatusType.System;
        public override object?[] GetBurcatConstructionValues() => [Name];
    }
}
