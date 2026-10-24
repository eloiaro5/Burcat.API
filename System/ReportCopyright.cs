using Burcat.API.Development;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Burcat.API.System
{
    [BurcatIdentity("231a8cf7-f56f-44e1-86c4-6090e196c120")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateCopyrightReport))]
    public class ReportCopyright : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IReport.IReportable> Accused { get; }
        [Length(32, 8192)]
        public string InfringementDescription { get; }
        public string URL { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime SetIn { get; }

        public ReportCopyright(BurcatIdentifier<Member> reporter, BurcatIdentifier<IReport.IReportable> message, string infringementDescription, string url, DateTime setIn = default) { Reporter = reporter; Accused = message; InfringementDescription = infringementDescription; URL = url; SetIn = setIn == default ? DateTime.Now : setIn; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member) || (DevOps.GetGrade(member) & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;

        public override object?[] GetBurcatConstructionValues() => [Reporter, Accused, InfringementDescription, URL, SetIn];
    }
}
