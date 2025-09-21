using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.CarbonListing;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonListing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface ICarbonListingService
    {
        Task<BaseResponse<CarbonListingResponse>> Create(CarbonListingRequest request, CarbonListingEnum.ListingType type);

        Task<BaseResponse<IPaginate<CarbonListingManagerResponse>>> GetAll(int page, int size, CarbonListingEnum.ListingType? type, CarbonListingEnum.ListingStatus? status);
        Task<BaseResponse<bool>> Delete(Guid id);

        Task<BaseResponse<IPaginate<CarbonListingManagerResponse>>> GetMyListing(int page, int size, CarbonListingEnum.ListingType? type, CarbonListingEnum.ListingStatus? status);

        Task<BaseResponse<CarbonListingDetailResponse>> GetById(Guid id);

        Task<BaseResponse<CarbonListingResponse>> Update(Guid id, CarbonListingUpdateRequest request);


    }
}
