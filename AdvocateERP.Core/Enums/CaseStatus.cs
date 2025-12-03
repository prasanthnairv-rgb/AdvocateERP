using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvocateERP.Core.Enums
{
    public enum CaseStatus
    {
        Draft = 0,
        Open = 1,
        PendingHearing = 2,
        InProgress = 3,
        Closed = 4,
        Archived = 5
    }
}
