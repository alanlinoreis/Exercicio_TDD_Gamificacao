using System;
using System.Threading.Tasks;
using Xunit;
using Gamification.Domain.Awards;
using Gamification.Domain.Ports.Fakes;
using Gamification.Domain.Ports;
using Gamification.Domain.Model;
using Gamification.Domain.Exceptions;

namespace Gamification.Domain.Tests
{
    public class AwardBadgeServiceExtraTests
    {
        [Fact]
        public async Task AwardAsync_requestId_null_allows_retries_but_prevents_duplicates_only_when_saved()
        {
            var read = new InMemoryAwardsReadStore();
            var write = new InMemoryAwardsWriteStore(read);
            var themeId = "t-ex-1";
            var now = DateTimeOffset.UtcNow;
            read.AddTheme(themeId, new ThemeBonusDates(null, now.AddMinutes(5), now.AddMinutes(10), 10, 25, 10));
            read.MarkConcluded("stu1","miss1");

            var service = new AwardBadgeService(read, write);

            var first = await service.AwardAsync("stu1", themeId, "miss1", "badge-x", now, null);
            Assert.Equal("badge-x", first.badge.Slug);
            Assert.Single(write.Logs);

            await Assert.ThrowsAsync<AlreadyGrantedException>(async () =>
                await service.AwardAsync("stu1", themeId, "miss1", "badge-x", now, null));
        }

        [Fact]
        public async Task AwardAsync_multiple_badges_for_same_mission_are_recorded_separately()
        {
            var read = new InMemoryAwardsReadStore();
            var write = new InMemoryAwardsWriteStore(read);
            var themeId = "t-ex-2";
            var now = DateTimeOffset.UtcNow;
            read.AddTheme(themeId, new ThemeBonusDates(null, now.AddMinutes(5), now.AddMinutes(10), 10, 30, 15));
            read.MarkConcluded("stu2","miss2");

            var service = new AwardBadgeService(read, write);

            var res1 = await service.AwardAsync("stu2", themeId, "miss2", "badge-a", now, "r1");
            var res2 = await service.AwardAsync("stu2", themeId, "miss2", "badge-b", now, "r2");

            Assert.Equal("badge-a", res1.badge.Slug);
            Assert.Equal("badge-b", res2.badge.Slug);
            Assert.Equal(2, write.Logs.Count);
        }

        [Fact]
        public async Task AwardAsync_grant_when_dates_null_uses_no_bonus_but_grants_badge()
        {
            var read = new InMemoryAwardsReadStore();
            var write = new InMemoryAwardsWriteStore(read);
            var themeId = "t-ex-3";
            read.AddTheme(themeId, new ThemeBonusDates(null, null, null, 10, 50, 25));
            read.MarkConcluded("stu3","miss3");

            var service = new AwardBadgeService(read, write);
            var now = DateTimeOffset.UtcNow;

            var (badge, xp, log) = await service.AwardAsync("stu3", themeId, "miss3", "badge-nobonus", now, "r3");

            Assert.Equal("badge-nobonus", badge.Slug);
            Assert.Equal(0, xp.Value);
            Assert.Equal("no_bonus", log.Reason);
            Assert.Single(write.Logs);
        }

        [Fact]
        public async Task AwardAsync_when_write_fails_no_jaconcedida_and_no_partial_log()
        {
            var read = new InMemoryAwardsReadStore();
            var write = new InMemoryAwardsWriteStore(read, failOnSave: true);
            var themeId = "t-ex-4";
            var now = DateTimeOffset.UtcNow;
            read.AddTheme(themeId, new ThemeBonusDates(null, now.AddMinutes(5), now.AddMinutes(10), 10, 40, 20));
            read.MarkConcluded("stu4","miss4");

            var service = new AwardBadgeService(read, write);

            await Assert.ThrowsAsync<DomainException>(async () =>
                await service.AwardAsync("stu4", themeId, "miss4", "badge-e", now, "r4"));

            var already = await read.JaConcedidaAsync("stu4", themeId, "miss4", "badge-e");
            Assert.False(already);
            Assert.Empty(write.Logs);
        }

        [Fact]
        public async Task AwardAsync_when_student_and_mission_have_special_chars_handles_properly()
        {
            var read = new InMemoryAwardsReadStore();
            var write = new InMemoryAwardsWriteStore(read);
            var themeId = "t-ex-5";
            var now = DateTimeOffset.UtcNow;
            read.AddTheme(themeId, new ThemeBonusDates(null, now.AddMinutes(5), now.AddMinutes(10), 10, 15, 5));
            read.MarkConcluded("stu-çãõ","miss-ñ");

            var service = new AwardBadgeService(read, write);

            var (badge, xp, log) = await service.AwardAsync("stu-çãõ", themeId, "miss-ñ", "badge-special", now, "r-special");

            Assert.Equal("badge-special", badge.Slug);
            Assert.Single(write.Logs);
        }
    }
}
