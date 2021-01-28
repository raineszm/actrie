using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AcTrie
{
    public class Trie<TValue> : IDictionary<string, TValue> where TValue : struct
    {
        private readonly IDictionary<char, Edge> _edges = new Dictionary<char, Edge>();
        public TValue? Value { get; set; }

        public Trie()
        {
        }

        public Trie(TValue value)
        {
            Value = value;
        }

        public bool IsReadOnly => false;

        public bool TryGetValue(string key, out TValue value)
        {
            var node = FindNode(key);
            if (node?.Value is not null)
            {
                value = (TValue) node.Value;
                return true;
            }

            value = default;

            return false;
        }

        public ICollection<string> Keys => this.Select(kv => kv.Key).ToList();

        public ICollection<TValue> Values => this.Select(kv => kv.Value).ToList();

        public TValue this[string key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                {
                    throw new KeyNotFoundException(key);
                }

                return value;
            }
            set
            {
                var (remainder, node) = WalkPath(key).Last();

                // We have found a node with this key
                // Set the value on that node
                if (remainder.Length == 0)
                {
                    node.Value = value;
                    return;
                }

                // There is no further progress to made in this key
                // We need add this key as a child of the current node
                node.AddChild(remainder, new Trie<TValue>(value));
            }
        }

        public bool ContainsKey(string key)
        {
            return FindNode(key) is not null;
        }

        public bool RemoveImpl(string key, TValue? value = null)
        {
            if (key.Length == 0) return false;
            IList<Step> path = WalkPath(key).ToList();

            // Path always returns a non-empty array
            // We walk up the tree backwards from the target node
            var (remainder, target) = path[^1];
            path.RemoveAt(path.Count - 1);

            // Target node not found
            if (remainder.Length > 0) return false;

            // There is no node with this key
            if (target.Value is null) return false;

            // If we need to check value do that here
            // For the ICollection implementation
            if (value is not null && !target.Value.Equals(value)) return false;

            // target is the node to be deleted

            // target is a child of `this` node
            if (path.Count == 0)
            {
                return RemoveChild(key);
            }

            // get for its parent
            var last = path[^1];

            var (edgeFromParent, parent) = last;
            var result = parent.RemoveChild(edgeFromParent);
            path = Trim(path);
            Compress(path);
            return result;
        }

        public bool Remove(string key)
        {
            return RemoveImpl(key);
        }


        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return RemoveImpl(item.Key, item.Value);
        }

        public int Count
        {
            get
            {
                var total = 0;
                if (Value is not null)
                {
                    total++;
                }

                if (_edges.Count <= 0) return total;
                foreach (var (_, target) in _edges.Values)
                {
                    total += target.Count;
                }

                return total;
            }
        }

        public void Clear()
        {
            Value = default;
            _edges.Clear();
        }

        public void Add(string key, TValue value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            var node = FindNode(item.Key);
            return node?.Value is not null && node.Value.Equals(item);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            foreach (var kv in this)
            {
                array[arrayIndex++] = kv;
            }
        }

        public (TValue?, string) ConsumeLongestPrefix(string key)
        {
            TValue? value = null;
            var valueKey = key;
            foreach (var (remainder, target) in WalkPath(key))
            {
                if (target.Value is null) continue;
                value = target.Value;
                valueKey = remainder;
            }

            return (value, valueKey);
        }

        private Step? GetChild(string key)
        {
            // An empty key means _this node_
            if (key.Length == 0) return new Step("", this);

            // We track edges by the first letter of their key
            var c = key[0];

            // No matching edge
            if (!_edges.TryGetValue(c, out var edge))
            {
                return null;
            }

            var i = edge.KeyTail.Length;
            
            // No match possible since the edge is longer than our key
            if (i >= key.Length) return null;

            // First letter matches but the full edge does not
            if (key.Substring(1, i) != edge.KeyTail) return null;

            // Strip off the consumed characters and return remaining key
            // and matched node
            return new Step(key.Substring(i + 1), edge.Target);
        }

        protected void AddChild(string key, Trie<TValue> node)
        {
            var c = key[0];
            var keyTail = key.Substring(1);

            if (!_edges.ContainsKey(c))
            {
                _edges.Add(c, new Edge(keyTail, node));
            }

            // We have a partial match with an existing edge
            // And must split it
            Split(c, keyTail, node);
        }

        protected bool RemoveChild(string key)
        {
            var c = key[0];
            var rest = key.Substring(1);

            if (!_edges.TryGetValue(c, out var edge)) return false;

            var (keyTail, target) = edge;
            if (rest.Substring(0, keyTail.Length) != keyTail) return false;

            _edges.Remove(c);

            // Absorb any orphaned children
            foreach (var (k, (childRest, childNode)) in target._edges)
            {
                var childKey = $"{key}{k}{childRest}";
                AddChild(childKey, childNode);
            }

            return true;
        }

        private Trie<TValue>? FindNode(string key)
        {
            var (remainder, currentNode) = WalkPath(key).Last();
            return remainder.Length > 0 ? null : currentNode;
        }


        public IEnumerable<Trie<TValue>> Leaves()
        {
            if (_edges.Count == 0)
            {
                yield return this;
                yield break;
            }

            foreach (var (_, target) in _edges.Values)
            {
                foreach (var leaf in target.Leaves())
                {
                    yield return leaf;
                }
            }
        }


        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            foreach (var (c, (keyTail, target)) in _edges)
            {
                var head = $"{c}{keyTail}";
                if (target.Value is not null)
                {
                    yield return new KeyValuePair<string, TValue>(head, (TValue) target.Value);
                }

                foreach (var (name, value) in target)
                {
                    yield return new KeyValuePair<string, TValue>($"{head}{name}", value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Split(char c, string rest, Trie<TValue> node)
        {
            if (!_edges.TryGetValue(c, out var edge))
            {
                throw new ArgumentException($"attempting to split undefined edge {c}");
            }

            // Look up the edge we're colliding with
            var (splitKey, currentNode) = edge;

            // Find longest matching prefix between keys
            var newLength = rest.Length;
            var oldLength = splitKey.Length;

            var matchLength = 0;
            for (var i = 0; i < Math.Min(oldLength, newLength); i++)
            {
                if (rest[i] != splitKey[i])
                {
                    break;
                }

                matchLength++;
            }

            // Our new node is exactly the same key as our old, so replace it.
            if (matchLength == newLength && matchLength == oldLength)
            {
                _edges[c] = new Edge(rest, node);
                return;
            }

            // Our new node needs to be inserted between
            // the parent and current child
            if (matchLength == newLength)
            {
                node.AddChild(splitKey.Substring(matchLength), currentNode);
                _edges[c] = new Edge(rest, node);
                return;
            }

            // New node is a child of existing node
            // This is an abuse of the add_child method
            // But we allow it because it's sane
            if (matchLength == oldLength)
            {
                currentNode.AddChild(rest.Substring(matchLength), node);
                return;
            }

            var newNode = new Trie<TValue>();
            newNode.AddChild(splitKey.Substring(matchLength), currentNode);
            newNode.AddChild(rest.Substring(matchLength), node);
            _edges[c] = new Edge(
                splitKey.Substring(0, matchLength),
                newNode
            );
        }

        private static IList<Step> Trim(IList<Step> path)
        {
            if (path.Count <= 0)
            {
                throw new ArgumentException("attempted to trim empty path");
            }

            // Pull out the parent of the removed node
            var (remainder, target) = path[^1];
            path.RemoveAt(path.Count - 1);

            // if path is empty it means that node is the root, i.e. self
            // Since we can't delete self, we're done
            //
            // We only need to delete nodes until
            // we have removed all dangling leaves
            // If a node has edges we are not a leaf and can stop trimming
            //
            // If node.value is not None
            // we have a value we are a valid leaf node
            // and we can stop trimming
            while (
                path.Count > 0 &&
                target._edges.Count == 0 &&
                target.Value is not null
            )
            {
                var (edgeFromParent, parent) = path[^1];
                parent.RemoveChild(
                    edgeFromParent.Substring(edgeFromParent.Length - remainder.Length)
                );
                path.RemoveAt(path.Count - 1);

                remainder = edgeFromParent;
                target = parent;
            }

            path.Add(new Step(remainder, target));

            // Return the remaining ancestors for compression
            return path;
        }

        private static void Compress(IList<Step> path)
        {
            if (path.Count <= 0)
            {
                throw new ArgumentException("attempted to compress empty path");
            }

            var (key, node) = path[^1];
            path.RemoveAt(path.Count - 1);

            var toCompress = new List<Step>();
            foreach (var x in path.Reverse())
            {
                if (x.Target.Value is null && x.Target._edges.Count == 1)
                {
                    toCompress.Add(x);
                }
                else
                {
                    break;
                }
            }

            if (toCompress.Count <= 0)
            {
                return;
            }

            var (edgeFromParent, parent) = toCompress[^1];
            path.RemoveAt(path.Count - 1);
            var keyTail = $"{string.Join("", toCompress.AsEnumerable().Reverse().Select(x => x.Remainder))}{key}";
            var newKey = edgeFromParent.Substring(
                0,
                edgeFromParent.Length - keyTail.Length
            );
            parent.RemoveChild(
                edgeFromParent.Substring(0, edgeFromParent.Length - key.Length)
            );
            parent.AddChild(newKey, node);
        }

        // Helper classes
        private record Edge(string KeyTail, Trie<TValue> Target);

        private record Step(string Remainder, Trie<TValue> Target);

        private IEnumerable<Step> WalkPath(string key)
        {
            var step = new Step(key, this);

            yield return step;

            // If we have consumed the whole key then we have found our node
            while (step.Remainder.Length > 0)
            {
                // Get the next step toward the sought node
                step = step.Target.GetChild(step.Remainder);

                // We have run out of matching nodes
                if (step is null)
                {
                    yield break;
                }

                yield return step;
            }
        }
    }
}