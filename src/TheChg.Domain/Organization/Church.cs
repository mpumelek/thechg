namespace TheChg.Domain.Organization;

public sealed class Church
{
    private Church() { } // EF Core

    private Church(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public static Church Create(Guid id, string name)
    {
        if (id == Guid.Empty) throw new ArgumentException("A church ID is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
            throw new ArgumentException("A church name of at most 200 characters is required.", nameof(name));

        return new Church(id, name.Trim());
    }
}
