// @ts-check

        /** @type {(string) => boolean} */
        const isKey = (c) => {
            const m = keyCode(c);
            return m <= 25 && m >= 0;
        }
        /** @type {(string) => boolean} */
        const isDoor = (c) => {
            const m = c.charCodeAt(0) - "A".charCodeAt(0);
            return m <= 25 && m >= 0;
        }
        /** @type {(string) => string} */
        const keyForDoor = (c) => {
            return c.toLowerCase();
        }
        /** @type {(string) => boolean} */
        const isWall = (c) => {
            return c === "#";
        }
        /** @type {(string) => boolean} */
        const isCurrentPosition = (c) => {
            const m = c.charCodeAt(0) - "0".charCodeAt(0);
            return c == "@" || (m >= 0 && m <= 9);
        }
        /** @type {(string) => number} */
        const keyCode = (c) => {
            return c.charCodeAt(0) - "a".charCodeAt(0);
        }
        /** @type {(number) => string} */
        const letterForCode = (l) => {
            return String.fromCharCode("a".charCodeAt(0) + l);
        }
        /** @type {(a1: boolean[], a2: boolean[]) => boolean} */
        const arrayOfBoolsIsSubsetOfOther = (a1, a2) => {
            let ok = true;
            for (let i = 0; i < a1.length; i++) {
                if (a1[i] === true && a2[i] !== true) {
                    ok = false;
                    break;
                }
            }
            return ok;
        }

        /**
         * @param {{x: number, y: number}} coord
         * @returns {{x: number, y: number}[]}
        */
        const getDirectNeighbours = (coord) => {
            const n = [];
            const moves = [
                { x: 1, y: 0},
                { x: -1, y: 0},
                { x: 0, y: 1},
                { x: 0, y: -1}
            ];
            moves.forEach(m => {
                n.push({ x: coord.x + m.x, y: coord.y + m.y });
            })
            return n;
        }

        class KeyedNode {
            /** @type {string} */
            node;
            /** @type {KeyCollection} */
            keys;

            /**
             * @param {string} node
             * @param {KeyCollection} keys
             * */
            constructor (node, keys) {
                this.node = node;
                this.keys = keys;
            }

            /**
             * @param {KeyCollection} keysInPossession
             * @returns {boolean}
             * */
            isAvailable(keysInPossession) {
                return arrayOfBoolsIsSubsetOfOther(this.keys.toArray(), keysInPossession.toArray());
            }
        }

        /**
         * @template T
         * @param {T} destination
         * @param {number} pathLength
         * @returns {{destination: T, pathLength: number}}
         * */
        function Path(destination, pathLength) {
            return { destination, pathLength };
        }

        class OrderedHeap {
            /** @type {{ item: KeyAndNodeCollection, pathLength: number}[]} */
            heap = [];

            add(item, pathLength) {
                const existing = this.heap.find(h => h.item.keys.state == item.keys.state && h.item.nodes.state == item.nodes.state);
                if (existing == null) {
                    this.heap.push({ item, pathLength });
                } else {
                    if (existing.pathLength > pathLength) {
                        existing.pathLength = pathLength;
                    }
                }
            }
            key(a) {
                return a.pathLength; // WHY THIS NOT WORK? It produced wrong numbers / a.item.keys.state
            }
            next() {
                this.heap.sort((a, b) => this.key(a) - this.key(b));
                const item = this.heap.shift();
                return item;
            }
        }

        class KeyCollection {
            /** @type {number} */
            state;

            /** @param {number} state */
            constructor(state) {
                this.state = state;
            }

            static from(keys) {
                let state = 0;
                keys.forEach(key => {
                    if (isKey(key)) {
                        state = state | (1 << keyCode(key));
                    }
                });
                return new KeyCollection(state);
            }

            static none = new KeyCollection(0);
            static all = new KeyCollection((1 << 26) - 1);

            /**
             * @param {KeyCollection} other
             * */
            isProperSubsetOf(other) {
                return arrayOfBoolsIsSubsetOfOther(this.toArray(), other.toArray()) && other.state != this.state;
            }

            /**
             * @returns {boolean}
             * */
            contains(key) {
                if (isKey(key)) {
                    var mask = 1 << keyCode(key);
                    return (this.state & mask) > 0;
                }
            }

            containsCollection(other) {
                return (this.state & other.state) === other.state;
            }

            /**
             * @param {string} key
             * @returns {KeyCollection}
             * */
            add(key) {
                if (isKey(key)) {
                    return new KeyCollection(this.state | (1 << keyCode(key)));
                }
            }

            toArray() {
                /** @type {boolean[]} */
                const bools = [];
                for (let i = 0; i <= 25; i++) {
                    const l = letterForCode(i);
                    bools.push(this.contains(l))
                }
                return bools;
            }
        }

        class NodeCollection {
            /** @type {number} */
            state;

            /** @param {number} state */
            constructor(state) {
                this.state = state;
            }

            static from(nodes) {
                let state = 0;
                nodes.forEach((node, index) => {
                    const shift = index * 8;
                    state = state | (("" + node).charCodeAt(0) << shift);
                });
                return new NodeCollection(state);
            }

            /**
             * @param {(a: string, b:number) => void} perform
             */
            forEach(perform) {
                for (let i = 0; i <= 3; i++) {
                    const shift = i * 8;
                    const node = (this.state >> shift) & 0xFF;
                    if (node == 0) break
                    perform(String.fromCharCode(node), i);
                }
            }

            /**
             * @param {number} index
             * @param {string} node
             * @returns {NodeCollection}
             * */
            replacingNode(index, node) {
                const shift = index * 8;
                let newState = this.state & ~(0xFF << shift);
                newState = newState | (node.charCodeAt(0) << shift);
                return new NodeCollection(newState)
            }
        }

        class KeyAndNodeCollection {
            /** @type {NodeCollection} */
            nodes;
            /** @type {KeyCollection} */
            keys;
            /** @param {NodeCollection} nodes
             *  @param {KeyCollection} keys
             * */
            constructor(nodes, keys) {
                this.nodes = nodes;
                this.keys = keys;
            }
        }

        class NodeMap {
            /** @type {string[][]} */
            matrix;
            /** @type {{ x: number, y: number }[]} */
            keyCoordinates;

            /** @param {string[][]} matrix
             *  @param {{ x: number, y: number }[]} coordinates
             * */
            constructor(matrix, coordinates) {
                this.matrix = matrix;
                this.keyCoordinates = coordinates;
            }

            /** @param {string} input */
            static from(input) {
                const lines = input.split("\n");
                const matrix = [];
                /** @type {{ x: number, y: number }[]} */
                const keyCoordinates = [];
                let currIndex = 0;
                lines.forEach((line, y) => {
                    const chars = line.split("");
                    chars.forEach((char, x) => {
                        const node = isCurrentPosition(char) ? ("" + ++currIndex) : char;
                        if (isCurrentPosition(node) || isKey(node)) {
                            keyCoordinates[node.charCodeAt(0)] = { x, y };
                        }
                        if (matrix[x] == null) matrix[x] = [];
                        matrix[x][y] = node;
                    });
                });
                return new NodeMap(matrix, keyCoordinates);
            }


            /**
             * @param {{x: number, y: number}} coordinate
             * */
            nodeAt(coordinate) {
                return this.matrix[coordinate.x][coordinate.y]
            }

            coordinateFor(node) {
                return this.keyCoordinates[node.charCodeAt(0)];
            }

            /**
             * @param {{x: number, y: number}} coordinate
             * @param {KeyCollection} keysInPossession
             * @returns {boolean}
             * */
            isAccessible(coordinate, keysInPossession) {
                const x = coordinate.x
                const y = coordinate.y

                if (this.matrix[x] != null) {
                    const s = this.matrix[x]
                    if (s[y] != null) {
                        const node = s[y]
                        const door = isDoor(node);
                        return (!door && !isWall(node)) || (door && keysInPossession.contains(node.toLowerCase()))
                    }
                }
                return false
            }

            getNodesWhere(where) {
                const result = [];
                this.matrix.forEach(line => {
                    line.forEach(char => {
                        if (where(char)) {
                            result.push(char);
                        }
                    });
                });
                return result;
            }

            /** 
             * @param {string} from
             * @param {({destination: KeyedNode, pathLength: number}) => boolean} onFound
            */
            findAllKeyPaths(from, onFound) {
                /**
                 * @param {{x: number, y: number}} currentCoordinate
                 * @param {string} currentNode
                 * @param {{x: number, y: number}[]} visited
                 * @param {number} pathLength
                 * @param {KeyCollection} requiredKeys
                 * */
                const recurse = (currentCoordinate, currentNode, visited, pathLength, requiredKeys) => {
                    if (isKey(currentNode) && pathLength != 0) {
                        // Found
                        if (onFound(Path(new KeyedNode(currentNode, requiredKeys), pathLength))) {
                            return;
                        }
                    }
                    //pathEvaluationCount++
                    visited.push(currentCoordinate)
                    getDirectNeighbours(currentCoordinate).forEach(coord => {
                        if (this.isAccessible(coord, KeyCollection.all) && !visited.find(v => v.x === coord.x && v.y === coord.y)) {
                            const neighbourNode = this.nodeAt(coord)
                            const nextRequiredKeys = isDoor(neighbourNode) ? (requiredKeys.add(keyForDoor(neighbourNode))) : requiredKeys;
                            recurse(coord, neighbourNode, visited, pathLength + 1, nextRequiredKeys)
                        }
                    });
                    visited.splice(visited.findIndex(v => v.x === currentCoordinate.x && v.y === currentCoordinate.y), 1);
                }
                recurse(this.coordinateFor(from), from, [], 0, KeyCollection.none)
            }

            edgeCache = {}; // HashMap<Node, Collection<Path<KeyedNode>>>(32)

            /** 
             * @param {string} node
             * @param {KeyCollection} currentKeys
             * @returns {{ [key: string]: {destination: KeyedNode, pathLength: number} }}
             * */
            edgesFrom(node, currentKeys) {
                if (this.edgeCache[node + currentKeys.state] != null) return this.edgeCache[node + currentKeys.state];

                /** @type {{ [key: string]: {destination: KeyedNode, pathLength: number} }} */
                const foundPaths = {}; // HashMap<Node, Path<KeyedNode>>(32)
                this.findAllKeyPaths(node, /** @param {{destination: KeyedNode, pathLength: number}} foundPath */ foundPath => {
                    // As an optimization we only need to find keys which are not yet collected, because at the time this
                    // method is called the optimal path to those nodes will already be found using Dijkstra
                    if (!currentKeys.contains(foundPath.destination.node)) {
                        // Stop when a key is found that is not in the current collection.
                        // It does not make sense to traverse the same path beyond that,
                        // because we arrive at a different key calling this method again
                        const existingEdge = foundPaths[foundPath.destination.node]
                        if (
                            existingEdge == null || // add non-existent paths
                            foundPath.pathLength < existingEdge.pathLength  || // add shorter paths
                            foundPath.destination.keys.isProperSubsetOf(existingEdge.destination.keys) // less keys needed
                        ) {
                            foundPaths[foundPath.destination.node] = foundPath
                        }
                        return true;
                    } else {
                        return false
                    }
                });
                this.edgeCache[node + currentKeys.state] = foundPaths;
                return foundPaths;
            }

            // Relevant means that the key the edge points to has not been collected yet and that it is reachable given the keys in possession.
            /** 
             * @param {KeyAndNodeCollection} keyAndNodeCollection
             * @param {(edge: {destination: KeyedNode, pathLength: number}, index: number) => void} perform
             * */
            forEachRelevantEdge(keyAndNodeCollection, perform) {
                keyAndNodeCollection.nodes.forEach(/** @param {string} subNode @param {number} index */(subNode, index) => {
                    const es = this.edgesFrom(subNode, keyAndNodeCollection.keys);
                    Object.keys(es).forEach(edgeK => {
                        const edge = es[edgeK];
                        // Only add keys that have not yet been collected and from paths that are actually available
                        if (edge.destination.isAvailable(keyAndNodeCollection.keys) && !keyAndNodeCollection.keys.contains(edge.destination.node)) {
                            perform(edge, index)
                        }
                    });
                });
            }

            minimumPathToCollectAllKeys() { debugger;
                // Use a FibonacciHeap to optimize performance. A FibonacciHeap allows reordering of the priority queue without a
                // remove and subsequent add.
                // The heap is automatically ordered by NodePath ascending (increasing path length, so shortest paths are first)
                const pending = new OrderedHeap();

                // The initial positions
                const initialNodes = this.getNodesWhere((n) => isCurrentPosition(n));
                
                // This is the collection of keys to check against for completeness
                const completeKeyCollection = KeyCollection.from(this.getNodesWhere((n) => isKey(n)));
                
                // The initial nodes (starting positions of the robots) in combination with an empty key collection
                const initialNodeCollection = new KeyAndNodeCollection(NodeCollection.from(initialNodes), KeyCollection.none);

                // Add the initial nodes to the pending queue
                pending.add(initialNodeCollection, 0)
                /** @type {KeyAndNodeCollection[]} */
                const settled = [];
                const founds = [];

                let i = 0;
                while (true) {
                    // TODO
                    //if (i > 20000) break;

                    // If the queue is empty: break out of the loop
                    /** @type {{ item: KeyAndNodeCollection, pathLength: number}} */
                    const current = pending.next();
                    if (current == null) break;
                    if (current.pathLength > Math.min(...founds)) break;

                    // Process the node collection with the lowest path length
                    const currentNodeCollection = current.item;

                    // If this collection contains all the keys we were looking for: we're done!
                    if (currentNodeCollection.keys.containsCollection(completeKeyCollection)) {
                        founds.push(current.pathLength);
                    }

                    // Add to the settled set to avoid revisiting the same node
                    settled.push(currentNodeCollection)

                    this.forEachRelevantEdge(currentNodeCollection, (edge, nodeIndex) => {
                        const neighbour = new KeyAndNodeCollection(currentNodeCollection.nodes.replacingNode(nodeIndex, edge.destination.node),
                            currentNodeCollection.keys.add(edge.destination.node))
                        if (!settled.find(a => a.keys.state == neighbour.keys.state && a.nodes.state == neighbour.nodes.state)) {
                            pending.add(neighbour, current.pathLength + edge.pathLength)
                        }
                    });
                }
                return founds;
                return Math.min(...founds);
            }
        }


        function getMinimumNumberOfMovesToCollectAllKeys(input) {
            const map = NodeMap.from(input);
            const result = map.minimumPathToCollectAllKeys();
            console.log(result);
        }

getMinimumNumberOfMovesToCollectAllKeys(`#################################################################################
#...#..j..........#.........#...........#.......#........n......#.....#........m#
#.#.#.#.###########.###.#####.#######O#####.###.#.#########.#.###.#.###.###.###.#
#.#.#.#...............#.Z.......#...#...#...#.#.#.#.......#.#.....#.....#.X.#...#
#.#.#.###########################P#.#.#.#.###.#.###.#####.###.###########.###.###
#.#.#.#.......#.........#......b..#.#.#.#.#.......#...#.#...#.....#.......#.....#
#.#.#.#.###.###.#######.#.#########.###.#.#######.###.#.###.#####.#.#############
#.#.#.#.#...#...#..l#...#.#.......#.#...#.....#.......#.#...#.....#.........#..k#
#.###.#.###.#.#####.#.###.#######.#.#.###.###.#########.#.###.#####.#######.#.#.#
#...#.#...#.#.#...#...#...#.....#.#...#.#...#...#.....#.#.#...#...#...#...#...#.#
#.#.#.#.#.###.#.#U###.###.#.###.#.#####.###.###.#.###.#.#.#.###.#.#####.#.#####F#
#.#.#.#.#...#...#...#.....#.#...#.......#...#g#...#.#.#...#.#...#...#.E.#.....#.#
#.#.#.#.###.#######.#######.###.###Y#.###.###.#####.#.###.###.#####.#.#######.#.#
#.#...#...#...#.....#......a..#...#.#...#.....#.....#...#.....#.......#.....#...#
#.#####D#####.#.#######.#####.###.###.#.#####.#.###.###.#################.#####.#
#.M...#.#...#.#.#c....#...#...#.#...#.#.#...#.#.#.#.#.#.#...............#...#...#
#####.###.#.#.#H#.###.#####.###.###.#.#.#.###.#.#.#.#.#.#.###########.#.###.#.###
#...#.....#.#.#.#...#...#...#.....#.#.#.#...#.#.#.....#.......#.....#.#.#...#.#.#
#.#.#######.#.#.#.#####.#.###.#.###.###.#.#.#.#.#####.#########.###.###.#.###.#.#
#.#.#....x#.#.#...#.....#.#...#.#..i#...#.#.#.#...#...#.........#.....#.#...#...#
#.###K#.#.#.#.#####.#####.#.###.#.###.#####.#.###.#####.###.#########.#.#.#.###.#
#...#.#.#.#.#.....#..r#...#...#.#.#.....#...#.....#.....#...#.......#...#.#.....#
#.#.#.#.###.#.###.#####.#####.#.#.#.###.#.#####.###.#####.#########.#####.#####.#
#.#.#.#.....#.#.........#.....#.#.#...#.#.......#...#...#.#.........#.....#...#.#
###.#.#########.#########.#####.#.#####.#####.###.#.#.#.###.#########.#####.#.#.#
#...#...#.....#.#...........#...#.....#.#..t#.#.#.#.#.#.....#...#.........#.#.#.#
#.#####.#.#.#.#.#####.#####.#########.#.#.#.#.#.#.#.#.#######.###.#########.#.###
#.......#.#.#.#.....#...#.#.....#...#.#.#.#.#...#.#.#.....#.......#...#...#.#...#
#.#########.#.###.#.###.#.#####.#.#.#.#.#.#.#####.#.#####.#########.#.#.#.#.###.#
#.#.....#...#...#.#.#...#...#...#.#...#.#.#.......#.#...#.......#q..#...#...#...#
#.###.#.#.#####.###.###.#.###.###.#####.#.#########.###.#######.#.###########.###
#.....#.#...#.#...#...#.#...#...#...#...#...#...#.#.....#.......#.#...#.....#...#
#######.#.#.#.###.#.#.#.###.###.###.###.#.#.#.#.#.#####.#.#######.#.###.#.#####.#
#.....#.#.#.#.....#.#.#.......#...#.....#.#.#.#.#.#.....#.#...#...#...#.#.....#.#
#.#.###.#.#.###.#####.#####.###.#.#####.#.#.#.#.#.#.#####.#.#.#.#####.#.#####.#.#
#.#.#...#.#...#.#...#.#.....#...#...#...#.#...#.#...#...#...#...#.....#.#.....#.#
###.#.###.###.#.#.#.#.#######.#######.###.#####.#.###.#.#########.#.###.#.###.#.#
#...#.#...#...#.#.#.#...#.....#.....#.#.#.#...#.#.#...#.......#...#.....#...#.#.#
#.###.#####.###.#.#.###.#.#####.###.#.#.#.###.#.###.#######.###.###########.###.#
#...........#.....#.....#.........#....@#@....#...........#...............#w....#
#################################################################################
#.....#.....#.......#.........#........@#@........#.#...................#...#...#
#.###.###.#.#.#####.#.#.#####.#.#####.#.#.#######.#.#.###############.#.#.#.#.#.#
#...#.....#...#.#...#.#...#.#.#.#.#...#.#.......#...#...#...#...#...#.#...#...#.#
###.#####.#####.#.#######.#.#.#.#.#.###.#.#####.###.###.#.###.#.#.#.#.#########.#
#...#...#.#...#.#.....#...#...#...#...#.#.#...#...#.#...#.#...#...#.#.........#.#
#.###.#.###.#.#.#####.#.###.#########.#.#.###.###.###.###.#.#######.###########.#
#.....#.....#.#.....#...#.#.#.......#.#.#.......#.....#...#.#...................#
#############.#####.#####.#.#.#####.#.#.#######.#######.#.#.###.###############.#
#...........#.....#.......#.#...#.#.#.#.#.....#.........#.#...#.#.........#...#.#
#########.#.#####.#.###.###.###.#.#.#.###.#.#.#######T#######.###.#######.#.###.#
#........f#.#...#.#...#.....#...#...#...#.#.#.#.....#.#.......#...#...#...#.#...#
#.#######.###.#.#.#####.#####.###.#####.#.#.#.#.###.#.#.#######.#####.#.###.#.###
#.#.#.....#...#.#.....#.......#...#.....#.#.#.#.#.#...#.#...#.......#.#.#.......#
#.#.#.#####.###.#.###.#########.###.#.###.#.###.#.#####.#.#.#.#####.#.#.#######.#
#.#.#...#...#.#.#...#.#...#...#.#.#.#...#.#...#.#...#...#.#...#...#...#.#.....#.#
#.#.###.#.###.#.###.#.###.#.#.#.#.#.###.#####.#.#.#.#.###.#####.#.#####.#.###.#.#
#.#.....#.#...#...#.#...#...#.#.#...#.#.#.....#.#.#.#.......#...#.#...#....s#.#v#
#.#.#####.#.#.###.###.#.#.###.#.###.#.#.#.#####.###.#####.###.###.#.#.#######.#.#
#.#...#...#.#...#...#.#.#...#.#...#..y#.#.#.........#...#.#...#.....#.........#.#
#.###.#.#######.###.###.#####.###.#####.#.#.#####.###.#.###.#########.###########
#.#...#.........#.#...#.....#...#.#.....#...#...#.#...#...#.#.......#.#.........#
#.#.###########.#.#########.#.###.#.#########.#.#.#.#####.#.#######R###.#######.#
#.#.#.....Q...#...#.........#...#.#.#...#...#.#.#.#.#...#.....#...#.....#...#.C.#
#.###.#.#########.#.###.#######.#.#.#.#.#.#.#.#.#.#.###.#####.#.#.#########.#.###
#...#.#...#...G.#.#...#.#.#.....#.#...#.#.#.#.#.#.#...#...#...#.#.....#.....#.#.#
#.#.#.###.#.###.#.#####.#.#.#.###.#####.#.#.#.#.#.###.#.#.#.#####.###.#.#####.#.#
#.#.#...#.#.#...#.#.....#.#.#.#...#...I.#.#...#.#...#.#.#.#.#.....#.#...#...#...#
#.#S###.#.#.#.#.#.#.#####.#.#.#.###.###.#.#####.#####.#.#.#.###.###.###.#.#.###.#
#.#.....#...#.#.#...#.......#...#...#.#.#.....#.......#.#.#...#.#.#...#.#.#.....#
#.###########.#######.###########.###.#.#.###.#########.#.###.#.#.#.#.#.#.#######
#e#.......#...#.....#.#.......#...#.#...#.#...#...#.#...#.......#...#...#.#.....#
#.#.#######.#.#.#.###.#.###.###.#.#.#.###N#.###.#.#.#.#########.#####.###.#.###.#
#.#.#.......#.#.#.....#...#.#...#.#.....#.#.#...#.#.#.#..p..#...#...#...#h..#.#.#
#.#.#.#########W#########.#.#.###.#####.###.#.###.#.#.#.###.#####.#B#########.#.#
#...#d....#...#.....#...#.#.....#.....#.#...#.#.#.#.#.#...#.#.....#.#.......#...#
###.#####.#.#.#####.#.#.###.#######.#.#.#.###.#.#.#.#.###.#.#.#####.#.#####.#.###
#...#...#...#.....#...#.V.#.#.....#.#.#.#.#...#z#.#...#...#.#...#.#...#.#.L.#.#.#
#.#####.#######.#########.###.###.###.#.#.#.###J#.#####.###.###.#.#####.#.###.#.#
#.............#...............#.....A.#.#.......#........o#.............#....u..#
#################################################################################`)
