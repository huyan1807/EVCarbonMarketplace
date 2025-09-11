using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Mapper
{
    public class AccountMapper : Profile
    {
        public AccountMapper() {
            //customer
            CreateMap<RegisterRequest, Account>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                  .ForMember(dest => dest.Password, opt => opt.MapFrom(src => PasswordUtil.HashPassword(src.Password)))
                  .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleEnum.CcBuyer.GetDescriptionFromEnum()))
                  .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                  .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.GetDescriptionFromEnum()))
                  .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                  .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                  .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            CreateMap<RegisterResponse, Account>();
        }
    }
}
