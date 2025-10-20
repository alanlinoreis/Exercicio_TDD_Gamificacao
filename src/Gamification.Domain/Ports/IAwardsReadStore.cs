using System;
using System.Threading.Tasks;
using Gamification.Domain.Model;

namespace Gamification.Domain.Ports;
public record ThemeBonusDates(DateTimeOffset? BonusStartDate, DateTimeOffset? BonusFullWeightEndDate, DateTimeOffset? BonusFinalDate, int XpBase, int XpFullWeight, int XpReducedWeight);

public interface IAwardsReadStore
{
    ValueTask<bool> MissaoConcluidaAsync(string studentId, string missionId);
    ValueTask<bool> JaConcedidaAsync(string studentId, string themeId, string missionId, string badgeSlug);
    ValueTask<bool> RequestIdExistsAsync(string requestId);
    ValueTask<ThemeBonusDates?> GetThemeBonusDatesAsync(string themeId);
}
