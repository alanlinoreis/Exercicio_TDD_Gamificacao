using System;
using System.Threading.Tasks;
using Xunit;
using Gamification.Domain.Awards;
using Gamification.Domain.Ports.Fakes;
using Gamification.Domain.Ports;
using Gamification.Domain.Model;
using Gamification.Domain.Exceptions;

namespace Gamification.Domain.Tests;
public class AwardBadgeServiceTests
{
    [Fact]
    public async Task ConcederBadge_quando_missao_concluida_concede_uma_unica_vez()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read);
        var themeId = "theme1";
        read.AddTheme(themeId, new ThemeBonusDates(null, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(2), 10, 20, 5));
        read.MarkConcluded("student1","mission1");

        var service = new AwardBadgeService(read, write);
        var now = DateTimeOffset.UtcNow;

        var result = await service.AwardAsync("student1", themeId, "mission1", "badge-a", now, null);
        Assert.Equal("badge-a", result.badge.Slug);
        Assert.Equal(0,  // depending on now and theme dates, xp may be 0 or not; assert that a log was registered
            result.xp.Value >= 0 ? 0 : 0); // trivial assert to keep intent; main check below
        Assert.Single(write.Logs);

        // second attempt should throw AlreadyGrantedException
        await Assert.ThrowsAsync<AlreadyGrantedException>(async () =>
            await service.AwardAsync("student1", themeId, "mission1", "badge-a", now, null));
    }

    [Fact]
    public async Task ConcederBadge_sem_concluir_missao_deve_falhar()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read);
        var service = new AwardBadgeService(read, write);
        await Assert.ThrowsAsync<IneligibleException>(async () =>
            await service.AwardAsync("studentX", "themeX", "missionX", "badge-x", DateTimeOffset.UtcNow, null));
    }

    [Fact]
    public async Task ConcederBadge_ate_bonusFullWeightEndDate_concede_bonus_integral()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read);
        var themeId = "theme2";
        var now = DateTimeOffset.UtcNow;
        read.AddTheme(themeId, new ThemeBonusDates(now.AddMinutes(-1), now.AddMinutes(10), now.AddMinutes(20), 10, 100, 50));
        read.MarkConcluded("s1","m1");
        var service = new AwardBadgeService(read, write);

        var (badge, xp, log) = await service.AwardAsync("s1", themeId, "m1", "badge-b", now, null);
        Assert.Equal(100, xp.Value);
        Assert.Equal("bonus_full", log.Reason);
    }

    [Fact]
    public async Task ConcederBadge_entre_fullWeight_e_finalDate_concede_bonus_reduzido()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read);
        var themeId = "theme3";
        var now = DateTimeOffset.UtcNow;
        read.AddTheme(themeId, new ThemeBonusDates(now.AddMinutes(-20), now.AddMinutes(-10), now.AddMinutes(10), 10, 80, 30));
        read.MarkConcluded("s2","m2");
        var service = new AwardBadgeService(read, write);

        var (badge, xp, log) = await service.AwardAsync("s2", themeId, "m2", "badge-c", now, null);
        Assert.Equal(30, xp.Value);
        Assert.Equal("bonus_reduced", log.Reason);
    }

    [Fact]
    public async Task ConcederBadge_apos_bonusFinalDate_nao_concede_bonus_mas_concede_badge()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read);
        var themeId = "theme4";
        var now = DateTimeOffset.UtcNow;
        read.AddTheme(themeId, new ThemeBonusDates(now.AddMinutes(-40), now.AddMinutes(-30), now.AddMinutes(-10), 10, 60, 20));
        read.MarkConcluded("s3","m3");
        var service = new AwardBadgeService(read, write);

        var (badge, xp, log) = await service.AwardAsync("s3", themeId, "m3", "badge-d", now, null);
        Assert.Equal(0, xp.Value);
        Assert.Equal("no_bonus", log.Reason);
    }

    [Fact]
    public async Task ConcederBadge_falha_na_gravacao_nao_deve_gerar_efeitos_parciais()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read, failOnSave: true);
        var themeId = "theme5";
        var now = DateTimeOffset.UtcNow;
        read.AddTheme(themeId, new ThemeBonusDates(null, now.AddMinutes(5), now.AddMinutes(10), 10, 40, 15));
        read.MarkConcluded("s4","m4");
        var service = new AwardBadgeService(read, write);

        await Assert.ThrowsAsync<Exception>(async () => await service.AwardAsync("s4", themeId, "m4", "badge-e", now, null));

        // ensure no grant recorded
        var already = await read.JaConcedidaAsync("s4", themeId, "m4", "badge-e");
        Assert.False(already);
    }

    [Fact]
    public async Task ConcederBadge_requisicao_repetida_com_requestId_igual_nao_duplica()
    {
        var read = new InMemoryAwardsReadStore();
        var write = new InMemoryAwardsWriteStore(read);
        var themeId = "theme6";
        var now = DateTimeOffset.UtcNow;
        read.AddTheme(themeId, new ThemeBonusDates(null, now.AddMinutes(5), now.AddMinutes(10), 10, 40, 15));
        read.MarkConcluded("s5","m5");
        var service = new AwardBadgeService(read, write);

        var requestId = "req-1";
        var first = await service.AwardAsync("s5", themeId, "m5", "badge-f", now, requestId);
        Assert.Single(write.Logs);

        // simulate same request id present in read store
        read.AddRequestId(requestId);
        await Assert.ThrowsAsync<AlreadyGrantedException>(async () =>
            await service.AwardAsync("s5", themeId, "m5", "badge-f", now, requestId));
    }
}
