using Burcat.API.Market.Collectionables;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Burcat.API.Development
{
    [BurcatIdentity("b454af1b-4433-4521-ae4a-8510d4e27435")]
    public class Permission : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<DevelopStatus> Creator { get; }
        public string Name { get; }
        public bool Customizable { get; set; }
        public bool CanPost { get; set; }
        public bool CanReadConfidentialData { get; set; }
        public bool CanSpend { get; set; }
        public bool CanMangeIcons { get; set; }
        public bool CanManageAlias { get; set; }
        public bool CanManageAvatars { get; set; }
        public bool CanManageGroups { get; set; }
        public bool CanManageAuctions { get; set; }
        public bool CanManageCards { get; set; }

        public Permission(BurcatIdentifier<DevelopStatus> creator, string name, bool customizable) { Creator = creator; Name = name; Customizable = customizable; }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            else if (obj is Permission other) return this == other;
            else return false;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new();

            hashCode.Add(Customizable);
            hashCode.Add(CanPost);
            hashCode.Add(CanReadConfidentialData);
            hashCode.Add(CanSpend);
            hashCode.Add(CanMangeIcons);
            hashCode.Add(CanManageAlias);
            hashCode.Add(CanManageAvatars);
            hashCode.Add(CanManageGroups);
            hashCode.Add(CanManageAuctions);
            hashCode.Add(CanManageCards);

            return hashCode.ToHashCode();
        }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from d in provider.Get<DevelopStatus>() where d == Creator && d.Member == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Customizable];

        public static bool operator ==(Permission a, Permission b) => a.Customizable == b.Customizable && a.CanPost == b.CanPost && a.CanReadConfidentialData == b.CanReadConfidentialData && a.CanSpend == b.CanSpend && a.CanMangeIcons == b.CanMangeIcons && a.CanManageAlias == b.CanManageAlias && a.CanManageAvatars == b.CanManageAvatars && a.CanManageGroups == b.CanManageGroups && a.CanManageAuctions == b.CanManageAuctions && a.CanManageCards == b.CanManageCards;
        public static bool operator !=(Permission a, Permission b) => !(a == b);
    }

    [BurcatIdentity("1147150e-a57b-4317-b389-3efa6f5ff3fd")]
    [BurcatUnique(nameof(Owner), nameof(Application))]
    public class PermissionGrant : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Member> Owner { get; }
        public BurcatIdentifier<Application> Application { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePermissionGrantPermission))]
        public BurcatIdentifier<Permission> Granted { get; set; }

        public PermissionGrant(BurcatIdentifier<Member> owner, BurcatIdentifier<Application> application, BurcatIdentifier<Permission> granted) { Owner = owner; Application = application; Granted = granted; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;
        public override object?[] GetBurcatConstructionValues() => [Owner, Application, Granted];
    }
}
