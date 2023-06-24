# AcTrie

This library provides a naive implementation of as the Aho-Corasick algorithm for string searching
as well as a simple implementation of a compressed radix trie.

## Usage

### Trie

The `Trie` module defines a compressed radix tree mapping string keys to a generic value. This class implements the `IDictionary<string, TValue>` interface. Additionally it implements the `ConsumeLongestPrefix` method, which returns the value corresponding to the key matching the longest prefix as well as the remaining text.

### AcTrie

The `AcTrie` module defines an `AcTrie` class implementing the `IDictionary` interface, generic over the key and value.
Additionally `AcTrie` implements an [Aho-Corasick](https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm) state machine.
This functionality is used to provide the `Search` and `LongestMatch` methods.


## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
