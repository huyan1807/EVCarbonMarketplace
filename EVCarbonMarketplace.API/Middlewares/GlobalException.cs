using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Response;
using System.Net;
using System.Text.Json;

namespace EVCarbonMarketplace.API.Middlewares
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;
        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                if (!context.Response.HasStarted)
                {
                    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                        await HandleUnauthorizedAsync(context);
                    }
                    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        await HandleForbiddenAsync(context);
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleUnauthorizedAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var errorResponse = new BaseResponse<object>
            {
                Status = StatusCodes.Status401Unauthorized.ToString(),
                Message = "Không được phép truy cập: Vui lòng đăng nhập.",
                Data = null
            };

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }

        private async Task HandleForbiddenAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var errorResponse = new BaseResponse<object>
            {
                Status = StatusCodes.Status403Forbidden.ToString(),
                Message = "Không có quyền truy cập: Bạn không có vai trò phù hợp.",
                Data = null
            };

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);

        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            BaseResponse<object> errorResponse;

            switch (exception)
            {
                case ModelValidationException modelValidationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new BaseResponse<object>
                    {
                        Status = StatusCodes.Status400BadRequest.ToString(),
                        Message = $"[{string.Join(", ", modelValidationEx.Errors.SelectMany(kvp => kvp.Value))}]",
                        Data = null
                    };
                    break;
                case BadHttpRequestException badRequestEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new BaseResponse<object>
                    {
                        Status = StatusCodes.Status400BadRequest.ToString(),
                        Message = badRequestEx.Message,
                        Data = null
                    };
                    break;
                case ForbiddentException forbiddentEx:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse = new BaseResponse<object>
                    {
                        Status = StatusCodes.Status403Forbidden.ToString(),
                        Message = forbiddentEx.Message,
                        Data = null
                    };
                    break;
                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new BaseResponse<object>
                    {
                        Status = StatusCodes.Status404NotFound.ToString(),
                        Message = notFoundEx.Message,
                        Data = null
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new BaseResponse<object>
                    {
                        Status = StatusCodes.Status500InternalServerError.ToString(),
                        Message = "Lỗi hệ thống: " + exception.Message + exception.StackTrace + exception.ToString(),
                        Data = null
                    };
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
