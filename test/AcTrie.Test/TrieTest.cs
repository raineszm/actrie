using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test
{
    public class TrieTest : ITrieTest
    {
        protected override ITrie<char, int> Create()
        {
            return new Trie<int>();
        }

        [Property(StartSize = 1)]
        public bool ConsumeLongestPrefix_ForAnEmptyTrie_ReturnsOriginalString(string text)
        {
            var trie = new Trie<int>();
            return trie.ConsumeLongestPrefix(text) == (null, text);
        }

        [Property(StartSize = 1)]
        public Property ConsumeLongestPrefix_WhenNoMatch_ReturnsOriginalString(string key, string text)
        {
            var trie = new Trie<int> {[key] = 1};
            return (trie.ConsumeLongestPrefix(text) == (null, text)).When(key != text);
        }

        [Property(StartSize = 1)]
        public bool ConsumeLongestPrefix_ForFullMatch_ReturnsEmptyString(string text)
        {
            var trie = new Trie<int>
            {
                [text] = 1
            };
            return trie.ConsumeLongestPrefix(text) == (1, "");
        }
        
        
        [Property(StartSize = 2)]
        public Property ConsumeLongestPrefix_ForPartialMatch_ReturnsSuffix(int value)
        {
            var keyAndPrefix = from text in Arb.Generate<string>()
                from i in Gen.Choose(1, text.Length - 1)
                select (text, i);
            
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