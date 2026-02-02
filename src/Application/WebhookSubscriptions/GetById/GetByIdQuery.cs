using Application.Abstractions.Messaging;

namespace Application.Webhooks.GetById;

public sealed record GetByIdQuery(Guid Id) : IQuery<GetByIdResponseDto>;