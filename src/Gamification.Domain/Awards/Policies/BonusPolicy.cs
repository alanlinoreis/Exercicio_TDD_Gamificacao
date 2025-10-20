using System;
using Gamification.Domain.Model;

namespace Gamification.Domain.Awards.Policies;
public sealed class BonusPolicy
{
    public static (XpAmount xp, string justification) Calculate(
        DateTimeOffset now,
        DateTimeOffset? bonusStartDate,
        DateTimeOffset? bonusFullWeightEndDate,
        DateTimeOffset? bonusFinalDate,
        int xpBase,
        int xpFullWeight,
        int xpReducedWeight)
    {
        // Validate configuration
        if (bonusStartDate.HasValue && bonusFinalDate.HasValue && bonusStartDate > bonusFinalDate)
            throw new ArgumentException("bonusStartDate must be <= bonusFinalDate");

        // Before start => full? We'll consider bonusStartDate as inclusive start of bonus window
        if (bonusFullWeightEndDate.HasValue && now <= bonusFullWeightEndDate.Value)
        {
            return (new XpAmount(xpFullWeight), "bonus_full");
        }

        if (bonusFinalDate.HasValue && bonusFullWeightEndDate.HasValue && now > bonusFullWeightEndDate.Value && now <= bonusFinalDate.Value)
        {
            return (new XpAmount(xpReducedWeight), "bonus_reduced");
        }

        // No bonus
        return (XpAmount.Zero, "no_bonus");
    }
}
