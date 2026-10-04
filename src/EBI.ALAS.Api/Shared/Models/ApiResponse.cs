namespace EBI.ALAS.Api.Shared.Models;

public sealed record ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse SuccessResponse(string message = "Operation completed")
        => new() { Success = true, Message = message };

    public static ApiResponse FailureResponse(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors?.ToList() ?? [] };

    public static ApiResponse FailureResponse(string message, string error)
        => new() { Success = false, Message = message, Errors = [error] };
}

public sealed record ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors?.ToList() ?? [] };

    public static ApiResponse<T> FailureResponse(string message, string error)
        => new() { Success = false, Message = message, Errors = [error] };
}

public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult() { }

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}

public sealed record PaginationRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 15;
}