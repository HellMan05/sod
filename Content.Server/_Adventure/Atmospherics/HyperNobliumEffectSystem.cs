using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
/// Эффект газа гиперноблиум - полное подавление всех химических реакций при наличии ≥5 молей
/// </summary>
[UsedImplicitly]
public sealed partial class HypernobliumEffect : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        // Получаем текущее количество Hypernoblium в смеси
        var hypernobliumAmount = mixture.GetMoles(Gas.HyperNoblium);

        // Если Hypernoblium достаточно - останавливаем все реакции
        if (hypernobliumAmount >= Atmospherics.HyperNobliumFullSuppressionThreshold)
        {
            // Очищаем результаты всех реакций
            Array.Clear(mixture.ReactionResults, 0, mixture.ReactionResults.Length);

            // Возвращаем флаг остановки всех последующих реакций
            return ReactionResult.StopReactions;
        }

        return ReactionResult.NoReaction;
    }
}

