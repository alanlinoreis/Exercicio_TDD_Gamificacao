namespace Gamification.Domain.Model;
public sealed class Badge
{
    public string Slug { get; }
    public string Title { get; }
    public Badge(string slug, string title)
    {
        Slug = slug ?? throw new System.ArgumentNullException(nameof(slug));
        Title = title ?? throw new System.ArgumentNullException(nameof(title));
    }
}
