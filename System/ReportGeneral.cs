using Burcat.API.Development;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API.System
{
    [BurcatIdentity("208599b5-4de4-4d94-9e85-7c80bb9d9f35")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateReportTarget))]
    public class ReportGeneral : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IReport.IReportable> Accused { get; }
        [Length(32, 8192)]
        public string Description { get; }
        public DateTime SetIn { get; }

        public ReportGeneral(BurcatIdentifier<Member> reporter, BurcatIdentifier<IReport.IReportable> message, string description, DateTime setIn = default) { Reporter = reporter; Accused = message; Description = description; SetIn = setIn == default ? DateTime.Now : setIn; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member) || (DevOps.GetGrade(member) & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;

        public override object?[] GetBurcatConstructionValues() => [Reporter, Accused, Description, SetIn];
    }
}
