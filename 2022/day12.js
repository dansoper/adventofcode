// @ts-check

/** @typedef {{ options: Coord[] }} Options */


function pathways(str) {
    /** @type {number[][]} */
    const grid = [];
    /** @type {Options[][]} */
    let gridOptions = [];
    /** @type {Coord?} */
    let aimingPoint = null;
    /** @type {Coord?} */
    let startingPoint = null;
    /** @type {Coord[]} */
    let visited = [];
    // If we've previously visited a path, then this path will always be longer than that one, so we abandon

    makeGrid(str);
    console.log(grid);
    findOptions();
    //console.log(gridOptions[0][2]);
    let isAimingPoint = a => aimingPoint != null && a.x == aimingPoint.x && a.y == aimingPoint.y
    walkPaths();

    // Part 2
    gridOptions = [];
    findOptionsPart2();
    visited = [];
    startingPoint = aimingPoint;
    isAimingPoint = a => grid[a.y][a.x] == 0;
    walkPaths();

    function walkPaths() {
        if (startingPoint == null || aimingPoint == null) return;
        let i = 0;
        /** @type {Coord[][]} */
        let paths = [[startingPoint]]
        let found = false;
        while (found == false) {
            i++;
            const newPaths = [];
            for (let p = 0; p < paths.length; p++) {
                const theseNewPaths = getPathsFromCoord(paths[p]);
                const arrivals = theseNewPaths.map(a => a[a.length-1]);
                if (arrivals.find(isAimingPoint) != null) {
                    found = true;
                    console.log(i); break;
                }
                newPaths.push(...theseNewPaths);
            }
            paths = newPaths;
            //if (i > 20) break;
            //console.log(i, paths.length);
        }
        //console.log(paths);
    }

    /**
     *  @param {Coord[]} coordList
     * @returns {Coord[][]} */
    function getPathsFromCoord(coordList) {
        const newPaths = [];
        const coord = coordList[coordList.length-1];
        const options = gridOptions[coord.y][coord.x];
        options.options.forEach(option => {
            if (coordList.find(a => a.x == option.x && a.y == option.y) == null && visited.find(a => a.x == option.x && a.y == option.y) == null) {
                visited.push(option);
                const newList = cloneCoordList(coordList);
                newList.push(option);
                newPaths.push(newList);
            }
        });
        return newPaths;
    }

    /**
     *  @param {Coord[]} coords
     * @returns {Coord[]} */
    function cloneCoordList(coords) {
        return [...coords.map(a => { return { ...a } })];
    }

    /** @param {string} str */
    function makeGrid(str) {
        const lines = str.split("\n");
        lines.forEach((line, y) => {
            const chars = line.split("");
            chars.forEach((char, x) => {
                if (char == "S") {
                    startingPoint = { x, y };
                    setGridItem(x, y, 0);
                } else if (char == "E") {
                    aimingPoint = { x, y };
                    setGridItem(x, y, 25);
                } else {
                    const num = char.charCodeAt(0) - 97;
                    setGridItem(x, y, num);
                }
            });
        })
    }

    /** @param {number} x */
    /** @param {number} y */
    /** @param {number} num */
    function setGridItem(x, y, num) {
        if (grid[y] == null) grid[y] = [];
        grid[y][x] = num;
    }

    /** @param {number} x */
    /** @param {number} y */
    /** @returns {number?} */
    function getGridItem(x, y) {
        if (y < 0 || x < 0 || y >= grid.length || x >= grid[0].length) {
            return null;
        }
        return grid[y][x];
    }

    /** @param {number} x */
    /** @param {number} y */
    /** @param {Options} opt */
    function setGridOption(x, y, opt) {
        if (gridOptions[y] == null) gridOptions[y] = [];
        gridOptions[y][x] = opt;
    }

    function findOptions() {
        /** @type {Coord[]} */
        const coordOptions = [ { x: -1, y: 0 }, { x: 1, y: 0 }, { x: 0, y: -1 }, { x: 0, y: 1 } ];

        for (let y = 0; y < grid.length; y++) {
            for (let x = 0; x < grid[0].length; x++) {
                const item = grid[y][x];
                const options = [];
                coordOptions.forEach(opt => {
                    const newCoord = { x: x + opt.x, y: y + opt.y };
                    const compareItem = getGridItem(newCoord.x, newCoord.y);
                    if (compareItem != null && compareItem <= item + 1) {
                        options.push(newCoord);
                    }
                });
                setGridOption(x, y, { options });
            }
        }
    }

    function findOptionsPart2() {
        /** @type {Coord[]} */
        const coordOptions = [ { x: -1, y: 0 }, { x: 1, y: 0 }, { x: 0, y: -1 }, { x: 0, y: 1 } ];

        for (let y = 0; y < grid.length; y++) {
            for (let x = 0; x < grid[0].length; x++) {
                const item = grid[y][x];
                const options = [];
                coordOptions.forEach(opt => {
                    const newCoord = { x: x + opt.x, y: y + opt.y };
                    const compareItem = getGridItem(newCoord.x, newCoord.y);
                    if (compareItem != null && compareItem >= item - 1) {
                        options.push(newCoord);
                    }
                });
                setGridOption(x, y, { options });
            }
        }
    }

}

const input = `abccccccccaaaaccccaaacaccccaaaaaacccccccccccaaaccccccccccaaaaaaacccccccccccccccccccccccccccccacaaaccccccccccccccccccccccccccccccccccccccccaaaaa
abccccccccaaaaccaaaaaaaacccaaaaaacccccccccccaaaacccccccccaaaaaaaaaacccccccccccccccaaccccccccaaaaaccaaaccaacccccccccccccccccccccccccccccccaaaaaa
abcccccccccaacccaaaaaaaaccccaaaaacccccccccccaaaacccccccaaaaaaaaaaaaaccccccccccaaaaaaccccccccaaaaaaaaaaaaaaccccccccccccccccaaaccccccccccccaaaaaa
abcccccccccccccccaaaaaccccccaacaaccccaacccccaaacccccccaaaaaaaaaaaaaaccccccccccaaaaaaacccccccccaaaaacaaaaaaccccccccccccccccaaccccccccccccccccaaa
abccccccccccccccccaaaaaccccccccccaaccaacccccccccccccccaaaaacaaaacacacccccaacccaaaaaaaacccccccaaaaacaaaaaaaccccccccccccccccaaacccccccccccccccaaa
abcccccccccccccccaaaaaaccccccccccaaaaaaccccccccccccccccaaaaaaaacaaaaacaaaaaccccaaaaaaacccccccaacaacaaaaaaaaccccccccaaaaccaakcccccccccccccccccaa
abcccccccccccccccaaaccacccccccccccaaaaaaacccccccaaaccccccaaaaaacaaaaaccaaaaaccaaaaaaccccccccccccaacaaaaaaaacccccccccaaaakkkklllcccccccccccccccc
abcccccaaaccccccccccccccccccccccccaaaaaaacccccccaaacacccaaaaaaaaaaaaacaaaaaaccaaaaaacccccccccccccccccaaaccccccccccccaaakkkkkllllcccccccaacccccc
abccccaaaacccccccccccccccccccccccaaaaaacccccccaccaaaaaccaaaaaaaaaaaaacaaaaccccccccaaccccccccccccccccccaaccccccccccccckkkkkkpllllccccaaaaaaccccc
abccccaaaacccccccccccccccccaaacccaaaaaacccccccaaaaaaaacccaaaaacaaaaaacccaaaccccccccccccccccccccccccccccccccccccccccckkkkpppppplllcccaaaaacccccc
abcccccaaaccccccccccccccccaaaacccccccaaccccccccaaaaacccccaaaccccaaacccccccccccccccccccccccccaaccccccccccccccccccjjjkkkkpppppppplllcccaaaaaacccc
abccccccccccccccccccccccccaaaaccccccccccccccccccaaaaacccccccccccccccccccccccccccccccccccccccaaaaaccccccccccccjjjjjjkkkrppppuppplllccccaaaaacccc
abccccccccccccccaaaccccccccaaaccccccccccccccccccaacaaccccccccccccccccccccccaaaccccccccaacccaaaaaccccccccccccjjjjjjjjrrrpuuuuuppplllcccccaaacccc
abcccccaaccaacccaaacacccccccccccccccccccccccccacaaaaccccccccccccccccccccccaaaaaaccccaaaacccaaaaaaccaccccccccjjjrrrrrrrrruuuuuppplllmcccddcccccc
abcccccaaaaaacaaaaaaaaccccccccccccccccccccccccaacaaaccccccccccccccccccccccaaaaaaccccaaaaaacccaaaaccaaacaaacjjjrrrrrrrrruuuxuuupqqlmmmmddddccccc
abcccccaaaaaccaaaaaaaaccccccccccccccccccccccccaaaaaccccaacccccccccccccccccaaaaaacccccaaaacccaacccccaaaaaaacjjjrrrrtuuuuuuxxyuvqqqqmmmmmddddcccc
abaacccaaaaaaccaaaaaccccccccccccccccccaaaaccccaaaaaaccaaaccccccccccccccccccaaaaaccccaaaaaccccccccccaaaaaaccjjjrrrtttuuuuuxxyvvvqqqqqmmmmdddcccc
abaaccaaaaaaaaccaaaaaccccccccccccccccaaaaaaaaaaaaaaaacaaacaaaccccccccccccccaacaacaacaacaaccccccccaaaaaaaaccijjqqrtttxxxxxxyyvvvvvqqqqmmmmdddccc
abaaccaaaaaaaacaaaaaaccccccccccccccccaaaaaaaaaaaaaaaaaaaaaaaacccccccccccccccaaacaaaccccccccccaaccaaaaaaaaaciiiqqqttxxxxxxxyyvvvvvvvqqqmmmdddccc
abaaaccccaaccccaaaccacccccccccccccccccaaaaaaccccaacaaaaaaaaaaccaaaccccccccccaaaaaaaccccccccccaaaaaaaaaaaaaaiiiqqqtttxxxxxxyyyyyyvvvqqqmmmdddccc
SbaaaccccaacccccccccccccccccccccccccaaaaaaaaccccaacccaaaaaaccaaaaaaccccccccccaaaaaacccccccccccaaaaacaaacaaaaiiiqqqttxxxxEzzyyyyyvvvqqqmmmdddccc
abaaaccccccccccccccccccccccccccccccaaaaaaaaaaccccccccaaaaaaccaaaaaaccccccccccaaaaaaaaccccccccaaaaaacaaaccaaaiiiqqqtttxxxyyyyyyvvvvqqqmmmdddcccc
abaccccccaacccccccccccccccccccccccccaaaaaaaaaaaacccccaaaaaaacaaaaaacccccccccaaaaaaaaacccccccaaaaaaaacaaaaaaaiiiqqqtttxxyyyyyyvvvvqqqqnnmeddcccc
abccccccaaaaccccccccccccaaaccccccccccccaaaaaaaaaaacccaaacaaacaaaaacccccccccaaaaaaaaaacccccccaaaaaaaaccaaaaaaaiiiqqtttxxyyyyyywvrrrrnnnneeeccccc
abccccccaaaacccccaacccccaaaacccccccccccaaaccaaaaaacccacccccccaaaaacccccccccaaacaaacccccccccccccaacccccccaaaaaiiqqqttxxwywwyyywwrrrnnnneeeeccccc
abccccccaaaaccaacaaaccccaaaaccccccaacccaacccaaaaaccccccccccccccccccccccccccccccaaacccccccccccccaaccccccaaaaaaiiqqqttwwwwwwwwywwrrrnnneeeecccccc
abccccccccccccaaaaacccccaaaccccacaaacccccccccaaaaacccccccccccccccccccccccccccccaaacccccccccccccccccccccaaaaaaiiqqpsswwwwsswwwwwrrnnneeeeccccccc
abcccccccccccccaaaaaacccccccccaaaaacaacccccccaacaacccccaaccccccccccccccccccccccccccccccccccccccccccccccaccaahhhpppssssssssswwwwrrnnneeeaccccccc
abcccccccccccaaaaaaaacccccccccaaaaaaaacccccaaccccccaaacaaccccccccccccccccccccccccccccccccccccaaaccaccccccccchhhpppsssssssssswwrrrnnneeeaaaacccc
abcccccccccccaaaaacaacccccccccccaaaaaccaaaaaaccccccaaaaaaccccccccccccccccccccccccccccaaccaaccaaaaaacccccccccchhpppppsspppossrrrrrnnneeeaaaacccc
abccccccccccccacaaaccccccccccccaaaaacccaaaaaaaacccccaaaaaaaccaaaccccccccaaaacccccccccaaaaaacccaaaaacccccccccchhhpppppppppoosrrrroonffeaaaaacccc
abccccccccccccccaaaccccccccccccaacaaaccaaaaaaaaccccaaaaaaaaacaaaccccccccaaaacccccccccaaaaaccaaaaaaacccccccccchhhhpppppppoooooooooonfffaaaaccccc
abcccccccccccccccccccccccaaaccccccaaccccaaaaaaaccccaaaaaaaaaaaaaaaccccccaaaacccccccccaaaaaacaaaaaaaacccccaacchhhhhhhhgggggoooooooofffaaaaaacccc
abcccccccccccccccccccccccaaaaacccaacccccaaaaaccccccaaaaaacaaaaaaaaccccccaaacccccccccaaaaaaaaaaaaaaaaccccaaacccchhhhhgggggggooooooffffaaaaaacccc
abccaaacccccccccccccccccaaaaaaccaaaccccaaaaaacccccccccaaacccaaaaacccccccccccccccccccaaaaaaaaccaaacacccccaaacaaachhhggggggggggfffffffcaacccccccc
abcaaaaccccccccccaacccccaaaaaaccaaacaaaccccaaccccccccccccccaaaaacccccccccccccccccccccccaacccccaaaccccaaaaaaaaaacccccccccaagggffffffcccccccccccc
abcaaaaccccccccccaaccccccaaaaaaaaaaaaaaccccccccccccccccccccaaaaaaccccccccccccccccccccccaaccccccccccccaaaaaaaaaccccccccccaaacgfffffcccccccccccaa
abccaaacccccccaaaaaaaacccaaaaaaaaaaaaaaccccccccaaaaaccaaaaaaaaaaaaaacaacccccccccaaaccacccccccccccccccccaaaaacccccccccccaaaaccccccccccccccccccaa
abccccccccccccaaaaaaaacccccccccaaaaaaccccccccccaaaaaccaaaaaaaaaaaaaacaaaaaacccccaaaaaacccccccccccccccccaaaaaacccccccccccaacccccccccccccccacacaa
abccccccccccccccaaaacccccccccccaaaaaacccccccccaaaaaacccaaaaaaaaaaaaacaaaaaacccccaaaaaacccccccccccccccccaaaaaaaccccccccccaacccccccccccccccaaaaaa
abcccccccccccccaaaaaaccccccccccaaaaaaaccccccccaaaaaaccccccaaaaaacccaaaaaaaaccccaaaaaaaaccccccccccccccccaaacaaacccccccccccccccccccccccccccaaaaaa`;

pathways(input);