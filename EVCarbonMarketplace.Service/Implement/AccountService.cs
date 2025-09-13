using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Request.Account;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Model.Payload.Response.User;
using EVCarbonMarketplace.Model.Payload.Settings;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IEmailSender _emailSender;
        private readonly IUploadService _uploadService;

        public AccountService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork,
            ILogger<AccountService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConnectionMultiplexer redis,
            IEmailSender emailSender,
            IUploadService uploadService
            ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _redis = redis;
            _emailSender = emailSender;
            _uploadService = uploadService;
        }

      

       

        public async Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();
            if (accounts.Any(a => a.Username.Equals(request.UserName)))
                throw new BadHttpRequestException("Tên đăng nhập đã tồn tại");
            if (accounts.Any(a => a.Email.Equals(request.Email)))
                throw new BadHttpRequestException("Email đã tồn tại");
            if (accounts.Any(a => a.Phone.Equals(request.Phone)))
                throw new BadHttpRequestException("Số điện thoại đã tồn tại");
            var redisDb = _redis.GetDatabase();
            if (redisDb == null) throw new RedisServerException("Không thể kết nối tới Redis");

            var key = "emailOtp:" + request.Email;
             var stored = await redisDb.StringGetAsync(key);
            if (string.IsNullOrEmpty(stored))
                throw new NotFoundException("Không tìm thấy mã OTP");
            if (!stored.Equals(request.Otp))
                throw new BadHttpRequestException("Mã OTP không chính xác");

            var account = _mapper.Map<Account>(request);
            account.AvatarUrl = await _uploadService.UploadImage(request.AvatarUrl);

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            // thiếu tạo ví 


            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            await redisDb.KeyDeleteAsync(key);

            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đăng kí tài khoản");

            return new BaseResponse<RegisterResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo tài khoản thành công",
                Data = _mapper.Map<RegisterResponse>(account)
            };



        }

        

        public async Task<BaseResponse<bool>> SendOtp(string email)
        {
            if (!EmailUtil.IsValidEmail(email))
            {
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Email không đúng định dạng",
                    Data = false
                };
            }
            var redisDb = _redis.GetDatabase();
            var key = "emailOtp:" + email;
            var existingOTP = await redisDb.StringGetAsync(key);
            if (!string.IsNullOrEmpty(existingOTP))
            {
                return new BaseResponse<bool>()
                {
                    Status = StatusCodes.Status409Conflict.ToString(),
                    Message = "Mã OTP đã được gửi, vui lòng chờ một lát",
                    Data = false
                };
            }
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();
            var otp = OtpUtil.GenerateOtp();

            var placeholders = new Dictionary<string, string>
            {
                { "otp", otp },
                { "email", email }
            };

            var html = EmailUtil.GetTemplate("Email.html", placeholders);

            var emailMessage = new EmailMessage()
            {
                ToAddress = email,
                Body = html,
                Subject = otp + " là mã xác thực của bạn"
            };
            await _emailSender.SendEmailAsync(emailMessage);
            var redisSuccess = await redisDb.StringSetAsync(key, otp, TimeSpan.FromMinutes(5));
            if (!redisSuccess) throw new BadHttpRequestException("Không thể lưu mã OTP");

            return new BaseResponse<bool>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Gửi mã OTP thành công",
                Data = true
            };
        }

    
        public async Task<BaseResponse<bool>> ChangePassword(ChangePasswordRequest request)
        {
            Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(                
              predicate: a => a.Id == accountId && a.IsActive ==true) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            if (!account.Password.Equals(PasswordUtil.HashPassword(request.OldPassword)))
                throw new BadHttpRequestException("Mật khẩu cũ không trùng khớp");
            if(!request.NewPassword.Equals(request.ConfirmPassword))
                throw new BadHttpRequestException("Mật khẩu mới và xác nhận mật khẩu mới không trùng khớp");
            account.Password = PasswordUtil.HashPassword(request.NewPassword);
            account.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đổi mật khẩu");
            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Thay đổi mật khẩu thành công",
                Data = true
            };

        }

        public async Task<BaseResponse<bool>> ForgotPassword(string email)
        {
          var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Email == email && a.IsActive == true) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var otpResult = await SendOtp(email);
            if(otpResult.Data == false)
            {
                return new BaseResponse<bool>
                {
                    Status = otpResult.Status,
                    Message = otpResult.Message,
                    Data = false
                };
            }
            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Gửi mã xác nhận quên mật khẩu thành công",
                Data = true
            };

        }
        public async Task<BaseResponse<GetUserResponse>> VerifyOtp(string email, string otp)
        {
           var redisDb = _redis.GetDatabase();
            if (redisDb == null) throw new RedisServerException("Không thể kết nối tới Redis");

            var key = "emailOtp:" + email;
            var storedOtp = await redisDb.StringGetAsync(key);
            if(string.IsNullOrEmpty(storedOtp))
                throw new NotFoundException("Không tìm thấy mã OTP");
            if (!storedOtp.Equals(otp))
                throw new BadHttpRequestException("Mã OTP không chính xác");
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Email == email && a.IsActive == true) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            await redisDb.KeyDeleteAsync(key);
            return new BaseResponse<GetUserResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xác thực mã OTP thành công",
                Data = new GetUserResponse
                {
                    AccountId = account.Id,
                    FullName = account.FullName,
                    Email = account.Email,
                    Phone = account.Phone,
                    DateOfBirth = account.DateOfBirth,
                    Gender = account.Gender,
                    AvatarUrl = account.AvatarUrl,
                }
            };
        }
        public async Task<BaseResponse<GetUserResponse>> ResetPassword(ResetPasswordRequest request)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Email == request.Email && a.IsActive == true) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            if (!request.NewPassword.Equals(request.ConfirmPassword))
                throw new BadHttpRequestException("Mật khẩu mới và xác nhận mật khẩu mới không trùng khớp");
            account.Password = PasswordUtil.HashPassword(request.NewPassword);
            account.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đổi mật khẩu");
            return new BaseResponse<GetUserResponse> {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đặt lại mật khẩu thành công",
                Data = new GetUserResponse
                {
                    AccountId = account.Id,
                    FullName = account.FullName,
                    Email = account.Email,
                    Phone = account.Phone,
                    DateOfBirth = account.DateOfBirth,
                    Gender = account.Gender,
                    AvatarUrl = account.AvatarUrl,
                }

            };
        }
        public async Task<BaseResponse<GetUserResponse>> ChangeAvatar(IFormFile file)
        {
            Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                          predicate: a => a.Id == accountId && a.IsActive == true) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            account.AvatarUrl = await _uploadService.UploadImage(file);
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đổi ảnh đại diện");
            return new BaseResponse<GetUserResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đổi ảnh đại diện thành công",
                Data = new GetUserResponse
                {
                    AccountId = account.Id,
                    FullName = account.FullName,
                    Email = account.Email,
                    Phone = account.Phone,
                    DateOfBirth = account.DateOfBirth,
                    Gender = account.Gender,
                    AvatarUrl = account.AvatarUrl,
                }
            };
        }
    }
}
