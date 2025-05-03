using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.Authentication;
using MIDASM.Contract.Constants;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using Rookies.Contract.Exceptions;
using System.Security.Claims;

namespace MIDASM.API.Middlewares;

public class ExecutionContextMiddleware
{
    private readonly RequestDelegate _next;

    public ExecutionContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository,
        IExecutionContext executionContext, IMemoryCache memoryCache)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            Guid.TryParse(context.User.FindFirstValue("sid"), out Guid id);
            Guid.TryParse(context.User.FindFirstValue("jti"), out Guid jti);
            string? bearer = context.Request.Headers["Authorization"].FirstOrDefault();
            string? accessToken = bearer?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ?? false
                ? bearer[7..]
                : null;

            string recallAccessTokenCacheKey = string.Format(CacheKey.RecallTokenKey, jti);
            if (memoryCache.Get(recallAccessTokenCacheKey) is not null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            User? user = await userRepository.GetByIdAsync(id, "Role");
            if (user == null)
            {
                throw new BadRequestException("User does not exists");
            }
            if(!user.IsVerifyCode)
            {
                throw new UnAuthorizedException("User has not been verified code");
            }    

            DateTime dateTimeNow = DateTime.UtcNow;
            if (user!.LastUpdateLimit.Month < dateTimeNow.Month || user!.LastUpdateLimit.Year < dateTimeNow.Year)
            {
                user.BookBorrowingLimit = 3;
                user.LastUpdateLimit = DateOnly.FromDateTime(dateTimeNow);
                userRepository.Update(user);
                await userRepository.SaveChangesAsync();
            }

            UserExecutionContext userExecutionContext = new()
            {
                Id = user!.Id,
                BookBorrowingLimit = user.BookBorrowingLimit,
                Role = new UserRoleExecutionContext { Name = user.Role.Name },
                Email = user.Email,
                Username = user.Username,
            };
            executionContext.SetJti(jti);
            executionContext.SetAccessToken(accessToken!);
            executionContext.SetUser(userExecutionContext);

            var userAgent = context?.Request.Headers["User-Agent"].ToString();

            executionContext.SetUserAgent(userAgent);
        }

        await _next(context);
    }
}