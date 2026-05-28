using Burcat.API.Development;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace Burcat.API.System
{
    [BurcatUnique(nameof(Post), nameof(Reporter))]
    public interface IReport : IInterfaceObject
    {
        BurcatIdentifier<IPost> Post { get; }
        BurcatIdentifier<Member> Reporter { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrOverNow))]
        DateTime SetIn { get; }
    }

    [BurcatIdentity("2054690c-420a-4bd2-ad7b-0a9cc4cb2a79")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePolitenessReport))]
    public class PolitenessReport : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IPost> Post { get; }
        public BurcatIdentifier<Politeness> ExpectedPoliteness { get; }
        public DateTime SetIn { get; }

        public PolitenessReport(BurcatIdentifier<Member> reporter, BurcatIdentifier<IPost> post, BurcatIdentifier<Politeness> expectedPoliteness, DateTime? setIn) { Reporter = reporter; Post = post; ExpectedPoliteness = expectedPoliteness; SetIn = setIn ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;
        public override object?[] GetBurcatConstructionValues() => [Reporter, Post, ExpectedPoliteness, SetIn];
    }

    [BurcatIdentity("231a8cf7-f56f-44e1-86c4-6090e196c120")]
    public class CopyrightReport : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IPost> Post { get; }
        public string InfringementDescription { get; }
        public string URL { get; }
        public DateTime SetIn { get; }

        public CopyrightReport(BurcatIdentifier<Member> reporter, BurcatIdentifier<IPost> post, string infringementDescription, string url, DateTime? setIn) { Reporter = reporter; Post = post; InfringementDescription = infringementDescription; URL = url; SetIn = setIn ?? DateTime.Now; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;
        public override object?[] GetBurcatConstructionValues() => [Reporter, Post, InfringementDescription, URL, SetIn];
    }
}
