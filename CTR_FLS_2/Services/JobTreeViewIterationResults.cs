using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.Services
{
    // This object is used when traversing the job tree and storing intermediary
    // results so they can ultimately be reported on
    public class JobTreeViewIterationResults
    {
        public int ParentItemId { get; set; }
        public int ChildItemId { get; set; }
        public int Level { get; set; }
        public int Sequence { get; set; }
    }
}
