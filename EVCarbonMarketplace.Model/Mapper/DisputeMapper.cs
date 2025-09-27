using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.Dispute;
using EVCarbonMarketplace.Model.Payload.Response.Dispute;
using EVCarbonMarketplace.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Mapper
{
    public class DisputeMapper : Profile
    {
        public DisputeMapper() {

            CreateMap<DisputeRequest, Dispute>()
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());

            CreateMap<UpdateDisputeStatusRequest, Dispute>()
                     .ForMember(dest => dest.CreateAt, opt => opt.Ignore());

            CreateMap<Dispute, DisputeResponse>();




        }
    }
}
