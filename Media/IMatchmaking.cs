using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Media
{
    [BurcatUnique(nameof(Match), nameof(Advertisement))]
    public interface IMatchmaking : IInterfaceObject
    {
        BurcatIdentifier<IPublisher> Match { get; }
        BurcatIdentifier<MessageAdvertisement> Advertisement { get; }
        bool IsInclusive { get; set; }
    }

    public interface IPublisher : IInterfaceObject { }
}
