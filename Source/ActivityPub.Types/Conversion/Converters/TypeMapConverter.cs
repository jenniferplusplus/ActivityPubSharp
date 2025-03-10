﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion.Overrides;
using ActivityPub.Types.Internal;
using ActivityPub.Types.Util;

namespace ActivityPub.Types.Conversion.Converters;

/// <inheritdoc />
public class TypeMapConverter : JsonConverter<TypeMap>
{
    /// <summary>
    ///     Chain of contexts that inherit from each other.
    ///     If a value is present, then it should be the parent of the current object's context.
    /// </summary>
    private NestedContextStack NestedContextStack { get; } = new();
    
    /// <inheritdoc />
    public TypeMapConverter()
    {
        _asTypeInfoCache = ASTypeInfoCache.Instance;
        _conversionOptions = new ConversionOptions();
    }

    /// <inheritdoc />
    public override TypeMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read input into temporary object
        var jsonElement = JsonElement.ParseValue(ref reader);

        return jsonElement.ValueKind switch
        {
            // String input is a special case of Link
            JsonValueKind.String => ReadString(jsonElement),
            
            // Object input can be anything
            JsonValueKind.Object => ReadObject(jsonElement, options),
          
            // Any other input is an error  
            _ => throw new JsonException($"Can't convert TypeMap from {jsonElement.ValueKind}")
        };
    }
    
    private TypeMap ReadString(JsonElement jsonElement)
    {
        // Construct the JSON-LD context
        var parentContext = NestedContextStack.Peek();
        var context = JsonLDContext.CreateASContext(parentContext);
        
        // Create and prep the type graph
        var typeMap = new TypeMap(context);
        typeMap.Extend<ASType, ASTypeEntity>();
        
        // Read Link from string
        var link = new ASLink(typeMap)
        {
            HRef = jsonElement.GetString()!
        };

        return link.TypeMap;
    }

    private TypeMap ReadObject(JsonElement jsonElement, JsonSerializerOptions options)
    {
        var typeGraphReader = new TypeGraphReader(options, jsonElement, NestedContextStack);
        return new TypeMap(typeGraphReader);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TypeMap typeMap, JsonSerializerOptions options)
    {
        // Construct meta
        var meta = new SerializationMetadata
        {
            TypeMap = typeMap,
            JsonSerializerOptions = options,
            JsonNodeOptions = options.ToNodeOptions()
        };

        // Links require special handling.
        // If the *only* property is href, then they compact to a string
        if (TryWriteAsLink(writer, typeMap))
            return;

        // Create node to hold the output
        var outputNode = new JsonObject(meta.JsonNodeOptions);

        // Write the TypeMap's own properties into the node
        WriteTypeMap(typeMap, outputNode, meta);

        // Write all entities into the node
        foreach (var (entityType, entity) in typeMap.AllEntities)
        {
            // Serialize with default logic
            WriteEntity(entity, entityType, outputNode, meta);
        }

        // Write the node
        outputNode.WriteTo(writer, options);
    }

    private static void WriteEntity(ASEntity entity, Type entityType, JsonObject outputNode, SerializationMetadata meta)
    {
        // Convert to an intermediate object.
        // This will contain the subset of fields that are owned by this entity.
        var element = JsonSerializer.SerializeToElement(entity, entityType, meta.JsonSerializerOptions);
        
        // Sanity check
        if (element.ValueKind != JsonValueKind.Object)
            throw new JsonException($"Failed to write {entityType} to object - serialization produced unsupported JSON type {element.ValueKind}");

        // Copy all properties
        foreach (var property in element.EnumerateObject())
        {
            var valueNode = property.Value.ToNode(meta.JsonNodeOptions);
            outputNode[property.Name] = valueNode;
        }
    }

    private static void WriteTypeMap(TypeMap typeMap, JsonObject outputNode, SerializationMetadata meta)
    {
        // "type" - AS / AP types. Can be string or array.
        if (typeMap.ASTypes.Count > 0)
            outputNode["type"] = JsonSerializer.SerializeToNode(typeMap.ASTypes, meta.JsonSerializerOptions);

        // "@context" - JSON-LD context. Can be string, array, or object.
        if (typeMap.LDContext.Count > 0)
            outputNode["@context"] = JsonSerializer.SerializeToNode(typeMap.LDContext, meta.JsonSerializerOptions);

        // Unmapped (overflow) JSON properties
        if (typeMap.UnmappedProperties != null)
            foreach (var (key, value) in typeMap.UnmappedProperties)
                outputNode[key] = value.ToNode(meta.JsonNodeOptions);
    }

    private static bool TryWriteAsLink(Utf8JsonWriter writer, TypeMap typeMap)
    {
        // If there are any unmapped properties, then bail
        if (typeMap.UnmappedProperties is { Count: > 0 })
            return false;

        // If there is any data in any link entities, then bail
        if (typeMap.AllEntities.Values.Any(link => link.RequiresObjectForm))
            return false;

        // If there is no ASLinkEntity, then bail
        if (!typeMap.IsModel<ASLink>(out var linkModel))
            return false;

        // Finally - its safe to write string form
        var href = linkModel.HRef.ToString();
        writer.WriteStringValue(href);
        return true;
    }
}