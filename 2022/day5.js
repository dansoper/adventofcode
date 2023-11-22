// @ts-check

/** @typedef {string[]} Stack */
/** @typedef {{ count: number, from: number, to: number }} Move */

/**
* @param {string} str
*/
function stacks(str) {
    /** @type {Stack[]} */
    let stacks = [];
    /** @type {Move[]} */
    const moves = [];
    let mode = 1;
    /**
    * @param {Move} move
    */
    function processMove(move) {
        if (mode == 1) {
            for (let i = 0; i < move.count; i++) {
                const removed = stacks[move.from].pop();
                if (removed != null) {
                    if (stacks[move.to] == null) stacks[move.to] = [];
                    stacks[move.to].push(removed);
                }
            }
        } else if (mode == 2) {
            const removed = stacks[move.from].splice(-move.count, move.count);
            stacks[move.to].push(...removed);
        }
    }

    function processMoves() {
        moves.forEach(move => processMove(move));
    }

    /**
    * @param {string} str
    */
    function parseStacks(str) {
        const lines = str.split("\n");
        lines.forEach(line => {
            for (let i = 0; i < line.length; i += 4) {
                if (line[i] == "[") {
                    const stack = (i / 4 + 1);
                    if (stacks[stack] == null) stacks[stack] = [];
                    const letter = line[i+1];
                    stacks[stack].unshift(letter);
                }
            }
        });
    }


    /**
    * @param {string} str
    */
    function parseMoves(str) {
        const lines = str.split("\n");
        lines.forEach(line => {
            const matches = line.match(/move (\d+) from (\d+) to (\d+)/);
            if (matches != null) {
                moves.push({ count: parseInt(matches[1]), from: parseInt(matches[2]), to: parseInt(matches[3]) });
            }
        });        
    }

    /** @returns {string} */
    function getStringFromStacks() {
        let toReturn = "";
        stacks.forEach(stack => {
            if (stack != null) {
                toReturn += stack[stack.length - 1];
            }
        });
        return toReturn;
    }

    const sections = str.split("\n\n");
    parseStacks(sections[0]);
    parseMoves(sections[1]);
    console.log(moves, stacks);
    processMoves();
    console.log(stacks);
    const res = getStringFromStacks();
    console.log("Part 1", res);
    stacks = [];
    parseStacks(sections[0]);
    mode = 2;
    processMoves();
    const res2 = getStringFromStacks();
    console.log("Part 2", res2);
}

var input = `    [B]             [B] [S]        
    [M]             [P] [L] [B] [J]
    [D]     [R]     [V] [D] [Q] [D]
    [T] [R] [Z]     [H] [H] [G] [C]
    [P] [W] [J] [B] [J] [F] [J] [S]
[N] [S] [Z] [V] [M] [N] [Z] [F] [M]
[W] [Z] [H] [D] [H] [G] [Q] [S] [W]
[B] [L] [Q] [W] [S] [L] [J] [W] [Z]
1   2   3   4   5   6   7   8   9 

move 3 from 5 to 2
move 5 from 3 to 1
move 4 from 4 to 9
move 6 from 1 to 4
move 6 from 8 to 7
move 5 from 2 to 7
move 1 from 5 to 4
move 11 from 9 to 7
move 1 from 1 to 9
move 6 from 4 to 6
move 12 from 6 to 7
move 1 from 9 to 2
move 2 from 4 to 6
move 1 from 8 to 9
move 1 from 9 to 4
move 1 from 6 to 1
move 2 from 7 to 5
move 2 from 6 to 7
move 2 from 1 to 6
move 2 from 4 to 7
move 1 from 5 to 4
move 1 from 5 to 6
move 1 from 6 to 1
move 1 from 1 to 3
move 1 from 4 to 1
move 1 from 1 to 4
move 1 from 4 to 5
move 1 from 3 to 9
move 1 from 5 to 1
move 4 from 2 to 1
move 20 from 7 to 8
move 24 from 7 to 3
move 3 from 6 to 4
move 1 from 1 to 9
move 1 from 9 to 3
move 2 from 1 to 2
move 2 from 4 to 1
move 2 from 2 to 1
move 14 from 3 to 6
move 6 from 1 to 6
move 10 from 3 to 2
move 1 from 2 to 3
move 6 from 6 to 5
move 2 from 3 to 4
move 13 from 8 to 4
move 1 from 9 to 7
move 1 from 6 to 3
move 10 from 4 to 2
move 1 from 3 to 6
move 2 from 8 to 7
move 1 from 7 to 2
move 11 from 6 to 8
move 2 from 6 to 1
move 2 from 1 to 3
move 1 from 8 to 6
move 1 from 3 to 9
move 3 from 8 to 2
move 1 from 3 to 6
move 2 from 6 to 4
move 1 from 6 to 5
move 11 from 2 to 9
move 2 from 4 to 6
move 1 from 6 to 1
move 1 from 1 to 5
move 11 from 2 to 7
move 12 from 7 to 5
move 1 from 6 to 2
move 10 from 8 to 7
move 6 from 5 to 3
move 4 from 5 to 4
move 11 from 9 to 7
move 7 from 4 to 9
move 4 from 9 to 6
move 12 from 7 to 3
move 1 from 8 to 9
move 1 from 5 to 1
move 1 from 1 to 2
move 1 from 6 to 9
move 3 from 4 to 1
move 1 from 9 to 7
move 8 from 7 to 2
move 3 from 6 to 1
move 8 from 2 to 3
move 1 from 7 to 4
move 2 from 7 to 2
move 1 from 5 to 2
move 8 from 5 to 1
move 3 from 9 to 6
move 1 from 6 to 2
move 1 from 4 to 5
move 1 from 5 to 4
move 2 from 9 to 3
move 1 from 8 to 6
move 1 from 4 to 5
move 1 from 5 to 1
move 1 from 6 to 8
move 1 from 8 to 1
move 7 from 1 to 5
move 11 from 3 to 7
move 1 from 1 to 9
move 4 from 2 to 1
move 5 from 1 to 3
move 1 from 5 to 9
move 1 from 6 to 3
move 6 from 2 to 1
move 5 from 7 to 3
move 1 from 6 to 8
move 1 from 8 to 4
move 6 from 7 to 9
move 4 from 9 to 8
move 2 from 8 to 9
move 2 from 5 to 8
move 13 from 3 to 7
move 1 from 3 to 8
move 2 from 1 to 9
move 3 from 1 to 5
move 1 from 4 to 1
move 6 from 5 to 9
move 8 from 9 to 8
move 2 from 7 to 3
move 1 from 9 to 7
move 1 from 5 to 2
move 5 from 9 to 8
move 1 from 8 to 7
move 1 from 2 to 9
move 7 from 1 to 2
move 4 from 7 to 5
move 6 from 2 to 3
move 1 from 2 to 1
move 10 from 8 to 9
move 3 from 8 to 9
move 4 from 5 to 1
move 2 from 8 to 6
move 9 from 9 to 8
move 1 from 9 to 6
move 8 from 8 to 4
move 12 from 3 to 5
move 1 from 4 to 2
move 3 from 8 to 1
move 3 from 9 to 7
move 1 from 3 to 2
move 1 from 6 to 9
move 8 from 3 to 8
move 6 from 4 to 5
move 1 from 7 to 6
move 1 from 8 to 1
move 6 from 8 to 7
move 1 from 3 to 6
move 7 from 1 to 5
move 1 from 4 to 9
move 4 from 6 to 5
move 13 from 7 to 5
move 1 from 8 to 2
move 2 from 9 to 3
move 4 from 7 to 2
move 1 from 3 to 8
move 1 from 3 to 4
move 4 from 1 to 2
move 1 from 5 to 7
move 23 from 5 to 6
move 1 from 8 to 6
move 1 from 9 to 4
move 5 from 2 to 6
move 1 from 4 to 9
move 1 from 9 to 3
move 1 from 7 to 8
move 1 from 4 to 3
move 1 from 3 to 7
move 1 from 7 to 5
move 1 from 8 to 7
move 12 from 6 to 1
move 1 from 2 to 5
move 1 from 3 to 1
move 20 from 5 to 2
move 14 from 2 to 4
move 11 from 2 to 6
move 1 from 7 to 8
move 13 from 1 to 8
move 9 from 8 to 4
move 3 from 8 to 6
move 10 from 6 to 8
move 6 from 6 to 4
move 4 from 8 to 5
move 26 from 4 to 2
move 2 from 5 to 2
move 5 from 8 to 1
move 1 from 8 to 3
move 2 from 1 to 3
move 2 from 3 to 7
move 27 from 2 to 7
move 2 from 8 to 1
move 1 from 3 to 7
move 6 from 6 to 2
move 4 from 6 to 1
move 4 from 6 to 4
move 2 from 5 to 4
move 4 from 2 to 1
move 3 from 1 to 8
move 1 from 2 to 8
move 8 from 4 to 3
move 1 from 2 to 8
move 5 from 8 to 6
move 1 from 4 to 2
move 1 from 2 to 1
move 6 from 3 to 1
move 13 from 7 to 1
move 1 from 2 to 8
move 1 from 8 to 2
move 1 from 6 to 2
move 1 from 2 to 8
move 1 from 8 to 2
move 14 from 7 to 1
move 5 from 6 to 3
move 2 from 3 to 1
move 3 from 3 to 2
move 3 from 7 to 4
move 1 from 4 to 9
move 1 from 9 to 7
move 2 from 3 to 6
move 5 from 2 to 7
move 1 from 7 to 6
move 5 from 7 to 6
move 2 from 6 to 7
move 1 from 6 to 8
move 1 from 4 to 7
move 4 from 6 to 9
move 35 from 1 to 8
move 3 from 7 to 2
move 1 from 2 to 5
move 24 from 8 to 3
move 1 from 5 to 8
move 13 from 3 to 6
move 2 from 2 to 6
move 6 from 6 to 4
move 11 from 1 to 6
move 12 from 6 to 1
move 1 from 8 to 1
move 2 from 1 to 3
move 5 from 4 to 1
move 1 from 6 to 4
move 1 from 8 to 3
move 13 from 3 to 9
move 3 from 8 to 2
move 3 from 2 to 7
move 1 from 3 to 6
move 3 from 7 to 8
move 14 from 1 to 3
move 1 from 1 to 9
move 6 from 3 to 8
move 17 from 8 to 6
move 1 from 3 to 7
move 1 from 7 to 8
move 26 from 6 to 7
move 1 from 1 to 9
move 3 from 4 to 1
move 2 from 3 to 8
move 1 from 8 to 4
move 14 from 9 to 7
move 12 from 7 to 3
move 2 from 1 to 4
move 2 from 7 to 8
move 2 from 8 to 3
move 4 from 9 to 8
move 1 from 4 to 7
move 1 from 1 to 3
move 2 from 4 to 2
move 24 from 7 to 6
move 1 from 8 to 1
move 1 from 7 to 2
move 1 from 7 to 9
move 3 from 2 to 9
move 1 from 1 to 6
move 5 from 8 to 2
move 5 from 3 to 4
move 1 from 2 to 5
move 3 from 9 to 8
move 2 from 4 to 9
move 16 from 6 to 3
move 14 from 3 to 8
move 1 from 7 to 9
move 8 from 6 to 9
move 4 from 8 to 5
move 8 from 8 to 3
move 1 from 5 to 8
move 1 from 2 to 4
move 4 from 8 to 7
move 1 from 5 to 6
move 12 from 9 to 5
move 15 from 5 to 8
move 1 from 6 to 1
move 2 from 2 to 6
move 3 from 4 to 2
move 4 from 2 to 7
move 8 from 7 to 3
move 1 from 1 to 4
move 3 from 6 to 9
move 16 from 8 to 3
move 3 from 9 to 4
move 1 from 8 to 9
move 2 from 9 to 4
move 24 from 3 to 8
move 19 from 8 to 7
move 2 from 8 to 7
move 7 from 4 to 5
move 13 from 7 to 5
move 4 from 7 to 8
move 7 from 8 to 1
move 3 from 5 to 3
move 3 from 7 to 2
move 1 from 1 to 4
move 1 from 7 to 2
move 3 from 2 to 4
move 8 from 3 to 1
move 11 from 1 to 3
move 12 from 3 to 4
move 1 from 2 to 5
move 18 from 3 to 8
move 3 from 1 to 9
move 1 from 3 to 5
move 15 from 5 to 4
move 4 from 5 to 1
move 23 from 4 to 6
move 3 from 1 to 6
move 13 from 8 to 3
move 25 from 6 to 2
move 1 from 9 to 5
move 5 from 3 to 8
move 17 from 2 to 8
move 4 from 4 to 1
move 1 from 9 to 7
move 5 from 2 to 6
move 2 from 2 to 4
move 1 from 9 to 4
move 6 from 3 to 9
move 16 from 8 to 3
move 2 from 1 to 8
move 1 from 7 to 4
move 5 from 4 to 7
move 1 from 5 to 3
move 2 from 7 to 1
move 9 from 8 to 4
move 3 from 7 to 2
move 2 from 8 to 3
move 10 from 4 to 1
move 1 from 2 to 3
move 5 from 3 to 7
move 2 from 8 to 9
move 2 from 9 to 8
move 1 from 2 to 1
move 3 from 9 to 6
move 2 from 2 to 8
move 4 from 7 to 3
move 4 from 8 to 6
move 1 from 7 to 1
move 1 from 4 to 8
move 4 from 3 to 4
move 4 from 4 to 2
move 6 from 1 to 2
move 1 from 4 to 3
move 5 from 3 to 8
move 6 from 3 to 8
move 2 from 2 to 8
move 3 from 2 to 9
move 8 from 1 to 6
move 3 from 2 to 7
move 2 from 7 to 2
move 13 from 6 to 5
move 7 from 5 to 9
move 3 from 2 to 7
move 1 from 2 to 9
move 2 from 5 to 2
move 3 from 8 to 5
move 5 from 3 to 4
move 2 from 2 to 1
move 9 from 8 to 7
move 1 from 1 to 8
move 6 from 5 to 2
move 4 from 2 to 8
move 4 from 7 to 1
move 1 from 2 to 6
move 5 from 1 to 6
move 1 from 8 to 2
move 1 from 2 to 9
move 13 from 6 to 5
move 2 from 7 to 2
move 1 from 8 to 7
move 4 from 4 to 7
move 1 from 4 to 1
move 4 from 8 to 4
move 6 from 5 to 9
move 2 from 1 to 4
move 1 from 8 to 6
move 11 from 9 to 5
move 1 from 7 to 8
move 1 from 8 to 1
move 1 from 1 to 3
move 6 from 4 to 8
move 1 from 8 to 4
move 1 from 1 to 6
move 6 from 9 to 7
move 1 from 4 to 5
move 3 from 2 to 1
move 1 from 8 to 2
move 1 from 3 to 2
move 20 from 5 to 6
move 3 from 1 to 6
move 2 from 2 to 9
move 3 from 8 to 3
move 5 from 3 to 8
move 1 from 1 to 6
move 2 from 8 to 9
move 7 from 9 to 5
move 3 from 5 to 4
move 3 from 8 to 3
move 9 from 7 to 9
move 1 from 8 to 5
move 7 from 7 to 9
move 2 from 5 to 2
move 9 from 9 to 2
move 1 from 7 to 3
move 2 from 9 to 1
move 2 from 5 to 9
move 2 from 1 to 4
move 2 from 3 to 7
move 18 from 6 to 7
move 7 from 9 to 1
move 7 from 6 to 8
move 4 from 4 to 9
move 4 from 8 to 3
move 2 from 8 to 2
move 1 from 8 to 5
move 1 from 4 to 7
move 1 from 5 to 1
move 2 from 9 to 3
move 12 from 2 to 5
move 6 from 5 to 6
move 5 from 7 to 2
move 3 from 6 to 4
move 1 from 4 to 7
move 1 from 4 to 1
move 2 from 5 to 8
move 1 from 8 to 2
move 2 from 9 to 7
move 8 from 1 to 8
move 11 from 7 to 1
move 5 from 8 to 2
move 7 from 7 to 5
move 1 from 9 to 4
move 1 from 7 to 5
move 7 from 5 to 7
move 2 from 6 to 1
move 1 from 8 to 2
move 12 from 1 to 7
move 2 from 1 to 2
move 3 from 8 to 5
move 3 from 5 to 2
move 8 from 7 to 3
move 1 from 3 to 1
move 3 from 6 to 4
move 4 from 5 to 6
move 14 from 2 to 9
move 3 from 6 to 9
move 3 from 4 to 2
move 1 from 1 to 7
move 1 from 7 to 1
move 3 from 3 to 5
move 8 from 7 to 4
move 1 from 5 to 9
move 3 from 2 to 4
move 1 from 3 to 4
move 4 from 2 to 6
move 2 from 6 to 7
move 3 from 5 to 4
move 16 from 4 to 1
move 7 from 9 to 8
move 1 from 5 to 1
move 3 from 7 to 9
move 3 from 9 to 4
move 7 from 1 to 7
move 6 from 7 to 1
move 5 from 3 to 1
move 11 from 9 to 2
move 3 from 4 to 6
move 9 from 2 to 8
move 6 from 3 to 5
move 2 from 8 to 6
move 5 from 5 to 3
move 2 from 7 to 1
move 3 from 3 to 9
move 1 from 2 to 4
move 1 from 5 to 1
move 13 from 1 to 2
move 5 from 8 to 6
move 2 from 3 to 9
move 2 from 4 to 7
move 5 from 6 to 9
move 7 from 9 to 1
move 3 from 7 to 2
move 6 from 8 to 6
move 5 from 6 to 2
move 2 from 8 to 3
move 2 from 9 to 4
move 6 from 2 to 5
move 1 from 3 to 7`;

stacks(input)