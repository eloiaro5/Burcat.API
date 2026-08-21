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
        public BurcatIdentifier<Application> Application { get; }
        public Guid BurcatClass { get; }
        public bool SelectOnly { get; }
        public bool IsOptional { get; }

        public Permission(BurcatIdentifier<Application> application, Guid burcatClass, bool selectOnly, bool isOptional) { Application = application; BurcatClass = burcatClass; SelectOnly = selectOnly; IsOptional = isOptional; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is BurcatIdentifier<Member> memberID && InterfaceOptions.UseProvider(provider => (from a in provider.Get<Application>() join d in provider.Get<DevOps>() on memberID equals d.Member where a.Identifier == (Guid)Application select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Application, BurcatClass, SelectOnly];
    }

    [BurcatIdentity("1147150e-a57b-4317-b389-3efa6f5ff3fd")]
    [BurcatUnique(nameof(Owner), nameof(Permission))]
    public class PermissionGrant : BurcatObject, IInterfaceObject
    {
        public static bool HasSelectGrant(BurcatIdentifier<Member> ownerMember, BurcatIdentifier<Member> foreignMember, IInterfaceObject interfaceObject)
        {
            Guid classID = BurcatChat.GetClassIdentity(interfaceObject);
            return InterfaceOptions.UseProvider(provider => (
                from g in provider.Get<PermissionGrant>()
                join p in provider.Get<Permission>() on (Guid)g.Permission equals p.Identifier
                join a in provider.Get<Application>() on (Guid)p.Application equals a.Identifier
                join da in provider.Get<DevOps>() on (Guid)a.Owner equals da.Identifier
                join c in provider.Get<ApplicationCollaboration>() on p.Identifier equals (Guid)c.Application
                join dc in provider.Get<DevOps>() on (Guid)c.With equals dc.Identifier
                where
                    g.Owner == ownerMember &&
                    p.BurcatClass == classID &&
                    (da.Identifier == (Guid)foreignMember || dc.Identifier == (Guid)foreignMember)
                select true).Any()
            );
        }

        public static bool HasManageGrant(BurcatIdentifier<Member> ownerMember, BurcatIdentifier<Member> foreignMember, IInterfaceObject interfaceObject)
        {
            Guid classID = BurcatChat.GetClassIdentity(interfaceObject);
            return InterfaceOptions.UseProvider(provider => (
                from g in provider.Get<PermissionGrant>()
                join p in provider.Get<Permission>() on (Guid)g.Permission equals p.Identifier
                join a in provider.Get<Application>() on (Guid)p.Application equals a.Identifier
                join da in provider.Get<DevOps>() on (Guid)a.Owner equals da.Identifier
                join c in provider.Get<ApplicationCollaboration>() on p.Identifier equals (Guid)c.Application
                join dc in provider.Get<DevOps>() on (Guid)c.With equals dc.Identifier
                where
                    g.Owner == ownerMember &&
                    p.BurcatClass == classID && p.SelectOnly == false &&
                    (da.Member == foreignMember || dc.Member == foreignMember)
                select true).Any()
            );
        }

        public BurcatIdentifier<Member> Owner { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePermissionGrantPermission))]
        public BurcatIdentifier<Permission> Permission { get; set; }

        public PermissionGrant(BurcatIdentifier<Member> owner, BurcatIdentifier<Permission> permission) { Owner = owner; Permission = permission; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Owner == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Permission];
    }
}
