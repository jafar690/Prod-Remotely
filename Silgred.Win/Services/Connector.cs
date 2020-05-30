using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Silgred.Win.Services
{
    public class Connector
    {
        public HubConnection Connection { get; private set; }

        public async Task Connect(string host)
        {
            if (Connection != null)
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }

            Connection = new HubConnectionBuilder()
                .WithUrl($"{host}/RCDeviceHub")
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();
            await Connection.StartAsync();
        }
    }
}