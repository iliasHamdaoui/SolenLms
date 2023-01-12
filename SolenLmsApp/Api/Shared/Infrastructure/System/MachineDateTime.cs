using Imanys.SolenLms.Application.Shared.Core.Infrastructure;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.System;

internal class MachineDateTime : IDateTime
{
    public DateTime Now => DateTime.Now;
}
