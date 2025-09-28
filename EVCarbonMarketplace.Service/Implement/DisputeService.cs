using AutoMapper;
using Microsoft.EntityFrameworkCore;

using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Request.Dispute;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Dispute;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Paginate;

namespace EVCarbonMarketplace.Service.Implement
{
    public class DisputeService : BaseService<DisputeService>, IDisputeService
    {
        private readonly IUploadService _uploadService;
        public DisputeService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<DisputeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadService uploadService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
        }

        public async Task<BaseResponse<DisputeResponse>> Create(DisputeRequest request)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: x => x.Id == request.TransactionId && x.IsActive == true,
                include: source => source.Include(x => x.Disputes)
                ) ?? throw new NotFoundException("Không tìm thấy giao dịch");

            if (transaction.Disputes.Any(x => x.SendAccountId == accountId && x.IsActive == true))
                throw new BadHttpRequestException("Bạn đã tạo tranh chấp cho giao dịch này");
            if (request.Type == null)
                throw new BadHttpRequestException("Loại tranh chấp không được để trống");
            if (!Enum.IsDefined(typeof(DisputeTypeEnum), request.Type))
                throw new BadHttpRequestException("Loại tranh chấp không hợp lệ");
            if (string.IsNullOrEmpty(request.Description))
                throw new BadHttpRequestException("Mô tả không được để trống");
            var dispute = _mapper.Map<Dispute>(request);
            dispute.SendAccountId = accountId;
            dispute.IsActive = true;
            dispute.Description = request.Description;
            if (request.EvidenceUrl != null)
            {
                var evidenceUrl = await _uploadService.UploadImage(request.EvidenceUrl);
                dispute.EvidenceUrl = evidenceUrl;
            }
            dispute.Status = DisputeStatusEnum.Pending.ToString();
            await _unitOfWork.GetRepository<Dispute>().InsertAsync(dispute);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess)
                throw new Exception("Có lỗi trong quá trình tạo tranh chấp");
            var disputeResponse = _mapper.Map<DisputeResponse>(dispute);
            return new BaseResponse<DisputeResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo tranh chấp thành công",
                Data = disputeResponse

            };

        }

        public async Task<BaseResponse<IPaginate<DisputeResponse>>> GetMyDisputes(int page, int size, DisputeTypeEnum? type, DisputeStatusEnum? status)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var disputes = await _unitOfWork.GetRepository<Dispute>().GetPagingListAsync(
                selector: s => new DisputeResponse
                {
                    Id = s.Id,
                    SendAccountId = s.SendAccountId.Value,
                    TransactionId = s.TransactionId.Value,
                    Type = s.Type,
                    Description = s.Description,
                    EvidenceUrl = s.EvidenceUrl,
                    Status = s.Status,
                    CreateAt = s.CreateAt.Value,
                },
                predicate: x => x.IsActive == true && x.SendAccountId == accountId && (type == null || x.Type == type.ToString())
                    && (status == null || x.Status == status.ToString()),
                include: s => s.Include(x => x.SendAccount).Include(x => x.Transaction),
                size: size,
                page: page,
                orderBy: source => source.OrderByDescending(x => x.CreateAt)
                );
            var disputeResponses = _mapper.Map<IPaginate<DisputeResponse>>(disputes);
            return new BaseResponse<IPaginate<DisputeResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tranh chấp thành công",
                Data = disputeResponses

            };

        }

        public List<KeyValuePair<string, string>> GetDisputeTypes()
        {
            return Enum.GetValues(typeof(DisputeTypeEnum))
              .Cast<DisputeTypeEnum>()
              .Select(e => new KeyValuePair<string, string>(
                  e.ToString(),
                  e.GetDescriptionFromEnum()
              )).ToList();
        }

        public async Task<BaseResponse<DisputeResponse>> Update(UpdateDisputeStatusRequest request)
        {
            var dispute = await _unitOfWork.GetRepository<Dispute>().SingleOrDefaultAsync(
                predicate: x => x.Id == request.DisputeId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tranh chấp");
            if (request.Status == null)
                throw new BadHttpRequestException("Trạng thái không được để trống");
            if (!Enum.IsDefined(typeof(DisputeStatusEnum), request.Status))
                throw new BadHttpRequestException("Trạng thái không hợp lệ");
            if (dispute.Status != DisputeStatusEnum.Pending.ToString())
                throw new BadHttpRequestException("Chỉ có thể cập nhật tranh chấp ở trạng thái đang chờ xử lý");
            dispute.Status = request.Status.ToString();
            dispute.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Dispute>().UpdateAsync(dispute);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess)
                throw new Exception("Có lỗi trong quá trình cập nhật tranh chấp");
            var disputeResponse = _mapper.Map<DisputeResponse>(dispute);
            return new BaseResponse<DisputeResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Cập nhật tranh chấp thành công",
                Data = disputeResponse

            };

        }

        public async Task<BaseResponse<DisputeDetailResponse>> GetById(Guid id)
        {
            var dispute = await _unitOfWork.GetRepository<Dispute>().SingleOrDefaultAsync(
                selector: s => new DisputeDetailResponse
                {
                    Id = s.Id,
                    Type = s.Type,
                    Status = s.Status,
                    Description = s.Description,
                    EvidenceUrl = s.EvidenceUrl,
                    CreateAt = s.CreateAt,

                    SendAccountId = s.SendAccountId,
                    SendAccountName = s.SendAccount.FullName,

                    TransactionId = s.Transaction.Id,
                    TransactionType = s.Transaction.Type,
                    TransactionStatus = s.Transaction.Status,
                    TransactionAmount = s.Transaction.Amount,
                    TransactionDate = s.Transaction.CreateAt,
                    TransactionDescription = s.Transaction.Description,

                    BuyerId = s.Transaction.BuyerId,
                    BuyerName = s.Transaction.Buyer.FullName,
                    SellerId = s.Transaction.SellerId,
                    SellerName = s.Transaction.Seller.FullName,

                    CarbonListingId = s.Transaction.CarbonListingId.Value,

                },
                predicate: x => x.Id == id && x.IsActive == true,
                include: s => s
                    .Include(x => x.SendAccount)
                    .Include(x => x.Transaction).ThenInclude(t => t.Buyer)
                    .Include(x => x.Transaction).ThenInclude(t => t.Seller)
                    .Include(x => x.Transaction).ThenInclude(t => t.CarbonListing)
                ) ?? throw new NotFoundException("Không tìm thấy tranh chấp");
            return new BaseResponse<DisputeDetailResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy chi tiết tranh chấp thành công",
                Data = dispute

            };

        }

        public async Task<BaseResponse<IPaginate<DisputeResponse>>> GetAll(int page, int size, DisputeTypeEnum? type, DisputeStatusEnum? status)
        {
            var disputes = await _unitOfWork.GetRepository<Dispute>().GetPagingListAsync(
                selector: s => new DisputeResponse
                {
                    Id = s.Id,
                    SendAccountId = s.SendAccountId.Value,
                    TransactionId = s.TransactionId.Value,
                    Type = s.Type,
                    Description = s.Description,
                    EvidenceUrl = s.EvidenceUrl,
                    Status = s.Status,
                    CreateAt = s.CreateAt.Value,
                },
                predicate: x => x.IsActive == true
                    && (type == null || x.Type == type.ToString())
                    && (status == null || x.Status == status.ToString())
                ,
                include: s => s.Include(x => x.SendAccount).Include(x => x.Transaction),
                size: size,
                page: page,
                orderBy: source => source.OrderByDescending(x => x.CreateAt)
                );
            var disputeResponses = _mapper.Map<IPaginate<DisputeResponse>>(disputes);
            return new BaseResponse<IPaginate<DisputeResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tranh chấp thành công",
                Data = disputeResponses

            };
        }

        public async Task<BaseResponse<bool>> Delete(Guid id)
        {
            var dispute = await _unitOfWork.GetRepository<Dispute>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tranh chấp");
            dispute.IsActive = false;
            dispute.DeleteAt = TimeUtil.GetCurrentSEATime();
            dispute.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Dispute>().UpdateAsync(dispute);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess)
                throw new Exception("Có lỗi trong quá trình xóa tranh chấp");
            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa tranh chấp thành công",
                Data = true

            };

        }
    }
}
