using Content.Server.Connection.Whitelist;
using Content.Shared.Database;
using Content.Shared._Adventure.Sponsors;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Connection.Whitelist.Conditions;

/// <summary>
/// Condition that matches if the player has subsponsor tier.
/// </summary>
public sealed partial class ConditionSubsponsorMatch : WhitelistCondition
{
    [DataField]
    public ProtoId<SubSponsorTierPrototype> Subsponsor;
}
