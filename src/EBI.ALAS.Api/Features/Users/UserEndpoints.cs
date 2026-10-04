using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.Users;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/", GetUsersAsync)
            .RequireAuthorization("CanViewUsers")
            .WithName("GetUsers")
            .Produces<ApiResponse<PagedResult<UserListItemDto>>>(200);

        group.MapGet("/{id:int}", GetUserAsync)
            .RequireAuthorization("CanViewUsers")
            .WithName("GetUser")
            .Produces<ApiResponse<UserDetailDto>>(200)
            .Produces<ApiResponse>(404);

        group.MapPost("/", CreateUserAsync)
            .RequireAuthorization("CanCreateUsers")
            .WithName("CreateUser")
            .Produces<ApiResponse<UserDetailDto>>(201)
            .Produces<ApiResponse>(400);

        group.MapPut("/{id:int}", UpdateUserAsync)
            .RequireAuthorization("CanEditUsers")
            .WithName("UpdateUser")
            .Produces<ApiResponse<UserDetailDto>>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(404);

        group.MapPatch("/{id:int}/status", ChangeUserStatusAsync)
            .RequireAuthorization("CanSuspendUsers")
            .WithName("ChangeUserStatus")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(404);
    }

    private static async Task<IResult> GetUsersAsync(
        int? page, int? pageSize, string? search, string? role, string? branchCode, bool? isActive,
        IUserService userService, CancellationToken ct)
    {
        var p = Math.Max(page ?? 1, 1);
        var ps = Math.Clamp(pageSize ?? 15, 1, 100);

        var result = await userService.GetPagedAsync(p, ps, search, role, branchCode, isActive, ct);
        return TypedResults.Ok(ApiResponse<PagedResult<UserListItemDto>>.SuccessResponse(result));
    }

    private static async Task<IResult> GetUserAsync(
        int id, IUserService userService, CancellationToken ct)
    {
        var result = await userService.GetByIdAsync(id, ct);
        if (result.IsFailure)
        {
            return TypedResults.NotFound(ApiResponse<UserDetailDto>.FailureResponse(result.Error.Message, result.Error.Code));
        }
        return TypedResults.Ok(ApiResponse<UserDetailDto>.SuccessResponse(result.Value!));
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest request, IUserService userService, CancellationToken ct)
    {
        var result = await userService.CreateAsync(request, ct);
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(ApiResponse<UserDetailDto>.FailureResponse(result.Error.Message, result.Error.Code));
        }
        return TypedResults.Created($"/api/users/{result.Value!.Id}", ApiResponse<UserDetailDto>.SuccessResponse(result.Value!, "User created"));
    }

    private static async Task<IResult> UpdateUserAsync(
        int id, UpdateUserRequest request, IUserService userService, CancellationToken ct)
    {
        var result = await userService.UpdateAsync(id, request, ct);
        if (result.IsFailure)
        {
            var statusCode = result.Error.Code == "USER_NOT_FOUND" ? 404 : 400;
            if (statusCode == 404)
            {
                return TypedResults.NotFound(ApiResponse<UserDetailDto>.FailureResponse(result.Error.Message, result.Error.Code));
            }
            return TypedResults.BadRequest(ApiResponse<UserDetailDto>.FailureResponse(result.Error.Message, result.Error.Code));
        }
        return TypedResults.Ok(ApiResponse<UserDetailDto>.SuccessResponse(result.Value!));
    }

    private static async Task<IResult> ChangeUserStatusAsync(
        int id, ChangeUserStatusRequest request, IUserService userService, CancellationToken ct)
    {
        var result = await userService.ChangeStatusAsync(id, request, ct);
        if (result.IsFailure)
        {
            var statusCode = result.Error.Code == "USER_NOT_FOUND" ? 404 : 400;
            if (statusCode == 404)
            {
                return TypedResults.NotFound(ApiResponse.FailureResponse(result.Error.Message, result.Error.Code));
            }
            return TypedResults.BadRequest(ApiResponse.FailureResponse(result.Error.Message, result.Error.Code));
        }
        return TypedResults.Ok(ApiResponse.SuccessResponse("User status updated"));
    }
}