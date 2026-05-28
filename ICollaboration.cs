using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API
{
    [BurcatUnique(nameof(About), nameof(With))]
    public interface ICollaboration : IInterfaceObject
    {
        BurcatIdentifier<ICollaborative> About { get; }
        BurcatIdentifier<Member> With { get; }

        public interface ICollaborative : IInterfaceObject { }
    }
}