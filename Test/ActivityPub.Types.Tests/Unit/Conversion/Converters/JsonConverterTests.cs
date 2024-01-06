﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text;
using System.Text.Json.Serialization;

namespace ActivityPub.Types.Tests.Unit.Conversion.Converters;

public abstract class JsonConverterTests<T, TConverter>
    where TConverter : JsonConverter<T>
{
    protected abstract TConverter ConverterUnderTest { get; set; }
    protected JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonSerializerOptions.Default;

    protected T? Read(string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json).AsSpan();
        return Read(bytes);
    }
    
    // Useful: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-utf8jsonreader
    protected T? Read(ReadOnlySpan<byte> json)
    {
        var reader = new Utf8JsonReader(json);
        reader.Read();

        return ConverterUnderTest.Read(ref reader, typeof(T), JsonSerializerOptions);
    }

    // Useful: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-utf8jsonwriter
    protected string Write(T input)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        ConverterUnderTest.Write(writer, input, JsonSerializerOptions);

        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    protected JsonElement WriteToElement(T input)
    {
        var json = Write(input);
        return JsonSerializer.Deserialize<JsonElement>(json, JsonSerializerOptions);
    }
}