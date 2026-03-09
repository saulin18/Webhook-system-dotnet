using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Domain.Webhooks;
using Infrastructure.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.WebHookDispatcher;


public sealed class WebHookDispatcherMassTransit(
    IPublishEndpoint endpoint
)
{
    public async Task DispatchAsync<T>(string eventType, T payload) where T : notnull
    {
        await endpoint.Publish(new WebHookDispatcherMessage(eventType, payload));
    }
}