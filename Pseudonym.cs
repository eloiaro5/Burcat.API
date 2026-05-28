using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API
{
    [BurcatIdentity("182bae4c-157a-447f-9bc8-a2fa6e3816a4")]
    [BurcatUnique(nameof(Owner), nameof(Value))]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePseudonym))]
    public class Pseudonym : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Owner { get; }
        [Length(4, 32)]
        public string Value { get; set; }
        public BurcatIdentifier<Politeness> Politeness { get; set; }

        public Pseudonym(BurcatIdentifier<Member> owner, string value, BurcatIdentifier<Politeness> politeness) { Owner = owner; Value = value; Politeness = politeness; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Value, Politeness];
    }
}
