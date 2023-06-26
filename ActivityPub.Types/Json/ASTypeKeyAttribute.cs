// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace ActivityPub.Types.Json;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class ASTypeKeyAttribute : Attribute
{
    /// <summary>
    /// ActivityStreams type name
    /// </summary>
    /// <seealso cref="ASType.Types"/>
    public readonly string Type;

    public ASTypeKeyAttribute(string type) => Type = type;
}