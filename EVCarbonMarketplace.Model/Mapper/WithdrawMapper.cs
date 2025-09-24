using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.Withdraw;
using EVCarbonMarketplace.Model.Payload.Response.Withdraw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Mapper
{
    public class WithdrawMapper : Profile
    {
        public WithdrawMapper() {
            CreateMap<Withdraw, WithdrawResponse>()
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankAccount.BankName))
                .ForMember(dest => dest.BankAccountNumber, opt => opt.MapFrom(src => src.BankAccount.BankAccountNumber))
                .ForMember(dest => dest.BankAccountHolder, opt => opt.MapFrom(src => src.BankAccount.BankAccountHolder))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.BankAccount.LogoUrl))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProofUrl, opt => opt.MapFrom(src => src.ProofUrl));

            CreateMap<WithdrawRequest, Withdraw>();

        }
    }
}
