using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test
{
    public class TrieTest
    {
        [Property(StartSize = 1)]
        public bool Get_GivenASetValue_GetsThatValue(ITrie<char, int> trie, string key, int value)
        {
            trie[key] = value;
            return trie[key] == value;
        }

        [Property(StartSize = 1)]
        public Property Set_GivenMultipleKeys_CanAddThem(ITrie<char, int> trie, int value, int value2)
        {
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
        public void ContainsKey_GivenAKey_ReturnsWhetherKeyIsInTrie(ITrie<char, int> trie, string key, int value)
        {
            trie.ContainsKey(key).Should().BeFalse();
            trie[key] = value;
            trie.ContainsKey(key).Should().BeTrue();
        }

        [Property(StartSize = 1)]
        public bool Remove_GivenAnEmptyTrie_CannotRemoveFromIt(ITrie<char, int> trie, string key)
        {
            return !trie.Remove(key);
        }
        
        [Property(StartSize = 1)]
        public bool Remove_GivenAKeyInTrie_RemovesIt(ITrie<char, int> trie, string key, int value)
        {
            trie[key] = value;
            trie.Remove(key);
            return !trie.ContainsKey(key);
        }
// describe('Trie', () => {
//   describe('iterate', () => {
//     it('iterates keys', () => {
//       fc.assert(
//         fc.property(
//           fc.dictionary(fc.string({ minLength: 1 }), fc.integer()),
//           (keysAndValues) => {
//             const trie = new Trie();
//             for (const [k, v] of Object.entries(keysAndValues)) {
//               trie.set(k, v);
//             }
//             const keys = Object.keys(keysAndValues).sort((a, b) =>
//               a.localeCompare(b),
//             );
//
//             const trieKeys = Array.from(trie)
//               .map(([x]) => x)
//               .sort((a, b) => a.localeCompare(b));
//
//             expect(trieKeys).toStrictEqual(keys);
//           },
//         ),
//       );
//     });
//   });
//
//   describe('consumeLongestPrefix', () => {
//     it('returns original for empty trie', () => {
//       fc.assert(
//         fc.property(fc.string({ minLength: 1 }), (text) => {
//           const trie = new Trie();
//           expect(trie.consumeLongestPrefix(text)).toStrictEqual([
//             undefined,
//             text,
//           ]);
//         }),
//       );
//     });
//     it('returns same text when no match', () => {
//       fc.assert(
//         fc.property(
//           fc.string({ minLength: 1 }),
//           fc.string({ minLength: 1 }),
//           (text, key) => {
//             fc.pre(!text.startsWith(key));
//             const trie = new Trie();
//             trie.set(key, 1);
//             expect(trie.consumeLongestPrefix(text)).toStrictEqual([
//               undefined,
//               text,
//             ]);
//           },
//         ),
//       );
//     });
//
//     it('returns empty string on full match', () => {
//       fc.assert(
//         fc.property(fc.string({ minLength: 1 }), fc.integer(), (key, value) => {
//           const trie = new Trie();
//           trie.set(key, value);
//           expect(trie.consumeLongestPrefix(key)).toStrictEqual([value, '']);
//         }),
//       );
//     });
//
//     it('returns suffix on partial match', () => {
//       fc.assert(
//         fc.property(textAndPrefix, fc.integer(), ([text, i], value) => {
//           const trie = new Trie();
//           trie.set(text.slice(0, i), value);
//           expect(trie.consumeLongestPrefix(text)).toStrictEqual([
//             value,
//             text.slice(i),
//           ]);
//         }),
//       );
//     });
//   });
// });
//
// const textAndPrefix: Arbitrary<[string, number]> = fc
//   .string({ minLength: 2 })
//   .chain((t) => {
//     return fc.integer({ min: 1, max: t.length - 1 }).map((i) => {
//       return [t, i];
//     });
//   });
//
    }
}