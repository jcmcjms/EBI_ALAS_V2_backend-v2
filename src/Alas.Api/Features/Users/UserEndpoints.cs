using System.Security.Cryptography;
using Alas.Api.Composition;
using FluentValidation;

namespace Alas.Api.Features.Users;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", HandleGetUsers)
            .WithName("GetUsers")
            .RequireAuthorization();

        group.MapGet("/{id:int}", HandleGetUserById)
            .WithName("GetUserById")
            .RequireAuthorization();

        group.MapPost("/", HandleCreateUser)
            .WithName("CreateUser")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .RequireAuthorization();

        group.MapPut("/{id:int}", HandleUpdateUser)
            .WithName("UpdateUser")
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPatch("/{id:int}/status", HandleUpdateUserStatus)
            .WithName("UpdateUserStatus")
            .RequireAuthorization();

        group.MapPost("/{id:int}/reset-password", HandleResetPassword)
            .WithName("ResetUserPassword")
            .RequireAuthorization();

        group.MapPost("/{id:int}/force-password-reset", HandleForcePasswordReset)
            .WithName("ForcePasswordReset")
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleGetUsers(
        [AsParameters] UserQueryParameters parameters,
        IUserService userService,
        CancellationToken ct)
    {
        var result = await userService.GetUsersAsync(parameters, ct);
        return Results.Ok(ApiResponse<PagedResult<UserResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetUserById(
        int id,
        IUserService userService,
        CancellationToken ct)
    {
        var user = await userService.GetUserByIdAsync(id, ct);
        return user is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<UserResponse>.SuccessResponse(user));
    }

    private static async Task<IResult> HandleCreateUser(
        CreateUserRequest request,
        IValidator<CreateUserRequest> validator,
        IUserService userService,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            var user = await userService.CreateUserAsync(request, ct);
            return Results.Created($"/api/users/{user.Id}", ApiResponse<UserResponse>.SuccessResponse(user, "User created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Problem(
                title: "Conflict",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.10");
        }
    }

    private static async Task<IResult> HandleUpdateUser(
        int id,
        UpdateUserRequest request,
        IValidator<UpdateUserRequest> validator,
        IUserService userService,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var user = await userService.UpdateUserAsync(id, request, ct);
        return user is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<UserResponse>.SuccessResponse(user, "User updated successfully"));
    }

    private static async Task<IResult> HandleUpdateUserStatus(
        int id,
        UserStatusRequest request,
        IUserService userService,
        CancellationToken ct)
    {
        var success = await userService.UpdateUserStatusAsync(id, request.IsActive, ct);
        return success
            ? Results.Ok(ApiResponse.SuccessResponse($"User status updated to {(request.IsActive ? "Active" : "Suspended")}"))
            : Results.NotFound();
    }

    private static async Task<IResult> HandleResetPassword(
        int id,
        ResetPasswordRequest? request,
        IUserService userService,
        CancellationToken ct)
    {
        var supplied = request?.NewPassword;
        var newPassword = string.IsNullOrWhiteSpace(supplied)
            ? GenerateTemporaryPassword()
            : supplied;

        try
        {
            var result = await userService.ResetPasswordAsync(id, newPassword, ct);
            return Results.Ok(ApiResponse<ResetPasswordResponse>.SuccessResponse(result, "Password reset successfully"));
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> HandleForcePasswordReset(
        int id,
        IUserService userService,
        CancellationToken ct)
    {
        var success = await userService.ForcePasswordResetAsync(id, ct);
        return success
            ? Results.Ok(ApiResponse.SuccessResponse("User will be required to change password on next login"))
            : Results.NotFound();
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        const string all = upper + lower + digits + special;

        var password = new char[16];
        password[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        password[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

        for (int i = 4; i < password.Length; i++)
            password[i] = all[RandomNumberGenerator.GetInt32(all.Length)];

        return new string(password);
    }
}

