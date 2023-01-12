using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Newtonsoft.Json;
using System.Text;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.Services.AzureServiceBus;
internal sealed class IdpEventsCreatorFactory
{

    public (bool success, BaseIntegratedEvent? createdEvent) GetEvent(ServiceBusReceivedMessage message)
    {

        var eventType = message.ApplicationProperties["eventType"];
        if (eventType == null)
            return (false, null);

        BaseIntegratedEvent? createdEvent = null;

        switch (eventType)
        {
            case nameof(UserAdded):
                createdEvent = JsonConvert.DeserializeObject<UserAdded>(Encoding.UTF8.GetString(message.Body));
                return (true, createdEvent);

            case nameof(UserDeleted):
                createdEvent = JsonConvert.DeserializeObject<UserDeleted>(Encoding.UTF8.GetString(message.Body));
                return (true, createdEvent);

            case nameof(OrganizationDeleted):
                createdEvent = JsonConvert.DeserializeObject<OrganizationDeleted>(Encoding.UTF8.GetString(message.Body));
                return (true, createdEvent);
        }


        return (false, createdEvent);
    }
}
