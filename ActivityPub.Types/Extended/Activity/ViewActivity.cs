// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.Json.Serialization;
using ActivityPub.Types.Json;

namespace ActivityPub.Types.Extended.Activity;

/// <summary>
/// Indicates that the actor has viewed the object. 
/// </summary>
public class ViewActivity : ASTransitiveActivity
{
    private ViewActivityEntity Entity { get; }
    
    public ViewActivity() => Entity = new ViewActivityEntity(TypeMap);
    public ViewActivity(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<ViewActivityEntity>();
}


/// <inheritdoc cref="ViewActivity"/>
[ASTypeKey(ViewType)]
public sealed class ViewActivityEntity : ASBase
{
    public const string ViewType = "View";

    public ViewActivityEntity(TypeMap typeMap) : base(ViewType, typeMap) {}
}