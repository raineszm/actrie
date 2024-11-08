using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using CITrie = AcTrie.AhoCorasick<char, int>;
using CINeedle = AcTrie.AhoCorasick<char, int>.Needle;

namespace AcTrie.Test;

public class AhoCorasickTest
{
    public readonly AhoCorasick<char, int> SoarAhoCorasick;

    public AhoCorasickTest()
    {
        SoarAhoCorasick = new CITrie { { "at", 0 }, { "art", 1 }, { "oars", 2 }, { "soar", 3 } };
    }

    public static IEnumerable<object[]> SearchData()
    {
        yield return new object[] { "ar", Array.Empty<CINeedle>() };
        yield return new object[]
        {
            "art", new[]
            {
                new CINeedle
                {
                    Start = 0,
                    End = 3,
                    Value = 1,
                },
            },
        };

        yield return new object[] { "artsy", new[] { new CINeedle { Start = 0, End = 3, Value = 1 } } };
        yield return new object[]
        {
            "soars",
            new[]
            {
                new CINeedle { Start = 0, End = 4, Value = 3 },
                new CINeedle { Start = 1, End = 5, Value = 2 },
            },
        };
        yield return new object[]
        {
            "artsoar",
            new[]
            {
                new CINeedle { Start = 0, End = 3, Value = 1 },
                new CINeedle { Start = 3, End = 7, Value = 3 },
            },
        };
    }

    [Theory]
    [MemberData(nameof(SearchData))]
    public void Search_GivenAString_ReturnsAllMatchingSubstrings(string str, CINeedle[] matches)
    {
        SoarAhoCorasick.Search(str).Should().BeEquivalentTo(matches);
    }

    public static IEnumerable<object[]> LongestMatchData()
    {
        yield return new object[] { "ar", null };
        yield return new object[] { "art", new CINeedle { Start = 0, End = 3, Value = 1 } };
        yield return new object[] { "artsy", new CINeedle { Start = 0, End = 3, Value = 1 } };
        yield return new object[] { "soars", new CINeedle { Start = 0, End = 4, Value = 3 } };
        yield return new object[] { "artsoar", new CINeedle { Start = 3, End = 7, Value = 3 } };
    }

    [Theory]
    [MemberData(nameof(LongestMatchData))]
    public void LongestMatch_GivenString_ReturnsLongestMatch(string str, CINeedle match)
    {
        SoarAhoCorasick.LongestMatch(str).Should().Be(match);
    }

    [Fact]
    public void LongestMatch_GivenConflictingPrefixMatchs_ReturnsLongest()
    {
        var trie = new AhoCorasick<char, int> { { "point", 1 }, { "conduct", 2 } };
        trie.LongestMatch("pconduct")
            .Should()
            .Be(
                new CINeedle
                {
                    Start = 1,
                    End = 8,
                    Value = 2,
                }
            );
    }

    [Fact]
    public void ContainsKey_GivenExistingKey_ReturnsTrue()
    {
        var trie = new AhoCorasick<char, int> { { "at", 0 } };
        trie.ContainsKey("at").Should().BeTrue();
    }


    [Fact]
    public void ContainsKey_GivenNonexistentKey_ReturnsFalse()
    {
        var trie = new AhoCorasick<char, int> { { "at", 0 } };
        trie.ContainsKey("ab").Should().BeFalse();
    }

    [Fact]
    public void TryAdd_GivenNewKey_ReturnsTrue()
    {
        var trie = new AhoCorasick<char, int>();
        trie.TryAdd("at", 0).Should().BeTrue();
    }

    [Fact]
    public void TryAdd_GivenExistingKey_ReturnsFalse()
    {
        var trie = new AhoCorasick<char, int> { { "at", 0 } };
        trie.TryAdd("at", 1).Should().BeFalse();
    }
}