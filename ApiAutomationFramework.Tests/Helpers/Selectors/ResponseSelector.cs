using ApiAutomationFramework.DTOs.Response;

namespace ApiAutomationFramework.Helpers.Selectors;

/// <summary>
/// Provides advanced selection and filtering capabilities on API responses.
/// Follows the Specification Pattern for reusable filter logic.
/// </summary>
public class ResponseSelector
{
    /// <summary>
    /// Filter users by criteria using LINQ-style predicates.
    /// </summary>
    public List<UserData> SelectUsers(
        UsersListResponse response,
        Func<UserData, bool> predicate)
    {
        return response.Data.Where(predicate).ToList();
    }

    /// <summary>
    /// Select users by domain.
    /// </summary>
    public List<UserData> SelectUsersByEmailDomain(
        UsersListResponse response,
        string domain)
    {
        return response.Data
            .Where(u => u.Email.EndsWith($"@{domain}", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Select posts matching multiple criteria.
    /// </summary>
    public List<PostResponse> SelectPosts(
        List<PostResponse> posts,
        int? userId = null,
        string? titleContains = null,
        int? minBodyLength = null)
    {
        var query = posts.AsEnumerable();

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        if (!string.IsNullOrEmpty(titleContains))
            query = query.Where(p =>
                p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));

        if (minBodyLength.HasValue)
            query = query.Where(p => p.Body.Length >= minBodyLength.Value);

        return query.ToList();
    }

    /// <summary>
    /// Group posts by user.
    /// </summary>
    public Dictionary<int, List<PostResponse>> GroupPostsByUser(
        List<PostResponse> posts)
    {
        return posts
            .GroupBy(p => p.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Get top N posts by body length.
    /// </summary>
    public List<PostResponse> GetTopPostsByLength(
        List<PostResponse> posts,
        int topN = 5)
    {
        return posts
            .OrderByDescending(p => p.Body.Length)
            .Take(topN)
            .ToList();
    }

    /// <summary>
    /// Complex selection with pagination and sorting.
    /// </summary>
    public PaginatedResult<T> SelectWithPagination<T>(
        IEnumerable<T> source,
        int page = 1,
        int pageSize = 10,
        Func<T, object>? orderBy = null,
        bool descending = false)
    {
        var query = source.AsEnumerable();

        if (orderBy != null)
        {
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        var totalCount = query.Count();
        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}