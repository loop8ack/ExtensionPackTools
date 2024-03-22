using System.Text.Json;
using System.Text.Json.Serialization;

using ExtensionManager.Manifest.Internal.Models;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Manifest.Internal.Versions;

internal class V1ManifestVersion : ManifestVersion
{
    public V1ManifestVersion()
        : base(new(1, 0))
    {
    }

    public override async Task<IManifest> ReadAsync(Stream stream)
    {
        var data = await JsonSerializer
            .DeserializeAsync<JsonManifest>(stream, (JsonSerializerOptions?)null)
            .ConfigureAwait(false);

        if (data is null)
            throw ThrowHelper.CreateCannotReadManifestException();

        return CreateManifestDto(data);

        static ManifestDto CreateManifestDto(JsonManifest data)
        {
            var extensions = new List<IVSExtension>();

            if (data.Extensions is not null)
            {
                foreach (var ext in data.Extensions)
                    extensions.Add(CreateExtensionDto(ext));
            }

            return new ManifestDto()
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                Extensions = extensions,
            };
        }

        static ExtensionDto CreateExtensionDto(JsonExtension data)
        {
            return new ExtensionDto()
            {
                Id = data.ID ?? throw ThrowHelper.CreateExtensionDataHasNoVsixIdException(),
                Name = data.Name,
                MoreInfoURL = data.MoreInfoURL,
                DownloadUrl = data.DownloadUrl,
            };
        }
    }

    public override async Task WriteAsync(Stream stream, IManifest manifest, CancellationToken cancellationToken)
    {
        var data = new JsonManifest(manifest);

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        await JsonSerializer.SerializeAsync(stream, data, options, cancellationToken);
    }
}

file sealed class JsonManifest
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName(ManifestVersion.VersionPropertyName)]
    public Version Version => new(1, 0);

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("extensions")]
    public List<JsonExtension>? Extensions { get; set; }

    [JsonConstructor]
    public JsonManifest() { }

    public JsonManifest(IManifest dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
        Extensions = new List<JsonExtension>();

        foreach (var ext in dto.Extensions)
            Extensions.Add(new(ext));
    }
}

file sealed class JsonExtension
{
    [JsonPropertyName("vsixId")]
    public string? ID { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("moreInfoUrl")]
    public string? MoreInfoURL { get; set; }

    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }

    [JsonConstructor]
    public JsonExtension() { }

    public JsonExtension(IVSExtension dto)
    {
        ID = dto.Id;
        Name = dto.Name;
        MoreInfoURL = dto.MoreInfoURL;
        DownloadUrl = dto.DownloadUrl;
    }

}
