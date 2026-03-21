using Domain.Webhooks;
using Application.Abstractions.Messaging;

namespace Application.WebHookDeliveries.GetById;

public sealed record GetByIdQuery(Guid Id) : IQuery<GetByIdResponseDto>;