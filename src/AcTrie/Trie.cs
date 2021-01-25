namespace AcTrie
{
    public class Trie
    {
//         import { Mapping } from './mapping';
//
// export class Trie<T> implements Mapping<string, T> {
//   private readonly edges: Map<string, Edge<T>> = new Map();
//
//   constructor(public value?: T) {}
//   public get(key: string): T | undefined {
//     const node = this.findNode(key);
//     return node?.value;
//   }
//
//   public set(key: string, value: T): this {
//     const walker = new PathWalker(this, key);
//     walker.fastForward();
//
//     // We have found a node with this key
//     // Set the value on that node
//     if (walker.remainder.length === 0) {
//       walker.currentNode.value = value;
//       return this;
//     }
//
//     // There is no further progress to made in this key
//     // We need add this key as a child of the current node
//     walker.currentNode.addChild(walker.remainder, new Trie(value));
//
//     return this;
//   }
//
//   public has(key: string): boolean {
//     return this.findNode(key) !== undefined;
//   }
//
//   public delete(key: string): boolean {
//     if (key.length === 0) keyError('');
//     const walker = new PathWalker(this, key);
//     let path = Array.from(walker);
//
//     // Target node not found
//     if (walker.remainder.length > 0) return false;
//
//     // We walk up the tree backwards from the target node
//     const popped = path.pop();
//     if (popped === undefined) return false;
//     const { target } = popped;
//     // There is no node with this key
//     if (target.value === undefined) return false;
//
//     // target is the node to be deleted
//     const last = path[path.length - 1];
//
//     // target is a child of this node
//     if (last === undefined) {
//       return this.removeChild(key);
//     } else {
//       const { remainder: edgeFromParent, target: parent } = last;
//       const result = parent.removeChild(edgeFromParent);
//       path = this.trim(path);
//       this.compress(path);
//       return result;
//     }
//   }
//
//   *[Symbol.iterator](): IterableIterator<[string, T]> {
//     for (const [c, { keyTail, target }] of this.edges.entries()) {
//       const base = `${c}${keyTail}`;
//       if (target.value !== undefined) {
//         yield [base, target.value];
//       }
//       for (const [name, value] of target) {
//         yield [`${base}${name}`, value];
//       }
//     }
//   }
//
//   public get size(): number {
//     let total = 0;
//     if (this.value !== undefined) {
//       total++;
//     }
//     if (this.edges.size > 0) {
//       for (const { target } of this.edges.values()) {
//         total += target.size;
//       }
//     }
//     return total;
//   }
//
//   clear(): void {
//     this.value = undefined;
//     this.edges.clear();
//   }
//
//   public consumeLongestPrefix(key: string): [T | undefined, string] {
//     let value;
//     let valueKey = key;
//     for (const { remainder, target } of new PathWalker(this, key)) {
//       if (target.value !== undefined) {
//         value = target.value;
//         valueKey = remainder;
//       }
//     }
//     return [value, valueKey];
//   }
//
//   public *leaves(): IterableIterator<Trie<T>> {
//     if (this.edges.size === 0) {
//       yield this;
//       return;
//     }
//     for (const { target } of this.edges.values()) {
//       yield* target.leaves();
//     }
//   }
//
//   getChild(key: string): PathStep<T> | undefined {
//     // An empty key means _this node_
//     if (key.length === 0) return { remainder: '', target: this };
//
//     // We track edges by the first letter of their key
//     const c = key[0];
//
//     const edge = this.edges.get(c);
//     // No matching edge
//     if (edge === undefined) return undefined;
//
//     const { keyTail, target } = edge;
//     const i = keyTail.length;
//
//     // First letter matches but the full edge does not
//     if (key.slice(1, 1 + i) !== keyTail) return undefined;
//
//     // Strip off the consumed characters and return remaining key
//     // and matched node
//     return { remainder: key.slice(i + 1), target };
//   }
//
//   protected addChild(key: string, node: Trie<T>): void {
//     const c = key[0];
//     const keyTail = key.slice(1);
//
//     if (!this.edges.has(c)) {
//       this.edges.set(c, { keyTail, target: node });
//     }
//     // We have a partial match with an existing edge
//     // And must split it
//     this.split(c, keyTail, node);
//   }
//
//   protected removeChild(key: string): boolean {
//     const c = key[0];
//     const rest = key.slice(1);
//
//     const edge = this.edges.get(c);
//     if (edge === undefined) return false;
//
//     const { keyTail, target } = edge;
//
//     if (rest.slice(0, keyTail.length) !== keyTail) return false;
//
//     this.edges.delete(c);
//
//     // Absorb any orphaned children
//     for (const [
//       k,
//       { keyTail: childRest, target: childNode },
//     ] of target.edges.entries()) {
//       const childKey = `${key}${k}${childRest}`;
//       this.addChild(childKey, childNode);
//     }
//
//     return true;
//   }
//
//   private findNode(key: string): Trie<T> | undefined {
//     const walker = new PathWalker(this, key);
//     walker.fastForward();
//     if (walker.remainder.length > 0) return undefined;
//     return walker.currentNode;
//   }
//
//   private split(c: string, rest: string, node: Trie<T>): void {
//     const edge = this.edges.get(c);
//     if (edge === undefined) {
//       throw new Error(`attempting to split undefined edge ${c}`);
//     }
//     // Look up the edge we're colliding with
//     const { keyTail: splitKey, target: currentNode } = edge;
//
//     // Find longest matching prefix
//     // between keys
//     const newLength = rest.length;
//     const oldLength = splitKey.length;
//
//     let matchLength = 0;
//     for (let i = 0; i < Math.min(oldLength, newLength); i++) {
//       if (rest[i] !== splitKey[i]) {
//         break;
//       }
//       matchLength++;
//     }
//
//     // Our new node is exactly the same key as our old,
//     // so replace it.
//     if (matchLength === newLength && matchLength === oldLength) {
//       this.edges.set(c, { keyTail: rest, target: node });
//       return;
//     }
//
//     // Our new node needs to be inserted between
//     // the parent and current child
//     if (matchLength === newLength) {
//       node.addChild(splitKey.slice(matchLength), currentNode);
//       this.edges.set(c, { keyTail: rest, target: node });
//       return;
//     }
//
//     // New node is a child of existing node
//     // This is an abuse of the add_child method
//     // But we allow it because it's sane
//     if (matchLength === oldLength) {
//       currentNode.addChild(rest.slice(matchLength), node);
//       return;
//     }
//
//     const newNode = new Trie<T>();
//     newNode.addChild(splitKey.slice(matchLength), currentNode);
//     newNode.addChild(rest.slice(matchLength), node);
//     this.edges.set(c, {
//       keyTail: splitKey.slice(0, matchLength),
//       target: newNode,
//     });
//   }
//
//   private trim(path: Array<PathStep<T>>): Array<PathStep<T>> {
//     // Pull out the parent of the removed node
//     const popped = path.pop();
//     if (popped === undefined) {
//       throw new Error('attempted to trim empty path');
//     }
//     let { remainder, target } = popped;
//
//     // if path is empty it means that node is the root, i.e. self
//     // Since we can't delete self, we're done
//     //
//     // We only need to delete nodes until
//     // we have removed all dangling leaves
//     // If a node has edges we are not a leaf and can stop trimming
//     //
//     // If node.value is not None
//     // we have a value we are a valid leaf node
//     // and we can stop trimming
//     while (
//       path.length > 0 &&
//       target?.edges?.size === 0 &&
//       target?.value === undefined
//     ) {
//       const { remainder: edgeFromParent, target: parent } = path[
//         path.length - 1
//       ];
//       parent.removeChild(
//         edgeFromParent.slice(edgeFromParent.length - remainder.length),
//       );
//       const popped = path.pop();
//       if (popped === undefined) {
//         throw new Error('empty path!');
//       }
//       remainder = popped.remainder;
//       target = popped.target;
//     }
//
//     path.push({ remainder, target });
//
//     // Return the remaining ancestors for compression
//     return path;
//   }
//
//   private compress(path: Array<PathStep<T>>): void {
//     let popped = path.pop();
//     if (popped === undefined) {
//       throw new Error('attempted to compressed empty path');
//     }
//     const { remainder: key, target: node } = popped;
//
//     const toCompress = [];
//     for (const x of path.reverse()) {
//       if (x.target.value === undefined && x.target.edges.size === 1) {
//         toCompress.push(x);
//       } else {
//         break;
//       }
//     }
//
//     if (toCompress.length === 0) {
//       return;
//     }
//
//     popped = toCompress.pop();
//     if (popped === undefined) {
//       throw new Error('attempted to compressed empty path');
//     }
//     const { remainder: edgeFromParent, target: parent } = popped;
//     const keyTail = `${toCompress
//       .reverse()
//       .map((x) => x.remainder)
//       .join('')}${key}`;
//     const newKey = edgeFromParent.slice(
//       0,
//       edgeFromParent.length - keyTail.length,
//     );
//     parent.removeChild(
//       edgeFromParent.slice(0, edgeFromParent.length - key.length),
//     );
//     parent.addChild(newKey, node);
//   }
// }
//
// function keyError(key: string): never {
//   throw new Error(`Key "${key}" does not exist in trie`);
// }
//
// export interface Edge<T> {
//   keyTail: string;
//   target: Trie<T>;
// }
//
// export interface PathStep<T> {
//   remainder: string;
//   target: Trie<T>;
// }
// export class PathWalker<T> implements IterableIterator<PathStep<T>> {
//   currentNode: Trie<T>;
//   remainder: string;
//   private done: boolean = false;
//
//   constructor(root: Trie<T>, key: string) {
//     this.currentNode = root;
//     this.remainder = key;
//   }
//
//   public [Symbol.iterator](): IterableIterator<PathStep<T>> {
//     return this;
//   }
//
//   public next(): IteratorResult<PathStep<T>, undefined> {
//     const step = { remainder: this.remainder, target: this.currentNode };
//     // If exhausted is true then we have run out of nodes
//     if (this.done) {
//       return {
//         done: true,
//         value: undefined,
//       };
//     }
//
//     if (this.remainder.length > 0) {
//       // Get the next step toward the sought node
//       const child = this.currentNode.getChild(this.remainder);
//
//       // We have run out of matching nodes
//       if (child === undefined) {
//         this.done = true;
//       } else {
//         this.remainder = child.remainder;
//         this.currentNode = child.target;
//       }
//     } else {
//       // If we have consumed the whole key then we have found our node
//       this.done = true;
//     }
//     return {
//       done: false,
//       value: step,
//     };
//   }
//
//   public fastForward(): void {
//     do {
//       const { done } = this.next();
//       if (done !== undefined && done) break;
//     } while (true);
//   }
// }
//
    }
}