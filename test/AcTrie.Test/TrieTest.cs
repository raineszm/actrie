using System.Collections.Generic;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test
{
    public class TrieTest : AbstractTrieTest
    {
        protected override IDictionary<string, int> Create()
        {
            return new Trie<int>();
        }

        [Property]
        public bool ConsumeLongestPrefix_ForAnEmptyTrie_ReturnsOriginalString(NonEmptyString text)
        {
            var trie = new Trie<int>();
            return trie.ConsumeLongestPrefix(text.Get) == (null, text.Get);
        }

        [Property]
        public Property ConsumeLongestPrefix_WhenNoMatch_ReturnsOriginalString(NonEmptyString key, NonEmptyString text)
        {
            var trie = new Trie<int> {[key.Get] = 1};
            return (trie.ConsumeLongestPrefix(text.Get) == (null, text.Get)).When(key.Get != text.Get);
        }

        [Property]
        public bool ConsumeLongestPrefix_ForFullMatch_ReturnsEmptyString(NonEmptyString text)
        {
            var trie = new Trie<int>
            {
                [text.Get] = 1
            };
            return trie.ConsumeLongestPrefix(text.Get) == (1, "");
        }
        
        
        [Property(StartSize = 2)]
        public Property ConsumeLongestPrefix_ForPartialMatch_ReturnsSuffix(int value)
        {
            var keyAndPrefix = from text in Arb.Generate<NonEmptyString>()
                from i in Gen.Choose(1, text.Get.Length - 1)
                select (text.Get, i);
            
            return Prop.ForAll(keyAndPrefix.ToArbitrary(), t =>
            {
                var (text, i) = t;
                var trie = new Trie<int>
                {
                    [text.Substring(0, i)] = value
                };
                return trie.ConsumeLongestPrefix(text) == (1, text.Substring(i));
            });
        }

    }
}