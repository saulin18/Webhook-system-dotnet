using Application.Abstractions.Messaging;
using Application.Extensions;

namespace Application.Webhooks.GetAll;

public sealed record GetAllSubscriptionsQuery(int Page, int PageSize, 
string? EventType, string? Url, bool? IsActive) : IQuery<GetAllSubscriptionsResponseDto>;