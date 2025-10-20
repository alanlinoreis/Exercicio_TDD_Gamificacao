using System;
using Xunit;
using Gamification.Domain.Awards.Policies;
using Gamification.Domain.Model;

namespace Gamification.Domain.Tests
{
    public class BonusPolicyEdgeTests
    {
        [Fact]
        public void Calculate_exactly_at_full_end_returns_full_bonus()
        {
            var now = DateTimeOffset.UtcNow;
            var fullEnd = now;
            var final = now.AddMinutes(10);

            var (xp, reason) = BonusPolicy.Calculate(fullEnd, now.AddMinutes(-10), fullEnd, final, 10, 200, 100);
            Assert.Equal(200, xp.Value);
            Assert.Equal("bonus_full", reason);
        }

        [Fact]
        public void Calculate_at_final_date_still_counts_as_reduced_bonus()
        {
            var now = DateTimeOffset.UtcNow;
            var fullEnd = now.AddMinutes(-10);
            var final = now;

            var (xp, reason) = BonusPolicy.Calculate(final, now.AddMinutes(-30), fullEnd, final, 10, 200, 100);
            Assert.Equal(100, xp.Value);
            Assert.Equal("bonus_reduced", reason);
        }

        [Fact]
        public void Calculate_with_zero_weights_does_not_throw_and_returns_valid_value()
        {
            var now = DateTimeOffset.UtcNow;
            var fullEnd = now.AddMinutes(10);
            var final = now.AddMinutes(20);

            var (xp, reason) = BonusPolicy.Calculate(now, now.AddMinutes(-5), fullEnd, final, 0, 100, 50);
            Assert.True(xp.Value >= 0);
            Assert.NotNull(reason);
        }

        [Fact]
        public void Calculate_negative_weight_does_not_throw_and_returns_valid_value()
        {
            var now = DateTimeOffset.UtcNow;
            var fullEnd = now.AddMinutes(10);
            var final = now.AddMinutes(20);

            var (xp, reason) = BonusPolicy.Calculate(now, now.AddMinutes(-5), fullEnd, final, -1, 100, 50);
            Assert.True(xp.Value >= 0);
            Assert.NotNull(reason);
        }

        [Fact]
        public void Calculate_when_fullEnd_before_start_does_not_throw()
        {
            var now = DateTimeOffset.UtcNow;
            var start = now.AddMinutes(5);
            var fullEnd = now; // fullEnd before start
            var final = now.AddMinutes(20);

            var (xp, reason) = BonusPolicy.Calculate(now, start, fullEnd, final, 10, 100, 50);
            Assert.True(xp.Value >= 0);
            Assert.NotNull(reason);
        }
    }
}
