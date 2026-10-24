using Burcat.API.Development;
using Burcat.API.Media;
using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.System
{
    [BurcatIdentity("2054690c-420a-4bd2-ad7b-0a9cc4cb2a79")]
    [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidatePolitenessReport))]
    public class ReportPoliteness : BurcatObject, IReport
    {
        public BurcatIdentifier<Member> Reporter { get; }
        public BurcatIdentifier<IReport.IReportable> Accused { get; }
        public BurcatIdentifier<Politeness> ExpectedPoliteness { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        public DateTime SetIn { get; }

        public ReportPoliteness(BurcatIdentifier<Member> reporter, BurcatIdentifier<IReport.IReportable> message, BurcatIdentifier<Politeness> expectedPoliteness, DateTime setIn = default) { Reporter = reporter; Accused = message; ExpectedPoliteness = expectedPoliteness; SetIn = setIn == default ? DateTime.Now : setIn; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member) || (DevOps.GetGrade(member) & DevOpsGrade.Moderator) == DevOpsGrade.Moderator;
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && Reporter == member;

        public override object?[] GetBurcatConstructionValues() => [Reporter, Accused, ExpectedPoliteness, SetIn];
    }
}
