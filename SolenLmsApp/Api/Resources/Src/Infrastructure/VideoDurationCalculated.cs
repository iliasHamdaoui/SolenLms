using MediatR;

namespace Imanys.SolenLms.Application.Resources.Infrastructure;

internal sealed record VideoDurationCalculated(string ResourceName, int Duration): INotification;