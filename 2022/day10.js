// @ts-check

function ops(str) {
    const operations = str.split("\n");

    let val = 1;
    //let toDo = [];
    let count = 0;
    let partOneSum = 0;
    const vals = [];
    for (let i = 0; i < operations.length; i++) {
        //console.log(i, val);
        
        const operation = operations[i];
        let change = 0;
        if (operation.startsWith("addx")) {
            const num = parseInt(operation.split(" ")[1]);
            change = num;
            inc();
            vals.push(val);
        }
        inc();
        vals.push(val);
        val += change;
    }
    console.log(val, count);

    console.log(partOneSum);

    function inc() {
        if ((count + 1) % 20 == 0 && (count + 1 == 20 || (count + 1 - 20) % 40 == 0)) {
            console.log(count, val * (count + 1));
            partOneSum += val * (count + 1);
        }
        count++;
    }

    function draw() {
        let toDraw = "";
        let col = 0;
        for (let i = 0; i < vals.length; i++) {
            if (col == 40) col = 0;
            const val = vals[i];
            if (val > col - 2 && val < col + 2) {
                toDraw += "#";
            } else {
                toDraw += ".";
            }
            if ((i + 1) % 40 == 0) {
                toDraw += "\n";
            }
            col++;

        }
        console.log(toDraw);
        //console.log(vals);
    }
    draw();
}

const input = `addx 2
addx 3
noop
addx 1
addx 27
addx -23
addx 5
noop
addx 1
noop
addx 4
addx 1
noop
addx 4
addx 5
noop
noop
noop
addx 5
addx -4
addx 4
noop
addx 1
addx -38
noop
noop
addx 7
addx 8
addx -3
noop
addx 3
noop
addx 5
noop
noop
addx -2
addx 2
addx 9
addx -2
addx 6
addx 1
addx -4
addx 5
addx 2
addx -14
addx -6
addx -16
addx 1
addx 5
addx 1
addx 4
addx -2
noop
addx -7
addx -3
addx 17
addx 5
noop
noop
addx 19
addx -16
noop
addx 14
addx -8
addx 2
noop
addx 4
noop
addx -35
addx -2
noop
noop
addx 7
addx 19
addx -26
addx 10
addx 29
addx -21
noop
addx 4
noop
noop
addx -9
addx 4
addx 8
addx 7
noop
addx -2
addx 5
addx 2
addx -19
addx -18
noop
noop
noop
noop
addx 7
addx -7
addx 37
addx -27
addx 5
addx 2
addx -12
addx 4
addx 11
noop
noop
noop
addx 5
addx -14
addx 21
addx -4
addx 5
addx 2
noop
addx -35
noop
noop
noop
noop
addx 7
addx 1
noop
noop
addx 5
addx -1
addx 5
addx 1
noop
addx 4
addx 1
noop
noop
addx 4
noop
addx 1
addx 2
addx 5
addx 2
addx 1
noop
noop
noop
noop`;

ops(input);