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
    public class Application : BurcatObject, ICollaborative
    {
        public BurcatIdentifier<DevOps> Creator { get; }
        [Length(4, 64)]
        public string Name { get; set; }
        public BurcatIdentifier<Image> Icon { get; set; }
        public BurcatIdentifier<Permission> Requirements { get; set; }
        public string? Description { get; set; }

        public Application(BurcatIdentifier<DevOps> creator, string name, BurcatIdentifier<Image> icon, BurcatIdentifier<Permission> requirements) { Creator = creator; Name = name; Icon = icon; Requirements = requirements; }
        public Application(BurcatIdentifier<DevOps> creator, string name, BurcatIdentifier<Image> icon, BurcatIdentifier<Permission> requirements, string description) : this(creator, name, icon, requirements) { Description = description; }

        //public ListSet<Development> GetDevelopers() => [.. from c in InterfaceConfiguration.Provider.GetQueryable<ApplicationCollaboration>(this) where c.Application == this select c.With]);

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from d in provider.Get<DevOps>() where d == Creator && d.Member == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Creator, Name, Icon, Requirements];
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

        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from a in provider.Get<Application>() join d in provider.Get<DevOps>() on (Guid)a.Creator equals d.Identifier where a == Application && d.Member == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Application, With];
    }
}
