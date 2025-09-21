using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.CarbonListing;
using EVCarbonMarketplace.Model.Payload.Response.CarbonListing;
using EVCarbonMarketplace.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Mapper
{
    public class CarbonListingMapper : Profile
    {
        public CarbonListingMapper()
        {

            CreateMap<CarbonListingRequest, CarbonListing>()
                    .ForMember(x => x.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                    .ForMember(x => x.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(x => x.IsActive, opt => opt.MapFrom(src => true))
                     .ForMember(x => x.AccountId, opt => opt.Ignore())
                    .ForMember(x => x.Type, opt => opt.Ignore())
                    .ForMember(x => x.Status, opt => opt.Ignore())
                    .ForMember(x => x.CarbonCreditId, opt => opt.Ignore());


            CreateMap<CarbonListing, CarbonListingResponse>();
        }
    }
}
