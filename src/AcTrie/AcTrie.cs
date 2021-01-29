using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AcTrie
{
    public class AcTrie<TToken, TValue> : IDictionary<IEnumerable<TToken>, TValue> where TValue : struct
        where TToken : notnull
    {
        public bool Modified = true;
        public AcNode<TToken, TValue> Root = new();

        IEnumerator<KeyValuePair<IEnumerable<TToken>, TValue>> IEnumerable<KeyValuePair<IEnumerable<TToken>, TValue>>.
            GetEnumerator()
        {
            return Root.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) Root).GetEnumerator();
        }

        public void Add(KeyValuePair<IEnumerable<TToken>, TValue> item)
        {
            Root.Add(item);
            Modified = true;
        }

        public void Clear()
        {
            Root.Clear();
            Modified = true;
        }

        public bool Contains(KeyValuePair<IEnumerable<TToken>, TValue> item)
        {
            return Root.Contains(item);
        }

        public void CopyTo(KeyValuePair<IEnumerable<TToken>, TValue>[] array, int arrayIndex)
        {
            Root.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<IEnumerable<TToken>, TValue> item)
        {
            return Modified = Root.Remove(item);
        }

        public int Count => Root.Count;

        public bool IsReadOnly => Root.IsReadOnly;

        public void Add(IEnumerable<TToken> key, TValue value)
        {
            Root.Add(key, value);
            Modified = true;
        }

        public bool ContainsKey(IEnumerable<TToken> key)
        {
            return Root.ContainsKey(key);
        }

        public bool Remove(IEnumerable<TToken> key)
        {
            return Modified = Root.Remove(key);
        }

        public bool TryGetValue(IEnumerable<TToken> key, out TValue value)
        {
            return Root.TryGetValue(key, out value);
        }

        public TValue this[IEnumerable<TToken> key]
        {
            get => Root[key];
            set
            {
                Root[key] = value;
                Modified = true;
            }
        }

        public ICollection<IEnumerable<TToken>> Keys => Root.Keys;

        public ICollection<TValue> Values => Root.Values;


        public IEnumerable<AcNode<TToken, TValue>> Leaves()
        {
            return Root.Nodes().Where(node => node.IsLeaf);
        }

        public IEnumerable<Needle> Search(IEnumerable<TToken> haystack)
        {
            Lock();

            // The slice of the haystack we are looking at currently
            // It's convenient to work with inclusive indices
            // internally, but we return indices in the same format
            // as a slice i.e. [start, end)
            var first = 0;
            var last = 0;
            var node = Root;


            var haystackList = haystack.ToList();

            // Each time through this loop
            // we start at the root of the trie
            // with a single character
            // and expand the matching string by moving
            // down the trie and increasing last
            while (last < haystackList.Count)
            {
                // Fetch next unread key element
                var k = haystackList[last];

                if (!Root.Children.ContainsKey(k))
                {
                    last++;
                    first = last;
                    continue;
                }

                // Move down the tree until we can no longer
                while (node.Children.ContainsKey(k))
                {
                    // while the next character matches
                    node = node.Children[k]; // fetch the matched node

                    if (node.Value is not null)
                        // if that node has a value we have a new matching substring
                        // Note the indices here are of the form [start, end)
                        yield return new Needle
                        {
                            // Starting index, end_index, value
                            Start = first,
                            End = last + 1,
                            Value = (TValue) node.Value
                        };

                    // If we've reached the end of the haystack
                    // we can't go any further down the tree
                    // so stop trying
                    if (last + 1 >= haystackList.Count) yield break;

                    // Otherwise, advance the right edge
                    last++;
                    // And fetch next unread character
                    k = haystackList[last];
                }

                // We've now gone as far down the trie as we can
                // And we must traverse back up along the suffix links
                // moving start up as appropriate until we can start
                // going down the tree again
                while (node.Suffix is not null && !node.Children.ContainsKey(k))
                {
                    // Move up the suffix link
                    var shift = node.Suffix.Shift;
                    node = node.Suffix.Node;

                    // Trim off the appropriate number of entries
                    // at the beginning of the haystack
                    first += shift;

                    // If this node is a match, yield it
                    if (node.Value is not null)
                        // Note inclusive form [start, end)
                        yield return new Needle
                        {
                            Start = first,
                            End = last + 1,
                            Value = (TValue) node.Value
                        }; // Starting index, end_index, node
                }

                if (node.Suffix is null)
                    // Move to the next entry and set the width of the match to 0
                    first = last;
            }
        }

        public Needle? LongestMatch(IEnumerable<TToken> haystack)
        {
            Needle? match = null;
            var length = 0;
            foreach (var needle in Search(haystack))
            {
                var needleLength = needle.End - needle.Start;
                if (needleLength <= length) continue;

                length = needleLength;
                match = needle;
            }

            return match;
        }

        public void Lock()
        {
            if (!Modified) return;
            ComputeSuffixes();
            Modified = false;
        }

        private void ComputeSuffixes()
        {
            var nodeQueue = new Queue<AcNode<TToken, TValue>>(new[] {Root});
            while (nodeQueue.Any())
            {
                var parent = nodeQueue.Dequeue();
                foreach (var (k, child) in parent.Children)
                {
                    nodeQueue.Enqueue(child);
                    if (parent == Root)
                    {
                        child.Suffix = new AcNode<TToken, TValue>.SuffixEdge {Node = Root, Shift = 1};
                        continue;
                    }

                    var target = parent.Suffix!.Node;
                    var shift = parent.Suffix!.Shift;

                    while (!target.Children.ContainsKey(k) && target.Suffix is not null)
                    {
                        var shiftAdd = target.Suffix.Shift;
                        target = target.Suffix.Node;
                        shift += shiftAdd;
                    }

                    child.Suffix = new AcNode<TToken, TValue>.SuffixEdge
                    {
                        Node = target.Children[k] ?? Root,
                        Shift = shift
                    };
                }
            }
        }

        public record Needle
        {
            public int End;
            public int Start;
            public TValue Value;
        }
    }
}