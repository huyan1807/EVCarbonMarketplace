using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IVehicleTypeService
    {
        Task<BaseResponse<IPaginate<VehicleTypeResponse>>> GetAll(int page , int size);
        Task<BaseResponse<VehicleTypeResponse>> Create(VehicleTypeRequest request);
        Task<BaseResponse<bool>> Delete(Guid id);
    }
}
