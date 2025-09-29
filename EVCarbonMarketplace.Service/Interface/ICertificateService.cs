using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public  interface ICertificateService
    {
        Task<BaseResponse<CertificateResponse>> RequestCertificate(Guid carbonCreditId);
        Task<BaseResponse<IPaginate<CertificateResponse>>> GetMyCertificate(int page ,int size);

        Task<BaseResponse<CertificateResponse>> GetCertificate(Guid carbonCreditId);

        Task<BaseResponse<IPaginate<CertificateResponse>>> GetAll(int page , int size);
        
        Task<BaseResponse<bool>> Delete(Guid id);
    }
}
