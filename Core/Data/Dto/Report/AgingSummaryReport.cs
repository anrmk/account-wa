﻿using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class AgingSummaryReport {
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Date { get; set; }
        public int DaysPerPeriod { get; set; }
        public int NumberOfPeriods { get; set; } //Количество периодово

        public int TotalCustomers { get; set; }

        public List<AgingSummaryPeriod> Columns { get; set; }
        public Dictionary<string, AgingSummaryBalance> Balance { get; set; }

        public List<AgingSummaryData> Data { get; set; }
        public int BalanceCustomers { get; set; }
    }

    public class AgingSummaryData {
        public string AccountNo { get; set; }

        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerAddress2 { get; set; }

        public CustomerDto Customer { get; set; }

        public Dictionary<string, decimal> Data { get; set; }
    }

    public class AgingSummaryBalance {
        public decimal Sum { get; set; }
        public int Count { get; set; }
    }

    public class AgingSummaryPeriod {
        public int? From { get; set; }
        public int? To { get; set; }
        public string Name { get; set; }
    }
}
