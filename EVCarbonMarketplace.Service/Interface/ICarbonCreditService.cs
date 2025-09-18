using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonCredit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface ICarbonCreditService
    {
        Task<BaseResponse<IPaginate<CarbonCreditResponse>>> GetMyCredits();

        Task<BaseResponse<IPaginate<CarbonCreditManageResponse>>> GetAllCredits(int page ,int size);
    }
}
