using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Response;
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

        public async Task<BaseResponse<decimal>> GetTransactionFee()
        {
            var feeSetting = await _unitOfWork.GetRepository<SystemSetting>()
                .SingleOrDefaultAsync(predicate: s => s.Key == "TransactionFeeRate" );

            decimal feeRate = feeSetting != null ? decimal.Parse(feeSetting.Value) : 0;

            return new BaseResponse<decimal>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy phí giao dịch thành công",
                Data = feeRate
            };
        }

        public async Task<BaseResponse<string>> UpdateTransactionFee(decimal newFeeRate)
        {
            try
            {
                var settingRepo = _unitOfWork.GetRepository<SystemSetting>();

                var feeSetting = await settingRepo.SingleOrDefaultAsync(
                    predicate: s => s.Key == "TransactionFeeRate" );

                if (feeSetting == null)
                {
                    // Nếu chưa có thì tạo mới
                    feeSetting = new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "TransactionFeeRate",
                        Value = newFeeRate.ToString(),                     
                        UpdateAt = TimeUtil.GetCurrentSEATime(),                      
                    };
                    await settingRepo.InsertAsync(feeSetting);
                }
                else
                {
                    feeSetting.Value = newFeeRate.ToString();
                    feeSetting.UpdateAt = TimeUtil.GetCurrentSEATime();
                    settingRepo.UpdateAsync(feeSetting);
                }

                var isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess) throw new Exception("Không thể cập nhật phí giao dịch");

                return new BaseResponse<string>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = $"Cập nhật phí giao dịch thành công: {newFeeRate}%",
                    Data = feeSetting.Value
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SystemSetting] Lỗi khi cập nhật TransactionFeeRate");
                return new BaseResponse<string>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Đã xảy ra lỗi khi cập nhật phí giao dịch",
                    Data = null
                };
            }
        }
    }
}
