namespace Gamification.Domain.Model;
public readonly struct XpAmount
{
    public int Value { get; }
    public XpAmount(int value) => Value = value;
    public static XpAmount Zero => new XpAmount(0);
    public override string ToString() => Value.ToString();
}
