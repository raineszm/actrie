using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test
{
    public abstract class ITrieTest
    {
        protected abstract ITrie<char, int> Create();
        [Property(StartSize = 1)]
        public bool Get_GivenASetValue_GetsThatValue(string key, int value)
        {
            var trie = Create();
            trie[key] = value;
            return trie[key] == value;
        }

        [Property(StartSize = 1)]
        public Property Set_GivenMultipleKeys_CanAddThem(int value, int value2)
        {
            var trie = Create();
            return Prop.ForAll<string, string>((key, key2) =>
            {
                trie[key] = value;
                trie[key2] = value2;
                return
                    (trie[key] == value && trie[key2] == value2)
                    .When(key != key2);
            });
        }

        [Property(StartSize = 1)]
        public void ContainsKey_GivenAKey_ReturnsWhetherKeyIsInTrie(string key, int value)
        {
            var trie = Create();
            trie.ContainsKey(key).Should().BeFalse();
            trie[key] = value;
            trie.ContainsKey(key).Should().BeTrue();
        }

        [Property(StartSize = 1)]
        public bool Remove_GivenAnEmptyTrie_CannotRemoveFromIt(string key)
        {
            var trie = Create();
            return !trie.Remove(key);
        }
        
        [Property(StartSize = 1)]
        public bool Remove_GivenAKeyInTrie_RemovesIt(string key, int value)
        {
            var trie = Create();
            trie[key] = value;
            trie.Remove(key);
            return !trie.ContainsKey(key);
        }
    }
}