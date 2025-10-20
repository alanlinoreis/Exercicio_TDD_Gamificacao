using System;
using Xunit;
using Gamification.Domain.Awards.Policies;
using Gamification.Domain.Model;

namespace Gamification.Domain.Tests;
public class BonusPolicyTests
{
    [Fact]
    public void BonusPolicy_full_then_reduced_then_none()
    {
        var now = DateTimeOffset.UtcNow;
        var fullEnd = now.AddMinutes(10);
        var final = now.AddMinutes(20);

        var (xp1, r1) = BonusPolicy.Calculate(now, now.AddMinutes(-5), fullEnd, final, 10, 100, 50);
        Assert.Equal(100, xp1.Value);
        Assert.Equal("bonus_full", r1);

        var (xp2, r2) = BonusPolicy.Calculate(now.AddMinutes(15), now.AddMinutes(-5), fullEnd, final, 10, 100, 50);
        Assert.Equal(50, xp2.Value);
        Assert.Equal("bonus_reduced", r2);

        var (xp3, r3) = BonusPolicy.Calculate(now.AddMinutes(30), now.AddMinutes(-5), fullEnd, final, 10, 100, 50);
        Assert.Equal(0, xp3.Value);
        Assert.Equal("no_bonus", r3);
    }

    [Fact]
    public void BonusPolicy_invalid_dates_throws()
    {
        var now = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() =>
            BonusPolicy.Calculate(now, now.AddDays(2), now.AddDays(1), now.AddDays(1), 10, 100, 50));
    }
}
