using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;
/// <summary>
/// Реакция синтеза хилиума из фрезона и бз.
/// </summary>
[UsedImplicitly]
public sealed partial class HealiumProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.HealiumMinTemperature ||
            mixture.Temperature > Atmospherics.HealiumMaxTemperature)
            return ReactionResult.NoReaction;

        var initialFrezon = mixture.GetMoles(Gas.Frezon);
        var initialBZ = mixture.GetMoles(Gas.BZ);

        var availableMix = (initialFrezon + initialBZ) / Atmospherics.HealiumConversionRatio;
        var limitingFactor = Math.Min(availableMix, Math.Min(initialFrezon, initialBZ));

        if (limitingFactor <= 0)
            return ReactionResult.NoReaction;

        var frezonBurned = limitingFactor;
        var bzBurned = limitingFactor;
        var healiumProduced = limitingFactor * 0.25f;

        mixture.AdjustMoles(Gas.Frezon, -frezonBurned);
        mixture.AdjustMoles(Gas.BZ, -bzBurned);
        mixture.AdjustMoles(Gas.Healium, healiumProduced);

        var temperatureFactor = MathHelper.Clamp(
            (mixture.Temperature - Atmospherics.HealiumMinTemperature) /
            (Atmospherics.HealiumMaxTemperature - Atmospherics.HealiumMinTemperature),
            0.5f, 2f);

        var energyReleased = healiumProduced * Atmospherics.HealiumFormationEnergy * temperatureFactor;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyReleased / heatCapacity;

        return ReactionResult.Reacting;
    }
}
