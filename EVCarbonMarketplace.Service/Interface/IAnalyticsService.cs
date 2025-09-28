using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Analytic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IAnalyticsService
    {
        Task<BaseResponse<AnalyticsUsersResponse>> GetUsers();
        Task<BaseResponse<string>> GetRealtimeUsers();
        Task<BaseResponse<List<AnalyticsUserDailyResponse>>> GetRegisteredUsersByDay();
        Task<BaseResponse<FinanceStatsResponse>> GetFinanceStats();
        Task<BaseResponse<TransactionStatsResponse>> GetTransactionStats();
    }
}
