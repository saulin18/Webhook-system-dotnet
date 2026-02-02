
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Serilog;

namespace Application.Extensions;

public interface IPagedResult<T>
{
    List<T> Items { get; }
    int TotalCount { get; }
    int Page { get; }
    int PageSize { get; }
}

public enum PaginationDirection
{
    Forward,
    Backward
}

public interface ICursorPagedResult<T>
{
    List<T> Items { get; }
    string? NextCursor { get; }
    string? PreviousCursor { get; }
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
    int PageSize { get; }
}

public sealed record CursorPagedResult<T>(List<T> Items, string? NextCursor, string? PreviousCursor, 
bool HasNextPage, bool HasPreviousPage, int PageSize) : ICursorPagedResult<T>;

public sealed record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize) : IPagedResult<T>;

public static class QueryableExtensions
{   
    //Offset pagination just in case
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        int totalCount = await query.CountAsync(cancellationToken);

        List<T> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        (items, totalCount, page, pageSize);
    }

public static async Task<CursorPagedResult<T>> ToCursorPagedResultAsync<T, TKey>(
    this IQueryable<T> query,
    Expression<Func<T, TKey>> keySelector,
    string? cursor,
    int pageSize,
    PaginationDirection direction = PaginationDirection.Forward,
    CancellationToken cancellationToken = default) where TKey : IComparable<TKey>
{
    try
    {
        Log.Information($"[Pagination] Starting pagination with cursor: '{cursor}', pageSize: {pageSize}, direction: {direction}");
        
        List<T> items = [];
        string? nextCursor = null;
        string? previousCursor = null;

        if (!string.IsNullOrEmpty(cursor))
        {
            Log.Information($"[Pagination] Processing cursor: {cursor}");
            TKey cursorValue = DecodeCursor<TKey>(cursor);
            Log.Information($"[Pagination] Decoded cursor value: {cursorValue}");
            
            if (direction == PaginationDirection.Forward)
            {
                query = query.Where(CreateGreaterThanExpression(keySelector, cursorValue));
                Log.Information($"[Pagination] Applied forward filter");
            }
            else
            {
                query = query.Where(CreateLessThanExpression(keySelector, cursorValue));
                Log.Information($"[Pagination] Applied backward filter");
            }
        }

        // Check if the query already has an order applied
            bool hasExistingOrder = HasExistingOrder(query);
        
        if (direction == PaginationDirection.Forward)
        {
            if (!hasExistingOrder)
            {
                query = query.OrderBy(keySelector);
                Log.Information($"[Pagination] Applied forward ordering");
            }
            else
            {
                Log.Information($"[Pagination] Order already applied, preserving existing order");
            }
        }
        else
        {
            if (!hasExistingOrder)
            {
                query = query.OrderByDescending(keySelector);
                Log.Information($"[Pagination] Applied backward ordering");
            }
            else
            {
                Log.Information($"[Pagination] Order already applied, preserving existing order");
            }
        }

        // Get one extra item to check if there's a next page
        Log.Information($"[Pagination] Executing query with Take({pageSize + 1})");
        items = await query.Take(pageSize + 1).ToListAsync(cancellationToken);
        Log.Information($"[Pagination] Retrieved {items.Count} items");

        bool hasNextPage = items.Count > pageSize;
        bool hasPreviousPage = !string.IsNullOrEmpty(cursor);

        if (hasNextPage)
        {
            items.RemoveAt(items.Count - 1); // Remove the extra item
            Log.Information($"[Pagination] Removed extra item, final count: {items.Count}");
        }

        if (items.Any())
        {
                Func<T, TKey> keyCompiled = keySelector.Compile();
            
            if (direction == PaginationDirection.Forward)
            {
                nextCursor = hasNextPage ? EncodeCursor(keyCompiled(items.Last())) : null;
                previousCursor = hasPreviousPage ? EncodeCursor(keyCompiled(items.First())) : null;
            }
            else
            {
                // For backward pagination, reverse the items and cursors
                items.Reverse();
                nextCursor = hasPreviousPage ? EncodeCursor(keyCompiled(items.Last())) : null;
                previousCursor = hasNextPage ? EncodeCursor(keyCompiled(items.First())) : null;
            }
            
            Log.Information($"[Pagination] Generated cursors - Next: {nextCursor}, Previous: {previousCursor}");
        }

        var result = new CursorPagedResult<T>
        (items, nextCursor, previousCursor, hasNextPage, hasPreviousPage, pageSize);
        
        Log.Information($"[Pagination] Pagination completed successfully. Items: {items.Count}, HasNext: {hasNextPage}, HasPrevious: {hasPreviousPage}");
        return result;
    }
    catch (Exception ex)
    {
        Log.Information($"[Pagination] ERROR in ToCursorPagedResultAsync: {ex.Message}");
        Log.Information($"[Pagination] Stack trace: {ex.StackTrace}");
        throw;
    }
}

 
    private static bool HasExistingOrder<T>(IQueryable<T> query)
    {
        try
        {
            
            Expression expression = query.Expression;
            
            if (expression.Type.IsGenericType && 
                expression.Type.GetGenericTypeDefinition() == typeof(IOrderedQueryable<>))
            {
                Log.Information($"[Pagination] Detected IOrderedQueryable type: {expression.Type}");
                return true;
            }

            string expressionString = expression.ToString();
            bool hasOrderBy = expressionString.Contains("OrderBy") || expressionString.Contains("OrderByDescending");
            
            if (hasOrderBy)
            {
                Log.Information($"[Pagination] Detected OrderBy in expression: {expressionString}");
            }
            
            return hasOrderBy;
        }
        catch (Exception ex)
        {
            Log.Information($"[Pagination] Error detecting existing order: {ex.Message}");
            return false;
        }
    }

    private static Expression<Func<T, bool>> CreateGreaterThanExpression<T, TKey>(
        Expression<Func<T, TKey>> keySelector, TKey value)
    {
        ParameterExpression parameter = keySelector.Parameters[0];
        Expression property = keySelector.Body;
        ConstantExpression constant = Expression.Constant(value, typeof(TKey));
        BinaryExpression greaterThan = Expression.GreaterThan(property, constant);
        
        return Expression.Lambda<Func<T, bool>>(greaterThan, parameter);
    }

    private static Expression<Func<T, bool>> CreateLessThanExpression<T, TKey>(
        Expression<Func<T, TKey>> keySelector, TKey value)
    {
        ParameterExpression parameter = keySelector.Parameters[0];
        Expression property = keySelector.Body;
        ConstantExpression constant = Expression.Constant(value, typeof(TKey));
        BinaryExpression lessThan = Expression.LessThan(property, constant);
        
        return Expression.Lambda<Func<T, bool>>(lessThan, parameter);
    }

    private static string EncodeCursor<T>(T value)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(value);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private static T DecodeCursor<T>(string cursor)
    {
        byte[] bytes = Convert.FromBase64String(cursor);
        string json = System.Text.Encoding.UTF8.GetString(bytes);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json)!;
    }
}

