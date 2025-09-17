using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response.ElectricVehicle;
using EVCarbonMarketplace.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Mapper
{
    public class ElectricVehicleMapper : Profile
    {
        public ElectricVehicleMapper()
        {
            // EV
            CreateMap<ElectricVehicleRequest, ElectricVehicle>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<ElectricVehicleUpdateRequest, ElectricVehicle>()
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            
            CreateMap<ElectricVehicle, ElectricVehicleResponse>();



        }
    }
}
