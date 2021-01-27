using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test
{
    public abstract class ITrieTest
    {
        protected abstract ITrie<char, int> Create();
        [Property]
        public bool Get_GivenASetValue_GetsThatValue(NonEmptyString key, int value)
        {
            var trie = Create();
            trie[key.Get] = value;
            return trie[key.Get] == value;
        }

        [Property]
        public Property Set_GivenMultipleKeys_CanAddThem(int value, int value2)
        {
            var trie = Create();
            return Prop.ForAll<NonEmptyString, NonEmptyString>((key, key2) =>
            {
                trie[key.Get] = value;
                trie[key2.Get] = value2;
                return
                    (trie[key.Get] == value && trie[key2.Get] == value2)
                    .When(key.Get != key2.Get);
            });
        }

        [Property]
        public void ContainsKey_GivenAKey_ReturnsWhetherKeyIsInTrie(NonEmptyString key, int value)
        {
            var trie = Create();
            trie.ContainsKey(key.Get).Should().BeFalse();
            trie[key.Get] = value;
            trie.ContainsKey(key.Get).Should().BeTrue();
        }

        [Property]
        public bool Remove_GivenAnEmptyTrie_CannotRemoveFromIt(NonEmptyString key)
        {
            var trie = Create();
            return !trie.Remove(key.Get);
        }
        
        [Property]
        public bool Remove_GivenAKeyInTrie_RemovesIt(NonEmptyString key, int value)
        {
            var trie = Create();
            trie[key.Get] = value;
            trie.Remove(key.Get);
            return !trie.ContainsKey(key.Get);
        }
    }
}