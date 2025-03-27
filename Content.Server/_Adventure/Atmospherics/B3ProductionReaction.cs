using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Синтез Б3 из плазмы и оксида азота.
///     Имеется лимит по давлению, если превышает 40КПа, реакция прекращается.
/// </summary>
[UsedImplicitly]
public sealed partial class B3ProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var initialPlasma = mixture.GetMoles(Gas.Plasma);
        var initialN20 = mixture.GetMoles(Gas.NitrousOxide);

        if (initialPlasma <= 0 || initialN20 <= 0)
            return ReactionResult.NoReaction;

        var plasmaLimit = initialPlasma / Atmospherics.B3PlasmaRatio;
        var n20Limit = initialN20 / Atmospherics.B3N20Ratio;
        var limitingFactor = Math.Min(plasmaLimit, n20Limit);

        if (limitingFactor <= 0)
            return ReactionResult.NoReaction;

        var plasmaBurned = limitingFactor * Atmospherics.B3PlasmaRatio;
        var n20Burned = limitingFactor * Atmospherics.B3N20Ratio;
        var b3Produced = (plasmaBurned + n20Burned) * Atmospherics.B3SynthesisEfficiency;

        mixture.AdjustMoles(Gas.Plasma, -plasmaBurned);
        mixture.AdjustMoles(Gas.NitrousOxide, -n20Burned);
        mixture.AdjustMoles(Gas.B3, b3Produced);

        return ReactionResult.Reacting;
    }
}
