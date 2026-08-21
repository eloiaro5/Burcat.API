using Burcat.API.Market;
using Burcat.API.Market.Collectionables;
using Burcat.API.Market.Trading;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using static Burcat.API.ICollaboration;

namespace Burcat.API.Development
{
    [BurcatIdentity("f05a5af4-0b66-4450-b712-3c44442c88c1")]
    [BurcatUnique(nameof(Owner), nameof(Name))]
    public class Application : BurcatObject, ICollaborative
    {
        public BurcatIdentifier<DevOps> Owner { get; }
        [Length(4, 64)]
        public string Name { get; set; }
        public BurcatIdentifier<Image>? Icon { get; set; }
        [Length(32, 8192)]
        public string Description { get; set; }

        public Application(BurcatIdentifier<DevOps> owner, string name, BurcatIdentifier<Image>? icon, string description) { Owner = owner; Name = name; Icon = icon; Description = description; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => InterfaceOptions.Find(Owner).Member == member;

        public override object?[] GetBurcatConstructionValues() => [Owner, Name, Icon, Description];
    }

    [BurcatIdentity("c89b0379-b24a-4a56-aa92-257b39eb934f")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateApplicationCollaboration))]
    [BurcatUnique(nameof(Application), nameof(With))]
    public class ApplicationCollaboration : BurcatObject, ICollaboration
    {
        public BurcatIdentifier<Application> Application { get; }
        public BurcatIdentifier<DevOps> With { get; }

        BurcatIdentifier<Member> ICollaboration.With => InterfaceOptions.UseProvider(provider => (from d in provider.Get<DevOps>() where d == With select d.Member).First());
        BurcatIdentifier<ICollaborative> ICollaboration.About => Application.Downcast<ICollaborative>();

        public ApplicationCollaboration(BurcatIdentifier<Application> application, BurcatIdentifier<DevOps> with) { Application = application; With = with; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from a in provider.Get<Application>() join d in provider.Get<DevOps>() on (Guid)a.Owner equals d.Identifier where a == Application && d.Member == member select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Application, With];
    }
}
