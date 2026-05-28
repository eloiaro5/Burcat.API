using BurcatProtocol;
using BurcatProtocol.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API
{
    [BurcatUnique(nameof(Owner), nameof(Icon))]
    [BurcatUnique(nameof(Owner), nameof(Icon), nameof(Name))]
    public interface IIconography : IInterfaceObject
    {
        BurcatIdentifier<IProfile> Owner { get; }
        BurcatIdentifier<Image> Icon { get; }
        [Length(1, 32)]
        string Name { get; set; }
    }
    public interface IProfile : IInterfaceObject { }
}
