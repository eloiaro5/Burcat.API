using BurcatProtocol;
using BurcatProtocol.Annotations;
using BurcatProtocol.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Media
{
    [BurcatIdentity("f9397374-3ff7-4a06-8e62-6f9eae16f080")]
    public interface IMessage : Burcat.API.System.IReport.IReportable
    {
        BurcatIdentifier<Member> Owner { get; }
        BurcatIdentifier<Politeness> Politeness { get; }
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateDateEqualOrUnderNow))]
        DateTime PostDate { get; }

        /// <summary>Clears message content and media references while preserving identity and conversation metadata.</summary>
        /// <remarks>Invoke through the protocol action pipeline to validate the deletion record and investigation status before mutation.</remarks>
        [BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateMessageDeletion))]
        public void Delete();
    }
}
