using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;

namespace AcTrie.Test;

public class TrieModelTest : ICommandGenerator<Trie<int>, IDictionary<string, int>>
{
    public Gen<Command<Trie<int>, IDictionary<string, int>>> Next(IDictionary<string, int> model)
    {
        return Arb.Generate<NonEmptyString>()
            .Zip(
                Arb.Generate<int>(),
                (key, value) =>
                {
                    return new Command<Trie<int>, IDictionary<string, int>>[]
                    {
                        new SizeCommand(),
                        new LeavesCommand(),
                        new GetCommand(key.Get),
                        new SetCommand(key.Get, value),
                        new DeleteCommand(key.Get),
                    };
                }
            )
            .SelectMany(Gen.Elements);
    }

    public Trie<int> InitialActual => new();
    public IDictionary<string, int> InitialModel => new Dictionary<string, int>();

    [Property]
    public Property Run() { return new TrieModelTest().ToProperty(); }

    public class SetCommand : Command<Trie<int>, IDictionary<string, int>>
    {
        public readonly string Key;
        public readonly int Value;

        public SetCommand(string key, int value)
        {
            Key = key;
            Value = value;
        }


        public override Trie<int> RunActual(Trie<int> trie)
        {
            trie[Key] = Value;
            return trie;
        }

        public override IDictionary<string, int> RunModel(IDictionary<string, int> dict)
        {
            dict[Key] = Value;
            return dict;
        }

        public override string ToString() { return $"Set('{Key}', {Value})"; }
    }

    public class DeleteCommand : Command<Trie<int>, IDictionary<string, int>>
    {
        public readonly string Key;
        private bool _dictRemoved;
        private bool _trieRemoved;

        public DeleteCommand(string key) { Key = key; }

        public override bool Pre(IDictionary<string, int> dict) { return dict.Count > 0; }

        public override Trie<int> RunActual(Trie<int> trie)
        {
            _trieRemoved = trie.Remove(Key);
            return trie;
        }

        public override IDictionary<string, int> RunModel(IDictionary<string, int> dict)
        {
            _dictRemoved = dict.Remove(Key);
            return dict;
        }

        public override Property Post(Trie<int> trie, IDictionary<string, int> dict)
        {
            return (_trieRemoved == _dictRemoved).ToProperty();
        }

        public override string ToString() { return $"Delete('{Key}')"; }
    }

    public class GetCommand : Command<Trie<int>, IDictionary<string, int>>
    {
        public readonly string Key;
        private int _dictGot;
        private bool _dictSuccess;
        private int _trieGot;
        private bool _trieSuccess;

        public GetCommand(string key) { Key = key; }

        public override bool Pre(IDictionary<string, int> dict) { return dict.Count > 0 && base.Pre(dict); }

        public override Trie<int> RunActual(Trie<int> trie)
        {
            _trieSuccess = trie.TryGetValue(Key, out _trieGot);
            return trie;
        }

        public override IDictionary<string, int> RunModel(IDictionary<string, int> dict)
        {
            _dictSuccess = dict.TryGetValue(Key, out _dictGot);
            return dict;
        }

        public override Property Post(Trie<int> trie, IDictionary<string, int> dict)
        {
            return (_trieSuccess == _dictSuccess && _trieGot == _dictGot).ToProperty();
        }

        public override string ToString() { return $"Get('{Key}')"; }
    }

    public class SizeCommand : Command<Trie<int>, IDictionary<string, int>>
    {
        public override Trie<int> RunActual(Trie<int> trie) { return trie; }

        public override IDictionary<string, int> RunModel(IDictionary<string, int> dict) { return dict; }

        public override Property Post(Trie<int> trie, IDictionary<string, int> model)
        {
            return (trie.Count == model.Count).ToProperty();
        }

        public override string ToString() { return "Size()"; }
    }

    public class LeavesCommand : Command<Trie<int>, IDictionary<string, int>>
    {
        public override bool Pre(IDictionary<string, int> dict) { return dict.Count > 0; }

        public override Trie<int> RunActual(Trie<int> trie) { return trie; }


        public override IDictionary<string, int> RunModel(IDictionary<string, int> dict) { return dict; }


        public override Property Post(Trie<int> trie, IDictionary<string, int> _)
        {
            return trie.Leaves().All(x => x.Value.IsSome || x != trie).ToProperty();
        }

        public override string ToString() { return "Leaves()"; }
    }
}