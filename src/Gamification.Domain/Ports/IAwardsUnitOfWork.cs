namespace Gamification.Domain.Ports;
public interface IAwardsUnitOfWork
{
    ValueTask ExecuteAsync(System.Func<ValueTask> operation);
}
