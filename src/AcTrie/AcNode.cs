using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AcTrie;

internal class AcNode<TToken, TValue> : IDictionary<IEnumerable<TToken>, TValue>
    where TToken : notnull
{
    public readonly IDictionary<TToken, AcNode<TToken, TValue>>
        Children = new Dictionary<TToken, AcNode<TToken, TValue>>();

    public SuffixEdge? Suffix;

    public Option<TValue> Value;
    public bool IsLeaf => !Children.Any();
    public bool IsReadOnly => false;

    public bool TryGetValue(IEnumerable<TToken> key, [MaybeNullWhen(false)] out TValue value)
    {
        value = default;
        var keyList = key.ToList();
        if (!keyList.Any()) return false;
        var node = IterPath(keyList.ToList()).Last();
        if (node == this || node is null) return false;
        return node.Value.TryGetValue(out value);
    }

    public TValue this[IEnumerable<TToken> key]
    {
        get
        {
            if (!TryGetValue(key, out var value)) throw new KeyNotFoundException(key.ToString());

            return value;
        }

        set => Add(key, value);
    }

    public void Add(IEnumerable<TToken> key, TValue value)
    {
        var keyList = key.ToList();

        // Key is exhausted
        // We are setting this node
        if (!keyList.Any())
        {
            Value = value;
            return;
        }

        Children.TryGetValue(keyList.First(), out var child);
        // If no matching child, create it
        if (child is null)
        {
            child = new AcNode<TToken, TValue>();
            Children[keyList.First()] = child;
        }

        child[keyList.Skip(1)] = value;
    }

    public void Add(KeyValuePair<IEnumerable<TToken>, TValue> item) { Add(item.Key, item.Value); }

    public bool Remove(IEnumerable<TToken> key) { return RemoveImpl(key); }

    public bool Remove(KeyValuePair<IEnumerable<TToken>, TValue> item) { return RemoveImpl(item.Key, item.Value); }


    public bool ContainsKey(IEnumerable<TToken> key) { return TryGetValue(key, out var _); }

    public bool Contains(KeyValuePair<IEnumerable<TToken>, TValue> item)
    {
        return TryGetValue(item.Key, out var value)
               && item.Value is not null
               && item.Value.Equals(value);
    }


    public int Count
    {
        get
        {
            var total = 0;
            if (Value.IsSome) total++;

            return total + Children.Values.Sum(child => child.Count);
        }
    }


    public void Clear()
    {
        Value = new Option<TValue>();
        Children.Clear();
    }

    public IEnumerator<KeyValuePair<IEnumerable<TToken>, TValue>> GetEnumerator()
    {
        if (Value.IsSome)
            yield return new KeyValuePair<IEnumerable<TToken>, TValue>(Array.Empty<TToken>(), Value.Value);

        foreach (var (c, child) in Children)
        foreach (var (k, v) in child)
        {
            yield return new KeyValuePair<IEnumerable<TToken>, TValue>(
                new[] { c }.Concat(k),
                v
            );
        }
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    public void CopyTo(KeyValuePair<IEnumerable<TToken>, TValue>[] array, int arrayIndex)
    {
        var i = arrayIndex;
        foreach (var item in this) array[i++] = item;
    }

    public ICollection<IEnumerable<TToken>> Keys => this.Select(kv => kv.Key).ToList();
    public ICollection<TValue> Values => this.Select(kv => kv.Value).ToList();

    private bool RemoveImpl(IEnumerable<TToken> key, Option<TValue> value = default)
    {
        var keyList = key.ToList();

        if (!keyList.Any()) return false;

        var nodes = IterPath(keyList).ToList();
        if (nodes.Count < keyList.Count + 1) return false;

        var target = nodes[^1];
        nodes.RemoveAt(nodes.Count - 1);
        if (target?.Value is null) return false;


        if (value.IsSome && !value.Equals(target.Value)) return false;

        TrimNodes(keyList, nodes);

        return true;
    }

    private static void TrimNodes(IReadOnlyList<TToken> keyList, IReadOnlyList<AcNode<TToken, TValue>?> nodes)
    {
        for (var i = Math.Min(keyList.Count, nodes.Count) - 1; i >= 0; i--)
        {
            var parent = nodes[i];
            if (parent is null) throw new NullReferenceException("accessed a child of an undefined node");

            var edge = keyList[i];
            parent.Children.Remove(edge);
            if (parent.Children.Count > 0 || parent.Value.IsSome) break;
        }
    }

    public IEnumerable<AcNode<TToken, TValue>> Nodes()
    {
        yield return this;
        foreach (var child in Children.Values)
        foreach (var node in child.Nodes())
            yield return node;
    }


    private IEnumerable<AcNode<TToken, TValue>?> IterPath(
        IReadOnlyList<TToken> key
    )
    {
        yield return this;
        if (!key.Any()) yield break;

        if (!Children.TryGetValue(key[0], out var child)) yield break;

        foreach (var node in child.IterPath(key.Skip(1).ToList()))
            yield return node;
    }

    public record SuffixEdge
    {
        public required AcNode<TToken, TValue> Node;
        public int Shift;
    }
}