# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added a changelog

### Changed

- Renamed `ACTrie` to `AhoCorasick` to avoid collision with package namespace
- Removed `struct` constraint on `Trie` and `AhoCorasick` type parameters
- Changed signature of `Trie.ConsumeLongestPrefix` method to return a `Match` type on success or null on failure.

## [0.1.0] - 2024-08-09

### Added

- `Trie` class which implements dictionary interface and allows finding longest prefixes
- `ACTrie` class which implements an Aho-Corasick automaton

[unreleased]: https://github.com/raineszm/actrie/compare/0.2.0...HEAD
[0.1.0]: https://github.com/raineszm/actrie/releases/tag/0.1.0