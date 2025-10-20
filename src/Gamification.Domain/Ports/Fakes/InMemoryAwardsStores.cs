using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gamification.Domain.Model;

namespace Gamification.Domain.Ports.Fakes;
using Gamification.Domain.Ports;
public sealed class InMemoryAwardsReadStore : IAwardsReadStore
{
    private readonly HashSet<string> _concluded = new();
    private readonly HashSet<string> _granted = new();
    private readonly HashSet<string> _requestIds = new();
    private readonly Dictionary<string, ThemeBonusDates> _themes = new();

    public void MarkConcluded(string studentId, string missionId) => _concluded.Add($"{studentId}::{missionId}");
    public void MarkGranted(string studentId, string themeId, string missionId, string badgeSlug) => _granted.Add($"{studentId}::{themeId}::{missionId}::{badgeSlug}");
    public void AddRequestId(string requestId) => _requestIds.Add(requestId);
    public void AddTheme(string themeId, ThemeBonusDates dates) => _themes[themeId] = dates;

    public ValueTask<bool> MissaoConcluidaAsync(string studentId, string missionId) => ValueTask.FromResult(_concluded.Contains($"{studentId}::{missionId}"));
    public ValueTask<bool> JaConcedidaAsync(string studentId, string themeId, string missionId, string badgeSlug) => ValueTask.FromResult(_granted.Contains($"{studentId}::{themeId}::{missionId}::{badgeSlug}"));
    public ValueTask<bool> RequestIdExistsAsync(string requestId) => ValueTask.FromResult(_requestIds.Contains(requestId));
    public ValueTask<ThemeBonusDates?> GetThemeBonusDatesAsync(string themeId) => ValueTask.FromResult(_themes.TryGetValue(themeId, out var v) ? v : null);
}

public sealed class InMemoryAwardsWriteStore : IAwardsWriteStore
{
    private readonly InMemoryAwardsReadStore _read;
    private readonly bool _failOnSave;

    public List<RewardLog> Logs { get; } = new();

    public InMemoryAwardsWriteStore(InMemoryAwardsReadStore read, bool failOnSave = false)
    {
        _read = read;
        _failOnSave = failOnSave;
    }

    public ValueTask SaveConcessaoAtomicAsync(Badge badge, string studentId, string themeId, string missionId, XpAmount xp, RewardLog log, string? requestId = null)
    {
        if (_failOnSave) throw new Exception("Simulated failure during save.");
        // persist logically to read store
        _read.MarkGranted(studentId, themeId, missionId, badge.Slug);
        if (!string.IsNullOrWhiteSpace(requestId)) _read.AddRequestId(requestId);
        Logs.Add(log);
        return ValueTask.CompletedTask;
    }
}
