// @ts-check

/** @typedef {{ items: number[], operation: Operation, test: Test, trueMonkey: number, falseMonkey: number, inspectedCount: number }} Monkey */
/** @typedef {{ left: number | "old", right: number | "old", operation: "*" | "+" }} Operation */
/** @typedef {{ divideBy: number }} Test */


const regex = /Monkey (?<monkey>[0-9]+):\nStarting items: (?<items>[0-9, ]+)\nOperation: new = (?<left>[a-z0-9]+) (?<operation>[+*]) (?<right>[a-z0-9]+)\nTest: divisible by (?<divisible>[0-9]+)\n  If true: throw to monkey (?<trueMonkey>[0-9]+)\n  If false: throw to monkey (?<falseMonkey>[0-9]+)/gm;

function monkeys(str) {
    /** @type {Monkey[]} */
    const monkeys = [];
    let reg;
    while ((reg = regex.exec(str)) != null) {
        const itemsString = reg.groups?.items;
        const items = itemsString?.split(", ").map(a => parseInt(a)) ?? [];
        monkeys.push({
            items,
            // @ts-ignore
            operation: { left: numOrOld(reg.groups?.left), right: numOrOld(reg.groups?.right), operation: reg.groups?.operation },
            // @ts-ignore
            test: { divideBy: parseInt(reg.groups?.divisible)  },
            // @ts-ignore
            trueMonkey: parseInt(reg.groups?.trueMonkey),
            // @ts-ignore
            falseMonkey: parseInt(reg.groups?.falseMonkey),
            inspectedCount: 0
        });
    }
    console.log(monkeys);
    const supermodulo = monkeys.reduce((prev, cur) => prev * cur.test.divideBy, 1);

    const numberOfRounds = 10000; //20;
    const partTwo = true;

    for (let round = 1; round <= numberOfRounds; round++) {
        for (let i = 0; i < monkeys.length; i++) {
            singleMonkeyRound(i);
        }
    }

    console.log(monkeys);

    const highest = monkeys.map(a => a.inspectedCount).sort((a, b) => b-a);
    const monkeyBusiness = highest[0] * highest[1];
    console.log(monkeyBusiness);

    /** 
     * @param {string | undefined} str
     * @returns {number | "old"}
     */
    function numOrOld(str) {
        if (str == "old") return str;
        if (str == null) return 0;
        return parseInt(str);
    }


    /** @param {number} index */
    function singleMonkeyRound(index) {
        const monkey = monkeys[index];
        for (let i = 0; i < monkey.items.length; i++) {
            let item = monkey.items[i];
            // inspect
            monkey.inspectedCount++;
            item = operateOnItem(item, monkey.operation);
            // worry level division
            if (!partTwo) item = Math.floor(item / 3);
            else item = item % supermodulo;
            // throw
            if (test(monkey.test, item)) {
                monkeys[monkey.trueMonkey].items.push(item);
            } else {
                monkeys[monkey.falseMonkey].items.push(item);
            }
        }
        monkey.items = [];
    }

    /** 
     * @param {number} item
     * @param {Operation} op
     * @returns {number}
     */
    function operateOnItem(item, op) {
        const l = op.left == "old" ? item : op.left
        const r = op.right == "old" ? item : op.right
        if (op.operation == "*") {
            return l * r;
        } else if (op.operation == "+") {
            return l + r;
        } else {
            return 0;
        }
    }

    /**
     * @param {Test} test
     * @param {number} num
     * @returns {boolean}
     */
    function test(test, num) {
        if (num % test.divideBy == 0) {
            return true;
        } else return false;
    }
}

const input = `Monkey 0:
Starting items: 74, 64, 74, 63, 53
Operation: new = old * 7
Test: divisible by 5
  If true: throw to monkey 1
  If false: throw to monkey 6

Monkey 1:
Starting items: 69, 99, 95, 62
Operation: new = old * old
Test: divisible by 17
  If true: throw to monkey 2
  If false: throw to monkey 5

Monkey 2:
Starting items: 59, 81
Operation: new = old + 8
Test: divisible by 7
  If true: throw to monkey 4
  If false: throw to monkey 3

Monkey 3:
Starting items: 50, 67, 63, 57, 63, 83, 97
Operation: new = old + 4
Test: divisible by 13
  If true: throw to monkey 0
  If false: throw to monkey 7

Monkey 4:
Starting items: 61, 94, 85, 52, 81, 90, 94, 70
Operation: new = old + 3
Test: divisible by 19
  If true: throw to monkey 7
  If false: throw to monkey 3

Monkey 5:
Starting items: 69
Operation: new = old + 5
Test: divisible by 3
  If true: throw to monkey 4
  If false: throw to monkey 2

Monkey 6:
Starting items: 54, 55, 58
Operation: new = old + 7
Test: divisible by 11
  If true: throw to monkey 1
  If false: throw to monkey 5

Monkey 7:
Starting items: 79, 51, 83, 88, 93, 76
Operation: new = old * 3
Test: divisible by 2
  If true: throw to monkey 0
  If false: throw to monkey 6`;

monkeys(input);