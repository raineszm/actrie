namespace AcTrie
{
    public class AcTrie
    {
        // import { Mapping } from './mapping';
        //
        // export interface Indexable<K> {
        //   [i: number]: K;
        //
        //   length: number;
        //   slice: (start: number, end?: number) => Indexable<K>;
        // }
        //
        // interface Suffix<K, V> {
        //   node: ACNode<K, V>;
        //   shift: number;
        // }
        //
        // export class ACNode<K, V> implements Mapping<Indexable<K>, V> {
        //   children: Map<K, ACNode<K, V>> = new Map();
        //   value?: V;
        //   suffix?: Suffix<K, V>;
        //
        //   public get(k: Indexable<K>): V | undefined {
        //     if (k.length === 0) return undefined;
        //     const node = Array.from(this.iterPath(k)).pop();
        //     if (node === this || node?.value === undefined) return undefined;
        //     return node.value;
        //   }
        //
        //   public set(k: Indexable<K>, v: V): this {
        //     // Key is exhausted
        //     // We are setting this node
        //     if (k.length === 0) {
        //       this.value = v;
        //       return this;
        //     }
        //     let child = this.children.get(k[0]);
        //     // If no matching child, create it
        //     if (child === undefined) {
        //       child = new ACNode<K, V>();
        //       this.children.set(k[0], child);
        //     }
        //     child.set(k.slice(1), v);
        //     return this;
        //   }
        //
        //   public delete(k: Indexable<K>): boolean {
        //     if (k.length === 0) return false;
        //
        //     const nodes = Array.from(this.iterPath(k));
        //     if (nodes.length < k.length + 1) return false;
        //
        //     const target = nodes.pop();
        //     if (target?.value === undefined) return false;
        //     for (let i = Math.min(k.length, nodes.length) - 1; i >= 0; i--) {
        //       const parent = nodes[i];
        //       if (parent === undefined) {
        //         throw new Error('accessed a child of an undefined node');
        //       }
        //       const edge = k[i];
        //       parent.children.delete(edge);
        //       if (parent.children.size > 0 || parent.value !== undefined) {
        //         break;
        //       }
        //     }
        //     return true;
        //   }
        //
        //   public has(k: Indexable<K>): boolean {
        //     return this.get(k) !== undefined;
        //   }
        //
        //   public get size(): number {
        //     let total = 0;
        //     if (this.value !== undefined) {
        //       total++;
        //     }
        //     for (const child of this.children.values()) {
        //       total += child.size;
        //     }
        //     return total;
        //   }
        //
        //   public *iterPath(
        //     k: Indexable<K>,
        //   ): IterableIterator<ACNode<K, V> | undefined> {
        //     yield this;
        //     if (k.length === 0) {
        //       return;
        //     }
        //
        //     const child = this.children.get(k[0]);
        //     if (child !== undefined) {
        //       yield* child.iterPath(k.slice(1));
        //     } else {
        //       yield undefined;
        //     }
        //   }
        //
        //   public clear(): this {
        //     this.value = undefined;
        //     this.children.clear();
        //     return this;
        //   }
        //
        //   public *[Symbol.iterator](): IterableIterator<ACNode<K, V>> {
        //     yield this;
        //     for (const node of this.children.values()) {
        //       yield* node;
        //     }
        //   }
        // }
        //
        // export class ACTrie<K, V> implements Mapping<Indexable<K>, V> {
        //   root: ACNode<K, V> = new ACNode<K, V>();
        //   modified: boolean = true;
        //
        //   public get(k: Indexable<K>): V | undefined {
        //     return this.root.get(k);
        //   }
        //
        //   public set(k: Indexable<K>, v: V): this {
        //     this.root.set(k, v);
        //     this.modified = true;
        //     return this;
        //   }
        //
        //   public delete(k: Indexable<K>): boolean {
        //     const result = this.root.delete(k);
        //     this.modified = true;
        //     return result;
        //   }
        //
        //   public has(k: Indexable<K>): boolean {
        //     return this.root.has(k);
        //   }
        //
        //   public get size(): number {
        //     return this.root.size;
        //   }
        //
        //   public clear(): this {
        //     this.root.clear();
        //     return this;
        //   }
        //
        //   public *leaves(): IterableIterator<ACNode<K, V>> {
        //     for (const node of this.root) {
        //       if (node.children.size === 0) yield node;
        //     }
        //   }
        //
        //   public *search(haystack: Indexable<K>): IterableIterator<Needle<V>> {
        //     this.lock();
        //
        //     // The slice of the haystack we are looking at currently
        //     // It's convenient to work with inclusive indices
        //     // internally, but we return indices in the same format
        //     // as a slice i.e. [start, end)
        //     let first = 0;
        //     let last = 0;
        //     let node = this.root;
        //
        //     // Each time through this loop
        //     // we start at the root of the trie
        //     // with a single character
        //     // and expand the matching string by moving
        //     // down the trie and increasing last
        //     while (last < haystack.length) {
        //       // Fetch next unread key element
        //       let k = haystack[last];
        //
        //       if (!this.root.children.has(k)) {
        //         last++;
        //         first = last;
        //         continue;
        //       }
        //
        //       // Move down the tree until we can no longer
        //       while (node.children.has(k)) {
        //         // while the next character matches
        //         node = node.children.get(k) as ACNode<K, V>; // fetch the matched node
        //
        //         if (node.value !== undefined) {
        //           // if that node has a value we have a new matching substring
        //           // Note the indices here are of the form [start, end)
        //           yield { start: first, end: last + 1, value: node.value }; // Starting index, end_index, value
        //         }
        //
        //         // If we've reached the end of the haystack
        //         // we can't go any further down the tree
        //         // so stop trying
        //         if (last + 1 >= haystack.length) return;
        //
        //         // Otherwise, advance the right edge
        //         last++;
        //         // And fetch next unread character
        //         k = haystack[last];
        //       }
        //
        //       // We've now gone as far down the trie as we can
        //       // And we must traverse back up along the suffix links
        //       // moving start up as appropriate until we can start
        //       // going down the tree again
        //       while (node.suffix !== undefined && !node.children.has(k)) {
        //         let shift;
        //         // Move up the suffix link
        //         ({ node, shift } = node.suffix);
        //
        //         // Trim off the appropriate number of entries
        //         // at the beginning of the haystack
        //         first += shift;
        //
        //         // If this node is a match, yield it
        //         if (node.value !== undefined) {
        //           // Note inclusive form [start, end)
        //           yield {
        //             start: first,
        //             end: last + 1,
        //             value: node.value,
        //           }; // Starting index, end_index, node
        //         }
        //       }
        //
        //       if (node.suffix === undefined) {
        //         // Move to the next entry and set the width of the match to 0
        //         first = last;
        //       }
        //     }
        //   }
        //
        //   public longestMatch(haystack: Indexable<K>): Needle<V> | undefined {
        //     let match;
        //     let length = 0;
        //     for (const needle of this.search(haystack)) {
        //       const needleLength = needle.end - needle.start;
        //       if (needleLength > length) {
        //         length = needleLength;
        //         match = needle;
        //       }
        //     }
        //     return match;
        //   }
        //
        //   public lock(): void {
        //     if (!this.modified) return;
        //     this.computeSuffixes();
        //     this.modified = false;
        //   }
        //
        //   private computeSuffixes(): void {
        //     const nodeQueue = [this.root];
        //     while (nodeQueue.length > 0) {
        //       const parent = nodeQueue.shift() as ACNode<K, V>;
        //       for (const [k, child] of parent.children.entries()) {
        //         nodeQueue.push(child);
        //         if (parent === this.root) {
        //           child.suffix = { node: this.root, shift: 1 };
        //           continue;
        //         }
        //
        //         let { node: target, shift } = parent.suffix as Suffix<K, V>;
        //         let shiftAdd;
        //         while (!target.children.has(k) && target.suffix !== undefined) {
        //           ({ node: target, shift: shiftAdd } = target.suffix);
        //           shift += shiftAdd;
        //         }
        //         child.suffix = {
        //           node: target.children.get(k) ?? this.root,
        //           shift,
        //         };
        //       }
        //     }
        //   }
        // }
        //
        // export interface Needle<T> {
        //   start: number;
        //   end: number;
        //   value: T;
        // }
        //
    }
}