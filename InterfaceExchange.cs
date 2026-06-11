using Burcat.API.Media;
using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burcat.API
{
    [BurcatIdentity("cb2622ad-4850-4e00-8828-70fa104fd942")]
    public abstract partial class InterfaceExchange : IBurcatObject
    {
        public Guid Identifier { get; set => throw new InvalidOperationException(); } = Guid.Empty;
        public Guid Revision { get; set => throw new InvalidOperationException(); } = Guid.Empty;

        public abstract BurcatList<Faction> DoFactionSearch(Member? member, string? search);

        BurcatField[] IBurcatObject.GetBurcatFields() => [];
        void IBurcatObject.SetBurcatFields(BurcatField[] fields) { }

        IBurcatObject?[] IBurcatObject.GetBurcatConstructionValues() => [];
    }
}
