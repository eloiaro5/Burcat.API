using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burcat.API
{
    public interface IInterfaceObject : IBurcatObject
    {
        [NotBurcatInvokable]
        bool ShouldManage(BurcatIdentifier<Member>? member);
    }
}
