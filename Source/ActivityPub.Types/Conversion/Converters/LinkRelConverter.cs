﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.Json;
using System.Text.Json.Serialization;
using ActivityPub.Types.Util;

namespace ActivityPub.Types.Conversion.Converters;

/// <summary>
///     Custom JSON converter for <see cref="LinkRel"/>.
/// </summary>
public class LinkRelConverter : JsonConverter<LinkRel>
{
    /// <inheritdoc />
    public override LinkRel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
    {
        JsonTokenType.Null => null,
        JsonTokenType.String => new LinkRel(reader.GetString()!),
        var badType => throw new JsonException($"Cannot parse LinkRel from {badType}")
    };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, LinkRel value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}