using System;
using System.Threading.Tasks;
using Gamification.Domain.Ports;
using Gamification.Domain.Model;
using Gamification.Domain.Exceptions;
using Gamification.Domain.Awards.Policies;

namespace Gamification.Domain.Awards;
public sealed class AwardBadgeService
{
    private readonly IAwardsReadStore _read;
    private readonly IAwardsWriteStore _write;

    public AwardBadgeService(IAwardsReadStore read, IAwardsWriteStore write)
    {
        _read = read ?? throw new ArgumentNullException(nameof(read));
        _write = write ?? throw new ArgumentNullException(nameof(write));
    }

    public async ValueTask<(Badge badge, XpAmount xp, RewardLog log)> AwardAsync(
        string studentId,
        string themeId,
        string missionId,
        string badgeSlug,
        DateTimeOffset now,
        string? requestId = null)
    {
        // Eligibility
        var concluded = await _read.MissaoConcluidaAsync(studentId, missionId);
        if (!concluded) throw new IneligibleException("Elegibilidade não satisfeita: missão não concluída.");

        // Idempotency by requestId
        if (!string.IsNullOrWhiteSpace(requestId))
        {
            var exists = await _read.RequestIdExistsAsync(requestId);
            if (exists) throw new AlreadyGrantedException("Requisição já processada (requestId).");
        }
        else
        {
            // Natural key check
            var already = await _read.JaConcedidaAsync(studentId, themeId, missionId, badgeSlug);
            if (already) throw new AlreadyGrantedException("Badge já concedida para esta chave natural.");
        }

        // Get theme dates & xp config
        var theme = await _read.GetThemeBonusDatesAsync(themeId);
        if (theme is null) throw new IneligibleException("Tema/missão inexistente ou sem configuração.");

        // Calculate bonus
        var (xp, justification) = BonusPolicy.Calculate(
            now,
            theme.BonusStartDate,
            theme.BonusFullWeightEndDate,
            theme.BonusFinalDate,
            theme.XpBase,
            theme.XpFullWeight,
            theme.XpReducedWeight
        );

        var badge = new Badge(badgeSlug, badgeSlug);
        var log = new RewardLog(studentId, themeId, missionId, badgeSlug, xp, now, "mission_completion", justification);

        // Atomic save
        try
        {
            await _write.SaveConcessaoAtomicAsync(badge, studentId, themeId, missionId, xp, log, requestId);
        }
        catch (Exception ex)
        {
            throw new DomainException($"Falha ao persistir concessão: {ex.Message}");
        }

        return (badge, xp, log);
    }
}
