using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Payload.Request.Account;
using EVCarbonMarketplace.Model.Payload.Request.Cva;
using EVCarbonMarketplace.Model.Payload.Request.Owner;
using EVCarbonMarketplace.Model.Payload.Response.Account;
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

            //owner
            CreateMap<RegisterOwnerRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => PasswordUtil.HashPassword(src.Password)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleEnum.EvOwner.GetDescriptionFromEnum()))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.GetDescriptionFromEnum()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));
            //cva
            CreateMap<RegisterCvaRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => PasswordUtil.HashPassword(src.Password)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleEnum.Cva.GetDescriptionFromEnum()))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.GetDescriptionFromEnum()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));
            CreateMap<Account, RegisterResponse>();
        }
    }
}
