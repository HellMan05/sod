using Robust.Shared.Prototypes;

namespace Content.Shared._Adventure.Sponsors;

/// <summary>
/// Like <see cref="SponsorTierPrototype">, but can be multiple on one player.
/// </summary>
[Prototype]
public sealed partial class SubSponsorTierPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = default!;

    [DataField]
    public string? DiscordRoleId { get; private set; } = null;
}
