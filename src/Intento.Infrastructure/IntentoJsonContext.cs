using System.Text.Json.Serialization;
using Intento.Core.Models;

namespace Intento.Infrastructure;

/// <summary>
/// Source-generated JSON metadata, so persistence keeps working when the app is published trimmed.
/// Produces the same JSON format as the previous reflection-based serializer.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Activity>))]
internal sealed partial class IntentoJsonContext : JsonSerializerContext
{
}
