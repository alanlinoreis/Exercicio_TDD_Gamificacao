using System.Threading.Tasks;
using Gamification.Domain.Model;

namespace Gamification.Domain.Ports;
public interface IAwardsWriteStore
{
    /// <summary>
    /// Persists badge + optional xp + audit log atomically. Should throw on failure.
    /// </summary>
    ValueTask SaveConcessaoAtomicAsync(Badge badge, string studentId, string themeId, string missionId, XpAmount xp, RewardLog log, string? requestId = null);
}
