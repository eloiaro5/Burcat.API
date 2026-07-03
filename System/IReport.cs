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

namespace Burcat.API.System
{
    [BurcatUnique(nameof(Message), nameof(Reporter))]
    public interface IReport : IInterfaceObject
    {
        BurcatIdentifier<IMessage> Message { get; }
        BurcatIdentifier<Member> Reporter { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrOverNow))]
        DateTime SetIn { get; }
    }

    [BurcatIdentity("208599b5-4de4-4d94-9e85-7c80bb9d9f35")]
    public class GeneralReport : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IMessage> Message { get; }
        [Length(32, 8192)]
        public string Description { get; }
        public DateTime SetIn { get; }

        public GeneralReport(BurcatIdentifier<Member> reporter, BurcatIdentifier<IMessage> message, string description, DateTime setIn = default) { Reporter = reporter; Message = message; Description = description; SetIn = setIn == default ? DateTime.Now : setIn; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member) || (DevOps.GetGrade(member) & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;

        public override object?[] GetBurcatConstructionValues() => [Reporter, Message, Description, SetIn];
    }

    [BurcatIdentity("2054690c-420a-4bd2-ad7b-0a9cc4cb2a79")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePolitenessReport))]
    public class PolitenessReport : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IMessage> Message { get; }
        public BurcatIdentifier<Politeness> ExpectedPoliteness { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime SetIn { get; }

        public PolitenessReport(BurcatIdentifier<Member> reporter, BurcatIdentifier<IMessage> message, BurcatIdentifier<Politeness> expectedPoliteness, DateTime setIn = default) { Reporter = reporter; Message = message; ExpectedPoliteness = expectedPoliteness; SetIn = setIn == default ? DateTime.Now : setIn; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member) || (DevOps.GetGrade(member) & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;

        public override object?[] GetBurcatConstructionValues() => [Reporter, Message, ExpectedPoliteness, SetIn];
    }

    [BurcatIdentity("231a8cf7-f56f-44e1-86c4-6090e196c120")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateCopyrightReport))]
    public class CopyrightReport : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IMessage> Message { get; }
        [Length(32, 8192)]
        public string InfringementDescription { get; }
        public string URL { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime SetIn { get; }

        public CopyrightReport(BurcatIdentifier<Member> reporter, BurcatIdentifier<IMessage> message, string infringementDescription, string url, DateTime setIn = default) { Reporter = reporter; Message = message; InfringementDescription = infringementDescription; URL = url; SetIn = setIn == default ? DateTime.Now : setIn; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member) || (DevOps.GetGrade(member) & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;

        public override object?[] GetBurcatConstructionValues() => [Reporter, Message, InfringementDescription, URL, SetIn];
    }
}
