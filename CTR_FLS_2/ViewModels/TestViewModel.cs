﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.ViewModels
{
    public class TestViewModel
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Job { get; set; }
        public Nullable<int> Suffix { get; set; }
        public Nullable<int> Test1 { get; set; }
        public Nullable<int> Template { get; set; }
        public string Name { get; set; }
        public string TestBolt { get; set; }
        public Nullable<int> AuditKey { get; set; }
        public string BoltSpec { get; set; }
        public string CyclesSamples { get; set; }
        public Nullable<System.DateTime> DateEntered { get; set; }
        public string EnteredBy { get; set; }
        public string Frequency { get; set; }
        public string MasterKey { get; set; }
        public string MaxInstall { get; set; }
        public string MaxValueRequired { get; set; }
        public string SigDecMax { get; set; }
        public string MinBrkway { get; set; }
        public string MinValueRequired { get; set; }
        public string SigDecMin { get; set; }
        public string PassFail { get; set; }
        public string PerformanceSpec { get; set; }
        public string PrintMinMax { get; set; }
        public string Requirements { get; set; }
        public string ResultCycles { get; set; }
        public string SeatLoad { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}