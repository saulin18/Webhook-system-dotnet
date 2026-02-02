using Domain.Webhooks;

namespace Application.Webhooks.GetById;

public sealed record GetByIdResponseDto(Guid Id, string Url, string EventType, bool IsActive, DateTime CreatedAt);