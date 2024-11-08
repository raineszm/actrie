using System.Collections.Generic;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test;

public class TrieTest : AbstractTrieTest
{
    protected override IDictionary<string, int> Create() { return new Trie<int>(); }

    [Property]
    public void ConsumeLongestPrefix_ForAnEmptyTrie_ReturnsNull(NonEmptyString text)
    {
        var trie = new Trie<int>();
        var result = trie.ConsumeLongestPrefix(text.Get);
        result.Should().BeNull();
    }

    [Property]
    public Property ConsumeLongestPrefix_WhenNoMatch_ReturnsNull(NonEmptyString key, NonEmptyString text)
    {
        var trie = new Trie<int> { [key.Get] = 1 };
        return (trie.ConsumeLongestPrefix(text.Get) is null).When(key.Get != text.Get);
    }

    [Property(Replay = "140881727,296846848")]
    public void ConsumeLongestPrefix_ForFullMatch_ReturnsEmptyString(NonEmptyString text)
    {
        var trie = new Trie<int>
        {
            [text.Get] = 1,
        };
        var result = trie.ConsumeLongestPrefix(text.Get);
        result?.Value.Should().Be(1);
        result?.Remainder.Should().BeEmpty();
    }


    [Property(StartSize = 2)]
    public Property ConsumeLongestPrefix_ForPartialMatch_ReturnsSuffix(int value)
    {
        var keyAndPrefix = from text in Arb.Generate<NonEmptyString>()
            from i in Gen.Choose(1, text.Get.Length - 1)
            select (text.Get, i);

        return Prop.ForAll(
            keyAndPrefix.ToArbitrary(),
            t =>
            {
                var (text, i) = t;
                var trie = new Trie<int>
                {
                    [text.Substring(0, i)] = value,
                };
                var result = trie.ConsumeLongestPrefix(text);
                result?.Value.Should().Be(value);
                result?.Remainder.Should().Be(text[i..]);
            }
        );
    }
}