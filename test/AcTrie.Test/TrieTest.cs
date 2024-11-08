using System.Collections.Generic;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test;

public class TrieTest : AbstractTrieTest
{
    protected override IDictionary<string, int> Create() { return new Trie<int>(); }

    [Property]
    public void ConsumeLongestPrefix_ForAnEmptyTrie_ReturnsOriginalString(NonEmptyString text)
    {
        var trie = new Trie<int>();
        var (value, remainder) = trie.ConsumeLongestPrefix(text.Get);
        value.IsNone.Should().BeTrue();
        remainder.Should().Be(text.Get);
    }

    [Property]
    public Property ConsumeLongestPrefix_WhenNoMatch_ReturnsOriginalString(NonEmptyString key, NonEmptyString text)
    {
        var trie = new Trie<int> { [key.Get] = 1 };
        return (trie.ConsumeLongestPrefix(text.Get) == (default, text.Get)).When(key.Get != text.Get);
    }

    [Property(Replay = "140881727,296846848")]
    public void ConsumeLongestPrefix_ForFullMatch_ReturnsEmptyString(NonEmptyString text)
    {
        var trie = new Trie<int>
        {
            [text.Get] = 1,
        };
        var (value, remainder) = trie.ConsumeLongestPrefix(text.Get);
        value.Should().Be(new Option<int>(1));
        remainder.Should().BeEmpty();
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
                var (result, remainder) = trie.ConsumeLongestPrefix(text);
                result.Should().Be(new Option<int>(value));
                remainder.Should().Be(text[i..]);
            }
        );
    }
}