using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.BankAccount;
using EVCarbonMarketplace.Model.Payload.Response.BankAccount;
using EVCarbonMarketplace.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Mapper
{
    public class BankAccountMapper : Profile
    {
        public BankAccountMapper() {


            CreateMap<CreateBankAccountRequest, BankAccount>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.AccountId, opt => opt.Ignore()) 
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(_ => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());
            CreateMap<BankAccount, BankAccountResponse>();

        }
    }
}
