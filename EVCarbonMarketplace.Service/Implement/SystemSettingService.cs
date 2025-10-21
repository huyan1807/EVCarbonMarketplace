using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.SystemSetting;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.SystemSetting;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class SystemSettingService : BaseService<SystemSettingService>, ISystemSettingService
    {
        public SystemSettingService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<SystemSettingService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<List<SystemSettingResponse>>> GetAll()
        {
            var settings = (await _unitOfWork.GetRepository<SystemSetting>()
                     .GetListAsync(
                         selector: s => new SystemSettingResponse
                         {
                             Id = s.Id,
                             Key = s.Key,
                             Value = s.Value,
                             Description = s.Description
                         }
                     )).ToList();
            return new BaseResponse<List<SystemSettingResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy tất cả cài đặt hệ thống thành công",
                Data = settings
            };
        }

  
        public async Task<BaseResponse<SystemSettingResponse>> Update(SystemSettingRequest request)
        {
            try
            {

                var setting = await _unitOfWork.GetRepository<SystemSetting>().SingleOrDefaultAsync(
                    predicate: s => s.Id == request.Id);

                if (setting == null)
                {
                    return new BaseResponse<SystemSettingResponse>
                    {
                        Status = StatusCodes.Status404NotFound.ToString(),
                        Message = "Cài đặt hệ thống không tồn tại",
                        Data = null
                    };
                }

                setting.Value = request.Value;
                setting.UpdateAt = TimeUtil.GetCurrentSEATime();
                _unitOfWork.GetRepository<SystemSetting>().UpdateAsync(setting);

                var isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess) throw new Exception("Không thể cập nhật cài đặt hệ thống");

                var response = new SystemSettingResponse
                {
                    Id = setting.Id,
                    Key = setting.Key,
                    Value = setting.Value,
                    Description = setting.Description,
                };

                return new BaseResponse<SystemSettingResponse>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Cập nhật cài đặt hệ thống thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SystemSetting] Lỗi khi cập nhật cài đặt hệ thống");
                return new BaseResponse<SystemSettingResponse>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Đã xảy ra lỗi khi cập nhật cài đặt hệ thống",
                    Data = null
                };
            }

        }

      
    }
}
