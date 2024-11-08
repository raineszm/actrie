using System.Diagnostics.CodeAnalysis;

namespace AcTrie;

public readonly record struct Option<T>
{
    public Option() { }

    public Option(T value)
    {
        Value = value;
        IsSome = true;
    }

    public T Value { get; } = default!;
    public bool IsSome { get; }
    public bool IsNone => !IsSome;

    public static implicit operator Option<T>(T value) { return new Option<T>(value); }

    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (IsSome)
        {
            value = Value;
            return true;
        }

        value = default;
        return false;
    }
}