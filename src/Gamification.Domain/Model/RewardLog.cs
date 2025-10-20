using System;
namespace Gamification.Domain.Model;
public sealed class RewardLog
{
    public string StudentId { get; }
    public string ThemeId { get; }
    public string MissionId { get; }
    public string BadgeSlug { get; }
    public XpAmount Xp { get; }
    public DateTimeOffset When { get; }
    public string Source { get; }
    public string Reason { get; }

    public RewardLog(string studentId, string themeId, string missionId, string badgeSlug, XpAmount xp, DateTimeOffset when, string source, string reason)
    {
        StudentId = studentId;
        ThemeId = themeId;
        MissionId = missionId;
        BadgeSlug = badgeSlug;
        Xp = xp;
        When = when;
        Source = source;
        Reason = reason;
    }
}
