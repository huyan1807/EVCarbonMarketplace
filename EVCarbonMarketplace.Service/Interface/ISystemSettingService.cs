using EVCarbonMarketplace.Model.Payload.Request.SystemSetting;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.SystemSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface ISystemSettingService
    {
        Task<BaseResponse<SystemSettingResponse>> Update(SystemSettingRequest request);
        Task<BaseResponse<List<SystemSettingResponse>>> GetAll();
    }
}
