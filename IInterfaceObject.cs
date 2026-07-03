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
        bool ShouldCreate(BurcatIdentifier<Member>? member) => false;
        [NotBurcatInvokable]
        bool ShouldSelect(BurcatIdentifier<Member>? member) => false;
        [NotBurcatInvokable]
        bool ShouldManage(BurcatIdentifier<Member>? member);
    }
}
