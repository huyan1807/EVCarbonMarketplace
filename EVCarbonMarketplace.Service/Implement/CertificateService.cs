using AutoMapper;
using Microsoft.EntityFrameworkCore;

using DinkToPdf.Contracts;
using DinkToPdf;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Certificate;
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
using EVCarbonMarketplace.Model.Paginate;

namespace EVCarbonMarketplace.Service.Implement
{
    public class CertificateService : BaseService<CertificateService>, ICertificateService
    {
        private readonly IUploadService _uploadService;
        private readonly IConverter _converter;

        public CertificateService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<CertificateService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadService uploadService, IConverter converter) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
            _converter = converter;
        }

        public async Task<BaseResponse<bool>> Delete(Guid id)
        {
           var certificate = await _unitOfWork.GetRepository<Certificate>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true && x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy chứng chỉ");

            certificate.IsActive = false;
            certificate.DeleteAt = TimeUtil.GetCurrentSEATime();

            _unitOfWork.GetRepository<Certificate>().UpdateAsync(certificate);
        var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new BadHttpRequestException("Có lỗi trong quá trình xóa chứng chỉ");

            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa chứng chỉ thành công",
                Data = true
            };
      }

        public async Task<BaseResponse<IPaginate<CertificateResponse>>> GetAll(int page, int size)
        {

            var certificateRepo = await _unitOfWork.GetRepository<Certificate>().GetPagingListAsync(

                selector: x => new CertificateResponse
                {
                    Id = x.Id,
                    CarbonCreditId = x.CarbonCreditId,
                    BuyerId = x.BuyerId,
                    SerialNumber = x.SerialNumber,
                    CertificateUrl = x.CertificateUrl,
                    Status = x.Status,
                    IssuedAt = x.IssuedAt,
                    CreateAt = x.CreateAt
                },
                predicate: x => x.IsActive == true && x.DeleteAt == null,
                orderBy: x => x.OrderByDescending(c => c.CreateAt),
                page: page,
                size: size
                );
            return new BaseResponse<IPaginate<CertificateResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách chứng chỉ thành công",
                Data = certificateRepo
            };

        }

        public async Task<BaseResponse<CertificateResponse>> GetCertificate(Guid carbonCreditId)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                 predicate: x => x.Id == accountId && x.IsActive == true && x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            var certificate = await _unitOfWork.GetRepository<Certificate>().SingleOrDefaultAsync(
                selector: x => new CertificateResponse
                {
                    Id = x.Id,
                    CarbonCreditId = x.CarbonCreditId,
                    BuyerId = x.BuyerId,
                    SerialNumber = x.SerialNumber,
                    CertificateUrl = x.CertificateUrl,
                    Status = x.Status,
                    IssuedAt = x.IssuedAt,
                    CreateAt = x.CreateAt
                },
                predicate: x => x.CarbonCreditId == carbonCreditId &&
                                 x.BuyerId == accountId &&
                                 x.IsActive == true &&
                                 x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy chứng chỉ");

            return new BaseResponse<CertificateResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy chứng chỉ thành công",
                Data = certificate
            };

        }

        public async Task<BaseResponse<IPaginate<CertificateResponse>>> GetMyCertificate(int page, int size)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                 predicate: x => x.Id == accountId && x.IsActive == true && x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var certificateRepo = await _unitOfWork.GetRepository<Certificate>().GetPagingListAsync(

                selector: x => new CertificateResponse
                {
                    Id = x.Id,
                    CarbonCreditId = x.CarbonCreditId,
                    BuyerId = x.BuyerId,
                    SerialNumber = x.SerialNumber,
                    CertificateUrl = x.CertificateUrl,
                    Status = x.Status,
                    IssuedAt = x.IssuedAt,
                    CreateAt = x.CreateAt
                },
                predicate: x => x.BuyerId == accountId && x.IsActive == true && x.DeleteAt == null,
                orderBy: x => x.OrderByDescending(c => c.CreateAt),
                page: page,
                size: size
                );
            return new BaseResponse<IPaginate<CertificateResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách chứng chỉ thành công",
                Data = certificateRepo
            };

        }

        public async Task<BaseResponse<CertificateResponse>> RequestCertificate(Guid carbonCreditId)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                 predicate: x => x.Id == accountId && x.IsActive == true && x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            var carbonCredit = await _unitOfWork.GetRepository<CarbonCredit>().SingleOrDefaultAsync(
                predicate: x => x.Id == carbonCreditId && x.IsActive == true && x.DeleteAt == null,
                  include: i => i.Include(c => c.CarbonEmission)
            ) ?? throw new NotFoundException("Không tìm thấy tín chỉ carbon");

            if (carbonCredit.Status != "Available")
                throw new NotFoundException("Tín chỉ carbon không khả dụng");

            var existingCertificate = await _unitOfWork.GetRepository<Certificate>().SingleOrDefaultAsync(
                predicate: x => x.CarbonCreditId == carbonCreditId &&
                                x.BuyerId == accountId &&
                                x.IsActive == true &&
                                x.DeleteAt == null
            );
            if (existingCertificate != null)
                throw new NotFoundException("Bạn đã yêu cầu cấp chứng chỉ cho tín chỉ carbon này");

            var serial = new Random().Next(100000, 999999);

            var placeholders = new Dictionary<string, string>
    {
        { "BuyerName", account.FullName },
        { "CarbonCreditId", carbonCredit.Id.ToString() },
        { "Quantity", carbonCredit.CarbonEmission.Co2reduced.ToString() },
        { "Credits", carbonCredit.Credits.ToString() },
        { "CarbonEmissionId", carbonCredit.CarbonEmissionId.ToString() },
        { "Vintage", carbonCredit.CreateAt?.Year.ToString() ?? "" },
        { "SerialNumber", serial.ToString() },
        { "IssuedAt", TimeUtil.GetCurrentSEATime().ToString("dd/MM/yyyy HH:mm") },
        { "Status", "Đã Phát Hành" }
    };

            string html = TemplateUtil.GetTemplate("CertificateTemplate.html", placeholders);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Landscape,
            PaperSize = PaperKind.A4
             },
                Objects = {
                new ObjectSettings {
                    HtmlContent = html,
                    WebSettings = {
                        DefaultEncoding = "utf-8",
                        LoadImages = true,
                        EnableIntelligentShrinking = true
                    },
                    UseLocalLinks = false,
                    UseExternalLinks = true,

                } }
            };

            var pdfBytes = _converter.Convert(doc);
            Console.WriteLine($"PDF length = {pdfBytes?.Length ?? 0} bytes");
            Console.WriteLine($"[RequestCertificate] carbonCreditId = {carbonCreditId}");

            var fileName = $"certificate-{carbonCreditId}.pdf";
            await using var stream = new MemoryStream(pdfBytes);
            var fileUrl = await _uploadService.UploadToFirebaseAsync(
                new FormFile(stream, 0, pdfBytes.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                }
            );

            var cert = new Certificate
            {
                Id = Guid.NewGuid(),
                CarbonCreditId = carbonCreditId,
                BuyerId = accountId,
                SerialNumber = serial,
                CertificateUrl = fileUrl, 
                Status = "Issued",
                IssuedAt = TimeUtil.GetCurrentSEATime(),
                IsActive = true,
                CreateAt = TimeUtil.GetCurrentSEATime()
            };

            await _unitOfWork.GetRepository<Certificate>().InsertAsync(cert);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new BadHttpRequestException("Có lỗi trong quá trình cấp chứng nhận");

            var response = new CertificateResponse
            {
                Id = cert.Id,
                CarbonCreditId = cert.CarbonCreditId,
                BuyerId = cert.BuyerId,
                SerialNumber = cert.SerialNumber,
                CertificateUrl = cert.CertificateUrl,
                Status = cert.Status,
                IssuedAt = cert.IssuedAt,
                CreateAt = cert.CreateAt
            };

            return new BaseResponse<CertificateResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Yêu cầu cấp chứng chỉ thành công",
                Data = response
            };
        }

    }
}
