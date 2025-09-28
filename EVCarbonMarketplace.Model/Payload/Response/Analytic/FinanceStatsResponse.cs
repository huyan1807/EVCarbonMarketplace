using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Analytic
{
    public class FinanceStatsResponse
    {
        public decimal TotalDeposit { get; set; }
        public decimal TotalWithdraw { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<FinanceDailyStats> ByDate { get; set; }
    }
}
