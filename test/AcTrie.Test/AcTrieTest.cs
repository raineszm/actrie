using System;
using System.Collections.Generic;
using Xunit;
using CITrie = AcTrie.AcTrie<char, int>;

namespace AcTrie.Test
{
    public class AcTrieTest
    {
        public readonly AcTrie<char, int> SoarAcTrie;

        public AcTrieTest()
        {
            SoarAcTrie = new CITrie();
            SoarAcTrie.Add("at", 0);
            SoarAcTrie.Add("art", 1);
            SoarAcTrie.Add("oars", 2);
            SoarAcTrie.Add("soar", 3);
        }

        public static IEnumerable<object[]> SearchData()
        {
            yield return new object[] {"ar", Array.Empty<CITrie.Needle>()};
            yield return new object[]
            {
                "art", new[]
                {
                    new CITrie.Needle
                    {
                        Start = 0,
                        End = 3,
                        Value = 1
                    }
                }
            };

            yield return new object[] {"artsy", new[] {new CITrie.Needle {Start = 0, End = 3, Value = 1}},};
            yield return new object[]
            {
                "soars",
                new[]
                {
                    new CITrie.Needle {Start = 0, End = 4, Value = 3},
                    new CITrie.Needle {Start = 1, End = 5, Value = 2},
                }
            };
            yield return new object[]
            {
                "artsoar",
                new[]
                {
                    new CITrie.Needle {Start = 0, End = 3, Value = 1},
                    new CITrie.Needle {Start = 3, End = 7, Value = 3},
                }
            };
        }

        [Theory]
        [MemberData(nameof(SearchData))]
        public void Search_GivenAString_ReturnsAllMatchingSubstrings(string str, AcTrie<char, int>.Needle[] matches)
        {
            SoarAcTrie.Search(str).Should().BeEquivalentTo(matches);
        }

        public static IEnumerable<object[]> LongestMatchData()
        {
            yield return new object[] {"ar", null};
            yield return new object[] {"art", new CITrie.Needle {Start = 0, End = 3, Value = 1}};
            yield return new object[] {"artsy", new CITrie.Needle {Start = 0, End = 3, Value = 1}};
            yield return new object[] {"soars", new CITrie.Needle {Start = 0, End = 4, Value = 3}};
            yield return new object[] {"artsoar", new CITrie.Needle {Start = 3, End = 7, Value = 3}};
        }

        [Theory]
        [MemberData(nameof(LongestMatchData))]
        public void LongestMatch_GivenString_ReturnsLongestMatch(string str, CITrie.Needle match)
        {
            SoarAcTrie.LongestMatch(str).Should().Be(match);
        }

        [Fact]
        public void LongestMatch_GivenConflictingPrefixMatchs_ReturnsLongest()
        {
            var trie = new AcTrie<char, int>();
            trie.Add("point", 1);
            trie.Add("conduct", 2);
            trie.LongestMatch("pconduct").Should().Be(new AcTrie<char, int>.Needle
            {
                Start = 1,
                End = 8,
                Value = 2,
            });
        }
    }
}