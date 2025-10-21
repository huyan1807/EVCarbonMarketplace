using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Request.Notification;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Model.Exceptions;
namespace EVCarbonMarketplace.Service.Implement
{
    public class NotificationService : BaseService<NotificationService>, INotificationService
    {
        private readonly FirestoreDb _firestore;
        public NotificationService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<NotificationService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, FirestoreDb firestore) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _firestore = firestore;
        }

        public async Task<BaseResponse<NotificationRequest>> Create(NotificationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId))
                    throw new ArgumentException("UserId không được để trống");

                request.IsRead = false;
                request.CreatedAt = Timestamp.FromDateTime(TimeUtil.GetCurrentSEATime().ToUniversalTime());

                var col = _firestore.Collection("notifications");
                await col.AddAsync(request);

                return new BaseResponse<NotificationRequest>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Tạo thông báo thành công",
                    Data = request
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thông báo");

                return new BaseResponse<NotificationRequest>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = null
                };
            }

        }

        public async Task<BaseResponse<bool>> DeleteNotification(string id)
        {
            try
            {
                var docRef = _firestore.Collection("notifications").Document(id);
                await docRef.DeleteAsync();

                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Xóa thông báo thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<BaseResponse<bool>> MarkAllRead()
        {
            try
            {
                var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
                var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản");
                var query = await _firestore
                    .Collection("notifications")
                    .WhereEqualTo("UserId", accountId.ToString())
                    .WhereEqualTo("IsRead", false)
                    .GetSnapshotAsync();

                if (!query.Any())
                {
                    return new BaseResponse<bool>
                    {
                        Status = StatusCodes.Status200OK.ToString(),
                        Message = "Không có thông báo nào cần đánh dấu đã đọc",
                        Data = false
                    };
                }

                var batch = _firestore.StartBatch();
                foreach (var doc in query.Documents)
                {
                    batch.Update(doc.Reference, new Dictionary<string, object> { { "IsRead", true } });
                }

                await batch.CommitAsync();

                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = $"Đã đánh dấu {query.Count} thông báo là đã đọc",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu tất cả thông báo đã đọc");
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<BaseResponse<bool>> MarkRead(string id)
        {
                try
                {
                var docRef = _firestore.Collection("notifications").Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "IsRead", true }
                });

                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Đánh dấu thông báo đã đọc thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu thông báo đã đọc");
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
