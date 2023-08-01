// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.Json.Serialization;
using ActivityPub.Types.Json;

namespace ActivityPub.Types.Extended.Activity;

/// <summary>
/// Indicates that the actor has created the object.
/// </summary>
public class CreateActivity : ASTransitiveActivity
{
    private CreateActivityEntity Entity { get; }
    
    public CreateActivity() => Entity = new CreateActivityEntity(TypeMap);
    public CreateActivity(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<CreateActivityEntity>();
}


/// <inheritdoc cref="CreateActivity"/>
[ASTypeKey(CreateType)]
public sealed class CreateActivityEntity : ASBase
{
    public const string CreateType = "Create";

    public CreateActivityEntity(TypeMap typeMap) : base(CreateType, typeMap) {}
}