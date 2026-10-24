﻿using Burcat.API.Development;
using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        /// <summary>Requires an existing, unexpired session belonging to a non-banned moderator.</summary>
        public static ValidationResult? ValidateModeratorSession(Session? session)
        {
            if (session is null) return new("An active moderator session is required.");
            Session? current = InterfaceOptions.TryFind<Session>(session);
            if (current is null || current.Token != session.Token || current.Owner != session.Owner || current.EndTime <= DateTime.Now)
                return new("An active moderator session is required.");
            Member? moderator = InterfaceOptions.TryFind(current.Owner);
            return moderator is not null && !moderator.IsBanned && DevOps.HasGrade(current.Owner, DevOpsGrade.Moderator)
                ? ValidationResult.Success : new("Moderator permission is required.");
        }

        /// <summary>Requires moderation status to be changed through the authorized moderation action.</summary>
        public static ValidationResult? ValidateMemberModeration(Member member)
        {
            Member? current = InterfaceOptions.TryFind<Member>(member);
            return current is not null && (member.IsBanned != current.IsBanned || member.IsUnderInvestigation != current.IsUnderInvestigation)
                ? new("Use the moderation action to change investigation or ban status.") : ValidationResult.Success;
        }

        /// <summary>Requires a report target that exists in one of the supported entity tables.</summary>
        public static ValidationResult? ValidateReportTarget(IReport report) => InterfaceOptions.TryFindReportable(report.Accused.Value) is null
            ? new("The report target does not exist.") : ValidationResult.Success;

        public static ValidationResult? ValidatePolitenessReport(ReportPoliteness report)
        {
            IReport.IReportable? target = InterfaceOptions.TryFindReportable(report.Accused.Value);
            if (target is null) return new("The report target does not exist.");
            if (GetReportOwner(target) == report.Reporter) return new("You cannot report your own content.");
            Politeness expected = InterfaceOptions.Find(report.ExpectedPoliteness);
            List<Politeness> declarations = [.. GetReportPolitenesses(target)];
            if (declarations.Count == 0) return new("The report target has no declared icon politeness to report.");
            else if (declarations.All(declared => declared >= expected)) return new("The expected politeness must be under the declared politeness.");
            else return ValidationResult.Success;
        }
        public static ValidationResult? ValidateCopyrightReport(ReportCopyright report)
        {
            IReport.IReportable? target = InterfaceOptions.TryFindReportable(report.Accused.Value);
            if (target is null) return new("The report target does not exist.");
            if (GetReportOwner(target) == report.Reporter) return new("You cannot report your own content.");
            else return ValidationResult.Success;
        }

        private static BurcatIdentifier<Member> GetReportOwner(IReport.IReportable target) => target switch
        {
            IMessage message => message.Owner,
            Faction faction => faction.Owner,
            Member member => member,
            Application application => InterfaceOptions.Find(application.Owner).Member,
            _ => throw new InvalidOperationException("The report target type is not supported.")
        };

        private static IEnumerable<Politeness> GetReportPolitenesses(IReport.IReportable target) => target switch
        {
            IMessage message => [InterfaceOptions.Find(message.Politeness)],
            Faction { Icon: BurcatIdentifier<Image> icon } => [InterfaceOptions.Find(InterfaceOptions.Find(icon).Politeness)],
            Faction => [],
            Application { Icon: BurcatIdentifier<Image> icon } => [InterfaceOptions.Find(InterfaceOptions.Find(icon).Politeness)],
            Application => [],
            Member member => (from iconography in InterfaceOptions.Provider.Get<MemberIconography>()
                join image in InterfaceOptions.Provider.Get<Image>() on (Guid)iconography.Icon equals image.Identifier
                join politeness in InterfaceOptions.Provider.Get<Politeness>() on (Guid)image.Politeness equals politeness.Identifier
                where (Guid)iconography.Owner == member.Identifier
                select politeness).AsEnumerable(),
            _ => throw new InvalidOperationException("The report target type is not supported.")
        };
    }
}
