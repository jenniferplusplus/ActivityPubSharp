// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.


using System.Diagnostics.CodeAnalysis;

namespace ActivityPub.Types.AS.Extended.Activity;

/// <summary>
///     Indicates that the actor has created the object.
/// </summary>
public class CreateActivity : ASActivity, IASModel<CreateActivity, CreateActivityEntity, ASActivity>
{
    /// <summary>
    ///     ActivityStreams type name for "Create" types.
    /// </summary>
    public const string CreateType = "Create";
    static string IASModel<CreateActivity>.ASTypeName => CreateType;

    /// <inheritdoc />
    public CreateActivity() => Entity = TypeMap.Extend<CreateActivityEntity>();

    /// <inheritdoc />
    public CreateActivity(TypeMap typeMap, bool isExtending = true) : base(typeMap, false)
        => Entity = TypeMap.ProjectTo<CreateActivityEntity>(isExtending);

    /// <inheritdoc />
    public CreateActivity(ASType existingGraph) : this(existingGraph.TypeMap) {}

    /// <inheritdoc />
    [SetsRequiredMembers]
    public CreateActivity(TypeMap typeMap, CreateActivityEntity? entity) : base(typeMap, null)
        => Entity = entity ?? typeMap.AsEntity<CreateActivityEntity>();

    static CreateActivity IASModel<CreateActivity>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    private CreateActivityEntity Entity { get; }
}

/// <inheritdoc cref="CreateActivity" />
public sealed class CreateActivityEntity : ASEntity<CreateActivity, CreateActivityEntity> {}