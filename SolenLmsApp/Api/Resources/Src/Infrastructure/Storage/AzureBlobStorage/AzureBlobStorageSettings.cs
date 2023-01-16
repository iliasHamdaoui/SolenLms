namespace Imanys.SolenLms.Application.Resources.Infrastructure.Storage.AzureBlobStorage;
internal sealed class AzureBlobStorageSettings
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = default!;
    public string ResourcesContainerName { get; set; } = default!;
}
