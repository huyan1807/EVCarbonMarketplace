using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
   public interface IElectricVehicleService
    {
        Task<BaseResponse<ElectricVehicleResponse>> Create(ElectricVehicleRequest request);
        Task<BaseResponse<ElectricVehicleResponse>> Update(Guid id, ElectricVehicleUpdateRequest request);
        Task<BaseResponse<ElectricVehicleResponse>> ChangeImage(Guid? id,IFormFile file);

        Task<BaseResponse<bool>> Delete(Guid id);

        Task<BaseResponse<IPaginate<ElectricVehicleResponse>>> GetAll(int page, int size);

        Task<BaseResponse<IPaginate<ElectricVehicleResponse>>> GetMyEVehicles(int page, int size);


    }
}
