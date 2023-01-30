using MediatR;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.VideoDurationCalculator;

internal sealed record VideoDurationCalculated(string ResourceName, int Duration): INotification;