using Application.Abstractions.Messaging;
using Application.Webhooks.GetById;

namespace Application.WebhookSubscriptions.GetById;

public sealed record GetByIdQuery(Guid Id) : IQuery<GetByIdResponseDto>;