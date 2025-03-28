using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;
/// <summary>
/// Реакция синтеза гипер-ноблиума из азота и трития.
/// </summary>
[UsedImplicitly]
public sealed partial class HyperNobliumProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.HyperNobliumFormationMinTemp ||
            mixture.Temperature > Atmospherics.HyperNobliumFormationMaxTemp)
            return ReactionResult.NoReaction;

        var initialNitrogen = mixture.GetMoles(Gas.Nitrogen);
        var initialTritium = mixture.GetMoles(Gas.Tritium);
        var initialBZ = mixture.GetMoles(Gas.BZ);

        var totalGas = initialTritium + initialBZ;
        var tritiumReductionFactor = totalGas > 0
            ? Math.Clamp(initialTritium / totalGas, 0.001f, 1f)
            : 1f;


        var nobliumPossible = Math.Min(
            (initialNitrogen + initialTritium) * 0.01f,
            Math.Min(
                initialTritium / (Atmospherics.HyperNobliumFormationTritiumRatio * tritiumReductionFactor),
                initialNitrogen / Atmospherics.HyperNobliumFormationNitrogenRatio
            )
        );

        if (nobliumPossible <= 0 ||
            initialTritium < Atmospherics.HyperNobliumFormationTritiumRatio * nobliumPossible * tritiumReductionFactor ||
            initialNitrogen < Atmospherics.HyperNobliumFormationNitrogenRatio * nobliumPossible)
        {
            return ReactionResult.NoReaction;
        }

        mixture.AdjustMoles(Gas.Nitrogen, -Atmospherics.HyperNobliumFormationNitrogenRatio * nobliumPossible);
        mixture.AdjustMoles(Gas.Tritium, -Atmospherics.HyperNobliumFormationTritiumRatio * nobliumPossible * tritiumReductionFactor);
        mixture.AdjustMoles(Gas.HyperNoblium, nobliumPossible);

        var energyReleased = nobliumPossible *
                           (Atmospherics.HyperNobliumFormationEnergy /
                           Math.Max(initialBZ, 1f));

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            mixture.Temperature = Math.Max(
                (mixture.Temperature * heatCapacity + energyReleased) / heatCapacity,
                Atmospherics.TCMB
            );
        }

        return ReactionResult.Reacting;
    }
}
