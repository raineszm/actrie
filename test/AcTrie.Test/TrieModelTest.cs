namespace AcTrie.Test
{
    public class TrieModelTest
    {
//         import * as fc from 'fast-check';
// import { Trie } from '../src/trie';
//
// type TrieModel<T> = Map<string, T>;
//
// class SetCommand implements fc.Command<TrieModel<number>, Trie<number>> {
//   constructor(readonly key: string, readonly value: number) {}
//
//   check = (): boolean => true;
//
//   run(model: TrieModel<number>, trie: Trie<number>): void {
//     trie.set(this.key, this.value);
//     model.set(this.key, this.value);
//   }
//
//   toString(): string {
//     return `set("${this.key}", ${this.value})`;
//   }
// }
//
// class DeleteCommand implements fc.Command<TrieModel<number>, Trie<number>> {
//   constructor(readonly key: string) {}
//
//   check(model: TrieModel<number>): boolean {
//     return model.size > 0;
//   }
//
//   run(model: TrieModel<number>, trie: Trie<number>): void {
//     // eslint-disable-next-line jest/no-standalone-expect
//     expect(trie.delete(this.key)).toBe(model.delete(this.key));
//   }
//
//   toString(): string {
//     return `delete("${this.key}")`;
//   }
// }
//
// class GetCommand implements fc.Command<TrieModel<number>, Trie<number>> {
//   constructor(readonly key: string) {}
//   check(model: TrieModel<number>): boolean {
//     return model.size > 0;
//   }
//
//   run(model: TrieModel<number>, trie: Trie<number>): void {
//     const got = trie.get(this.key);
//     // eslint-disable-next-line jest/no-standalone-expect
//     expect(got).toBe(model.get(this.key));
//   }
//
//   toString(): string {
//     return `get("${this.key}")`;
//   }
// }
//
// class SizeCommand implements fc.Command<TrieModel<number>, Trie<number>> {
//   check = (): boolean => true;
//
//   run(model: TrieModel<number>, trie: Trie<number>): void {
//     // eslint-disable-next-line jest/no-standalone-expect
//     expect(trie.size).toBe(model.size);
//   }
//
//   toString = (): string => 'size()';
// }
//
// class LeavesCommand implements fc.Command<TrieModel<number>, Trie<number>> {
//   check = (model: TrieModel<number>): boolean => model.size > 0;
//
//   run(_model: TrieModel<number>, trie: Trie<number>): void {
//     const leaves = Array.from(trie.leaves());
//     // eslint-disable-next-line jest/no-standalone-expect
//     expect(
//       leaves.every((node) => node.value !== undefined || node !== trie),
//     ).toBeTruthy();
//   }
//
//   toString = (): string => 'leaves()';
// }
//
// const keyArb = fc.string({ minLength: 1 });
// const trieCommands = [
//   fc.constant(new SizeCommand()),
//   fc.constant(new LeavesCommand()),
//   keyArb.map((key) => new GetCommand(key)),
//   keyArb.map((key) => new DeleteCommand(key)),
//   fc
//     .tuple(keyArb, fc.integer())
//     .map(([key, value]) => new SetCommand(key, value)),
// ];
//
// describe('trie', () => {
//   // eslint-disable-next-line jest/expect-expect
//   it('passes model test', () => {
//     fc.assert(
//       fc.property(
//         fc.commands(trieCommands, {
//           maxCommands: 100,
//         }),
//         (cmds) => {
//           const s = (): { model: TrieModel<number>; real: Trie<number> } => ({
//             model: new Map<string, number>(),
//             real: new Trie<number>(),
//           });
//           fc.modelRun(s, cmds);
//         },
//       ),
//     );
//   });
// });
//
    }
}