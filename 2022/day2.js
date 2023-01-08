// @ts-check
/** @typedef {"rock"|"paper"|"scissors"} RPS */
/** @typedef {"win"|"lose"|"draw"} WLD */
/** @typedef {{ opponent: RPS, player: RPS }} Play */
/** @typedef {{ opponent: RPS, result: WLD }} PlayWithScore */
        
/**
* @param {string} str
* @returns {RPS}
*/
function parseOpponent(str) {
    if (str == "A") return "rock";
    else if (str == "B") return "paper";
    else if (str == "C") return "scissors";
    else throw "opponent" + str;

}
/**
* @param {string} str
* @returns {RPS}
*/
function parsePlayer(str) {
    if (str == "X") return "rock";
    else if (str == "Y") return "paper";
    else if (str == "Z") return "scissors";
    else throw "player" + str;
}
/**
* @param {string} str
* @returns {WLD}
*/
function parseResult(str) {
    if (str == "X") return "lose";
    else if (str == "Y") return "draw";
    else if (str == "Z") return "win";
    else throw "player" + str;
}
/**
* @param {string} inp
* @returns {Play}
*/
function parsePlay(inp) {
    const spl = inp.split(" ");
    return { opponent: parseOpponent(spl[0]), player: parsePlayer(spl[1]) };
}
/**
* @param {string} str
* @returns {Play[]}
*/
function parsePlays(str) {
    const lines = str.split("\n");
    return lines.map(a => parsePlay(a));
}

/**
* @param {string} inp
* @returns {PlayWithScore}
*/
function parsePlayWithScore(inp) {
    const spl = inp.split(" ");
    return { opponent: parseOpponent(spl[0]), result: parseResult(spl[1]) };
}

/**
* @param {string} str
* @returns {PlayWithScore[]}
*/
function parsePlayWithScores(str) {
    const lines = str.split("\n");
    return lines.map(a => parsePlayWithScore(a));
}

/**
* @param {Play} play
* @returns {number}
*/
function scorePlay(play) {
    const scoreForPlayer = play.player == "rock" ? 1 : (play.player == "paper" ? 2 : 3);
    let scoreForPlay = 0;
    if (play.player == "rock") {
        if (play.opponent == "rock") scoreForPlay = 3;
        if (play.opponent == "paper") scoreForPlay = 0;
        if (play.opponent == "scissors") scoreForPlay = 6;
    }
    if (play.player == "paper") {
        if (play.opponent == "rock") scoreForPlay = 6;
        if (play.opponent == "paper") scoreForPlay = 3;
        if (play.opponent == "scissors") scoreForPlay = 0;
    }
    if (play.player == "scissors") {
        if (play.opponent == "rock") scoreForPlay = 0;
        if (play.opponent == "paper") scoreForPlay = 6;
        if (play.opponent == "scissors") scoreForPlay = 3;
    }
    return scoreForPlayer + scoreForPlay;
}
/**
* @param {PlayWithScore} play
* @returns {RPS}
*/
function playForScore(play) {
    if (play.result == "win") {
        if (play.opponent == "rock") return "paper";
        if (play.opponent == "paper") return "scissors";
        if (play.opponent == "scissors") return "rock";
    }
    if (play.result == "draw") {
        return play.opponent;
    }
    if (play.result == "lose") {
        if (play.opponent == "rock") return "scissors";
        if (play.opponent == "paper") return "rock";
        if (play.opponent == "scissors") return "paper";
    }
    throw "AAAA score";
}
/**
* @param {PlayWithScore} play
* @returns {Play}
*/
function playFromPlayToScore(play) {
    return { opponent: play.opponent, player: playForScore(play) };
}


const input = `A X
B Z
C Z
B Z
B Z
A Y
A Y
A Z
A Y
B Y
A Y
B Y
B Z
A Y
C X
B X
A Y
B Z
B X
A Y
A Y
A Z
B Z
B Z
C Y
C Y
B Z
A Y
A Y
B Z
B X
B Z
A Z
A X
C X
A Y
B Z
A Y
B Z
B Z
B Z
B Z
A Y
A Y
C Y
A Y
A Y
A Y
B Z
A Y
A Z
B X
B Z
A Y
A Y
A Y
B Z
C X
B Z
A Y
B Z
A Y
A Y
A Y
A Z
A X
C X
A Y
C Z
C X
C X
B Z
A Z
A Z
B Z
A Z
A X
C X
A X
C X
A Y
B Z
A Z
B Z
A Y
B Z
C X
A Y
B Z
A Z
B Z
A Y
A Z
B Z
A X
C X
A X
A Y
B Z
B Z
A Y
B Z
B Z
C Z
A Y
A Y
C Y
A Z
B Z
A Y
C X
A Y
B Z
A Y
C X
A Y
B Z
A Y
C X
C X
B Z
B Z
B Z
A Y
A X
C X
A Y
A Y
A Y
A Y
A Y
A Y
B Z
A Z
A Y
A Z
B Z
B Z
B Z
A Y
B Z
A X
A Y
A Z
A Z
A Y
A Y
B Z
A Y
C X
B Z
A Y
B Z
C X
A Y
A Y
B Z
C X
A Y
A Y
A X
A Z
A Y
A Z
A Y
A Y
B Z
B Z
A Y
C X
B Z
B Y
A Y
C Y
A Y
A Z
B Y
B X
A X
B Z
B Z
A Y
A X
A Y
A Y
A Y
A Z
A Y
A X
B Z
A Z
A Z
A Y
C Z
A Y
B Z
C X
A Z
C X
B Z
A Y
C Z
A Z
B Z
A Y
A Y
B Z
A Y
C X
C X
A Z
A Z
A Y
A Y
A Y
A Y
C X
C X
A Z
A Z
C X
B Z
C Y
A Y
A Y
A Y
A Z
B X
C Z
A Y
B Z
A Z
A X
A Y
A Y
B Z
C Y
A Y
A Z
A Y
C X
C X
A Z
A Y
B Z
A Y
A Z
C X
A Z
A Y
A Y
A Z
B Z
A Y
A Y
C X
A Y
C X
B Z
A X
B Z
B Z
A Y
A Y
A Y
A Y
A Y
A Y
B Z
A Y
A Y
A Z
B Z
A Y
A Y
A Z
B Z
A Y
C X
A Y
B Y
B Z
A Y
B Z
A Y
A Y
A X
A Z
A X
B Z
B Z
A Y
A Y
C X
A Z
A X
A Y
A Y
A Y
C X
A Y
A Z
B Z
A X
B Z
A Y
A Z
C X
B X
A Y
C X
A Y
A Y
A Y
A Y
B Z
C X
B X
A Y
B Z
A Z
C X
B Z
B Z
C X
A Z
C X
B Z
C Y
B Z
B Y
B Z
B Z
B Z
C Z
C X
A Y
C X
B Z
A Y
A X
B Z
C X
A Y
A Y
A Z
B Z
B Z
C X
B Z
A X
B Z
A Y
A Y
B Z
A Y
A Y
A X
A Z
B Z
B Z
A Z
A Z
A Y
B X
A Y
C X
A X
A Y
A X
A Y
A X
C X
B Z
A Y
B Z
A Y
B Z
B X
B Z
A Y
B Z
B Z
C X
A Z
A Y
B Z
B Z
C Y
A Y
B X
B Z
B Z
A Y
B Z
C X
A Y
A Y
B Z
B Z
B Z
C X
A Y
A Z
C X
A Z
C X
B Z
A Y
A Y
B Z
A Y
B Y
B Y
A Z
A Y
B Z
A Y
C X
A Z
A Y
A Z
A X
A Y
A Y
A Z
C X
B Z
A Y
A Y
A Y
C X
B Z
C Y
A Y
B X
B Z
B Y
B Z
A Y
A X
A Y
A X
A X
B Z
A Y
A Z
B Z
B X
A Y
C Z
C X
B Z
B Z
C X
A Y
B Z
A Y
B Z
B Z
A Y
A Y
B Y
A Y
B Z
B Z
A X
B Z
A Y
A Y
B X
A Y
B X
C X
B Z
A Y
B X
A Z
A Y
A Y
A Y
B Z
C X
A Y
B Z
C Y
A Y
B Z
A Z
C X
A Y
A Y
C X
A Y
B Y
A Z
A X
A Y
C X
B Z
C X
B Z
A Y
A Z
B X
B Y
C X
B Z
A Y
C Z
A Y
B Z
B Z
A Z
B Z
B Z
B Z
B X
A Z
B Z
B Z
C X
A Y
A Z
B Z
A Y
B Z
A Z
C X
A Y
B Z
C Z
C X
B Z
C X
A Y
A Y
A X
A Y
B X
A Z
A Y
C Z
C X
A Z
A Y
B Z
A Y
A Y
A Y
A Z
B Z
B Z
C X
A Y
A Y
A Y
A Y
A Z
A Y
C X
B X
B Z
C X
A Y
A Y
A Z
C X
A Y
A Y
A Y
B Z
A Y
B Y
A Y
B Z
B Z
B Z
A X
C X
A Y
B Y
A Z
C X
A Y
A Y
A Y
B Z
A Y
A Y
C Y
A Y
A Y
A Y
C X
A Z
A Y
A Y
A Y
C X
B Z
A Y
C X
B X
A Z
A Z
A X
A Z
A Y
B Y
C X
B Z
C X
C Z
A Y
C X
A Z
C X
B Z
B Z
A X
B Y
B Z
C Y
B Z
A Z
C X
A X
B Z
B Z
A Y
A Y
A Y
A X
A Z
B Z
A Y
A Y
A Y
A Y
A Z
B Z
B Z
B Z
A X
A Y
B Z
A Z
B Y
A Y
A Y
A Z
A Y
C Z
A Z
B Z
A Y
C X
A Y
C X
B Z
B Z
B Z
A Y
A Y
C X
B Z
C X
A Z
A Y
C Y
C X
C X
A Y
C X
B Z
B Z
B X
C X
B Z
A Y
A Y
A Y
A Y
A Y
B X
A Y
C X
C X
A Y
B Z
C X
C X
A Y
C Y
B Z
B Z
C X
A Z
B Z
A Y
A Z
A Z
A Y
A Y
A Y
B Y
B Z
A X
A Y
A Y
B Z
C X
C X
B Z
B Z
A Y
B X
C X
B Z
A Y
B Z
A Z
A X
A Y
A X
B Z
A Z
B Z
A Z
B Z
A Y
A Y
A Y
B Z
A X
A Y
A Y
C Z
A Y
A Y
C X
A Z
A Z
B Z
B X
A Y
A X
A Y
C X
C X
B Z
B Z
B Z
A Y
A Y
A Z
A Y
C X
C X
A Z
A X
A Z
B Z
A Z
A X
B Z
A Y
B Z
A Z
C X
C X
C Y
A Y
A Y
A Y
C Y
A Y
B Z
A X
A Y
A Z
A Y
B Z
A Z
B Z
B Z
A X
C X
A Y
A Y
B Z
B Z
A X
A Z
A X
C X
B Z
A Z
A Z
A Y
A Z
B Z
C X
A Z
B Z
A Y
A Z
A Y
C X
A Y
A Y
B Z
B Z
B Z
A Y
A Y
B Z
B X
C X
A Y
A Z
A Z
B Z
C X
B Y
A Z
A Z
B X
C Z
C X
A Y
A Z
A Z
C X
B Z
B Z
B Z
A Y
A Y
B Z
B Z
A Z
C X
B Z
B Z
A Y
A Y
A Z
A Y
C X
B X
A Z
C X
A Y
B Z
A Y
A Z
A Y
A Y
B Z
B Z
C X
A Y
A Y
B Z
A Z
A Y
A Y
A Y
A Y
C Z
A X
B Z
B Z
A Y
A Y
C X
A Y
C X
B Z
B Z
A Z
A Y
A Y
A Y
A Y
C X
B Z
A Y
B Z
A X
A Y
A Y
A Y
C Y
A Y
B Z
C X
A Y
A Y
A Y
A Y
A Y
A Y
A Z
A X
B Y
B Z
B Z
A Y
C Y
C X
A Y
B Z
A Y
B X
A X
B Z
A Y
A Y
A Y
A Y
C Y
A Y
B Z
A Y
B Z
A Z
B Z
A Y
A Y
A Z
B Z
A Y
A Y
A Y
B Z
A Y
B Z
B Z
A Y
A Y
A Y
A Y
A Z
A Y
A X
A Y
B Y
B Z
B Z
A Y
C X
B Z
B Y
B Y
C Z
C X
A Y
B Z
B Y
B Z
B X
A Z
A Y
C X
A Y
A Y
C X
A Y
C Y
B Y
C X
A Z
A Z
A Y
A Y
A X
C X
B Z
C X
C X
C X
A Y
A Y
A Y
A Z
C Y
A Z
B Z
C X
B Z
A Z
A Y
B Z
C X
B Z
C Z
C X
B X
A Y
C Z
C Z
B Z
B Z
A Y
B Y
B Z
A Y
A Y
B Z
A Z
C X
A Z
A Z
A Z
B Y
B Z
A Y
A Y
B Z
A Y
A Z
C X
A Y
A Z
A Z
A Y
A Y
B Z
A Y
A Y
A Y
A Y
A Y
A Z
B Z
A Y
B Z
B X
A Y
A Z
B Z
A Z
A Y
B Z
A Y
B Z
A Y
C X
A Y
B Z
A Y
B Z
A Y
A Y
A Y
B Z
A Y
B Z
C X
B Z
B Z
A Y
A X
A X
A X
A Y
C Y
A X
B Z
A X
C X
C X
C X
B Z
A Y
C X
C X
C Y
B Z
A X
A Y
A X
A X
B Y
C Z
A Y
C X
B Z
C Y
B Z
B Z
C Z
A Y
B Z
C X
A Y
C X
B Z
A Y
A Y
A Z
A Y
A Y
B Z
A Y
C X
C X
B Z
B Z
C X
A X
A Y
B Y
A X
A X
A Z
B Z
A Z
C X
A X
A Y
B Z
A Y
A Y
A X
A Y
A Y
A Z
A Y
B Z
C Z
B Z
B Z
A Y
C X
A Y
A Y
B Z
C Y
C X
A Y
A Y
A Y
A Z
A Y
A Y
A Y
B Z
C X
A Z
C X
B Z
A Y
B X
B Z
B Z
A Y
A Y
C X
B Z
A X
C X
A Z
B Z
B Z
C X
B Y
C X
A Y
A Z
C X
A Y
B Z
C X
A Y
A X
A X
A Y
C Y
B X
A Y
B X
A Y
A Y
C X
A Z
A Y
A Z
B Z
A Y
C X
A Z
B Z
A Y
C Z
A X
C X
A X
C Y
A Y
A Z
B Y
C X
B Z
A Y
C X
A Y
A Y
A Y
B Y
A X
C X
A Y
A Y
C X
B Z
A Z
A X
A Y
A Y
A Y
A Z
A Z
B Z
A Z
A Y
A Z
A Y
C X
A Z
C X
B Z
B Z
A X
A Y
B X
B Z
B Z
A Y
B X
A X
A X
A Y
A Y
C X
C X
A Y
C Y
A Y
C X
B Z
C X
B Z
A Z
B Y
A Y
B Z
A Y
C Y
B Z
B Z
A Y
A Y
B Z
A Y
B Z
B Z
A Z
A Y
A X
A Y
A Y
A Y
A Y
A Z
B Z
A Y
A Y
A Y
B Z
A Y
C X
C X
B Z
A Y
B Z
B X
A Y
C Z
A Z
A Y
B Z
A X
C X
B X
A Y
A Z
A Y
A Y
B Z
A Y
B Z
A Y
C X
A Y
A X
A Y
C X
A Y
C X
C X
B Z
A Y
B Z
A X
B Z
A Z
C X
B Z
A Y
A Y
A Z
B Z
A X
A Y
A Y
A Z
A Y
A X
A Y
C Y
A Y
A Y
A Y
A Z
A Y
B Z
B Z
A Y
A X
C X
B Z
B Z
A Z
A Y
A Z
B Z
A Y
A Y
C X
B Z
A X
A Y
B Z
A Y
B Z
B Z
B Z
A X
B Z
B Z
A Y
A Y
A Y
B Z
A Y
A X
B Z
A Y
B Z
A Y
A Y
B Z
A Y
A Y
B Z
A X
A Z
B X
A Y
B Z
C X
A Y
A Y
C Z
A Y
C X
A X
B Z
A Y
C X
A Z
A Y
B Z
A Z
A X
A Y
C X
A Z
A X
A Y
A X
B Z
B X
A Y
A Y
A Y
C X
A Y
A Y
B Z
B X
B Z
B Z
C X
C X
B Z
B Z
B Z
A Y
B Z
A Z
A Y
C X
B Z
A Y
A Y
B Z
C X
A Y
C X
A Y
B Z
A Z
A Y
A Y
A Y
B X
C X
B Z
B X
A Y
A Y
C X
A Y
A Z
C X
A Y
A Z
B Z
A Y
A Y
A Y
A Y
A Y
B Z
A Y
C X
A Y
B Z
C X
A X
B Z
C X
A Y
A Y
A Y
A Z
A X
A Y
C Y
A Y
A Z
A Y
A Y
B Z
A Y
B Z
A Y
C Y
A Z
A Y
C Z
A X
A Y
A Y
C Y
A Z
A X
C X
B X
A Y
C X
A Y
A Y
A Y
C Y
C X
A Y
B Z
A Y
A Y
A Y
A Z
A X
A Y
A Y
A Y
B Z
C Y
B Z
A Y
B Z
B Z
A Z
B Z
A X
B Z
C Y
A Y
A Y
C X
B Z
A Y
A Z
C X
B Z
A Z
B Z
C X
B X
C X
C X
C X
A Z
A Y
A Z
A Y
B Z
A Y
B Z
C X
A Y
B Z
B Z
A Y
A Y
B Z
A X
A Z
B Z
B Z
A Y
A Y
A Z
C X
A X
B Y
A Y
B Z
C X
A Y
A Y
A Y
A Y
A Y
B Z
B Z
B Z
A Y
A X
A Y
A X
B Y
B Z
A Z
A Y
A Y
A Y
B Z
B Y
C X
A Z
C Z
A Z
C Y
B Z
A Z
C Z
A Y
B Z
A Y
A Z
A Y
A Y
A Y
A Y
A Y
B Z
A Z
A Y
B X
C Y
C X
A Y
B Z
B Z
A Y
B Y
B Z
B Z
C X
C X
C X
B Z
A X
B Z
A Y
A Y
B X
A Z
A Y
B Z
A Z
A Y
B Z
B Z
C Y
B Z
B Z
B Z
B Z
A Z
B Z
A Y
B Z
A Y
C X
A Y
B Z
A Y
A Y
C X
A Y
A Y
A Y
A Y
B Z
A Y
A Y
A Y
A Z
C X
A Y
A Y
A Y
B Z
C X
A X
A Y
B X
C X
A Z
A X
B Z
A Y
C Y
C X
A Y
A Z
B Z
A Y
A Y
A Z
A Y
A Y
B Z
C Z
C X
A Y
A Y
C X
A Y
A X
B Z
C Z
A Y
B Z
C X
B Y
A Y
B X
A Y
A Y
B Z
B Z
A X
A Z
A Z
B Z
A Y
B Z
A Y
A Y
B Z
B Z
B X
A Y
B X
C X
B Z
B X
A Y
B Y
C X
A Y
A Z
A Y
B Z
A Y
B Z
A Y
B Z
A Y
A Y
A Y
A Z
B Z
A Y
B X
B X
B Z
B Z
A Z
B Z
B Z
A Y
C X
C Y
B Z
A Z
A Z
A Z
B Z
B Z
A Y
C X
A Y
A Y
C Y
A Z
A Y
A Y
A Y
A Y
A Y
B Z
A Y
C Y
B Z
B Z
A Y
A Y
A Y
A Y
A Y
B Z
B Z
A Y
B Z
B Z
B Z
A Y
A Y
C Z
B Y
A Y
C X
A Z
B Z
B Z
B Z
B Z
A X
A Y
A X
B Z
A Y
A X
A Y
B Z
B Z
C X
C Y
B Z
A Y
A Z
B X
A Z
A Y
A Z
A Y
C X
B Z
A Y
C Z
A Z
B Z
A Y
C X
B Z
A Z
A Y
A Y
A Y
A Z
C X
B X
A Y
C X
A Y
A Y
A Y
C X
A Y
A Y
A Y
A Y
A Y
A Y
B Z
A Y
B Z
B Z
B Z
A Y
A Y
A Y
A Y
A Z
A Y
B Z
B Z
C X
B Z
A Z
B Z
A Z
A Y
A X
B Z
A Y
A Y
B Z
B Z
A Y
B Z
B Z
A Y
A Y
B Z
B Z
A X
B Z
A Z
B Z
C X
B Z
A Y
B Y
A Y
B Z
A Y
C X
A Z
B Z
A Y
A Y
C X
A X
A Z
A Y
A Y
A Y
B Z
A X
A Z
A Y
A Y
A Y
A Y
C X
C X
B Z
B Z
A Y
A X
C X
B Z
B Z
B Z
A Z
B Z
B Z
B Z
A Z
A X
A Y
B Z
A X
B Z
A X
B X
B X
B Z
A Z
C X
A X
B Z
B Z
A Y
C Y
B Z
B Z
B Z
B Z
A Y
C X
B Z
B Z
A X
B Z
A Y
C X
C Z
A Y
A Y
B Z
B Z
A Z
A Y
B Z
C X
B Z
A Y
A Y
C X
A Y
A Y
A X
A Y
A Y
A Z
C X
A X
B Z
C X
A X
A Y
A Y
B Z
A Z
A Y
A X
B Z
A Y
A Y
A Z
A Y
A Y
A Y
B Z
C X
C X
A Y
A Y
A X
A Z
A Y
B X
C X
C X
B Z
A Y
B Z
B Z
A Y
A Y
A Y
A X
B X
A Y
B Z
A Y
A X
A Y
C Y
A Y
B Z
A Y
A X
B X
B Z
A Y
B Z
B Z
C X
A Z
A Y
B Z
A X
A Y
A Y
C X
B Z
B Z
A Z
A Y
A Z
A Z
B Z
B Z
A X
A Y
C X
A Y
B Z
B Z
A Y
A Y
A Z
A Y
A Y
A Y
A Y
C X
B Z
A Y
B Z
A Z
B Z
A Y
B Z
A Y
B Z
A Y
C X
A X
A Y
B X
C X
B Z
A Y
A Y
A Z
A Y
A Y
A Y
A Y
A Y
A Y
B Z
A X
B Z
B Z
A Y
C X
A Y
C X
A Z
A Y
C X
A Y
A Y
A Y
C X
A X
A Y
C Y
A Y
A Y
B Y
B Z
C X
A Y
B Z
B Z
A Y
A Y
B Z
A Y
A Y
B Z
A X
A X
B Y
B X
A Y
A Z
C X
A Y
A Y
A Y
A Z
C Z
A Z
A Y
A Z
A Y
A X
B Z
B Z
C X
B Z
C X
C X
A Z
A Y
A Y
A Y
A Y
A Y
C X
B Z
B Y
B Z
A Y
B Y
A Y
A Y
A Y
B Z
A Z
C Z
A Z
B Y
A Y
A Y
B Z
B Y
A Z
A Y
A Z
A Y
C X
B Z
B Z
A Z
A X
C X
A X
A X
B Z
C X
A Y
A Y
A Z
C X
C Y
C X
A Y
B Z
A Y
A X
A Z
B Z
A Y
A Y
A Y
B Z
A Y
B Z
B Y
B Z
B Z
B Z
B Z
A Y
B Z
B Z
A X
C Y
C X
A Y
A Z
B Z
A Y
A Y
A Y
A Y
A Y
A Y
B Z
B Z
A Y
B Y
B Z
A Y
A Y
A Z
A Y
A Y
A Y
B Y
B Z
C X
A Z
A Z
B Z
C Y
C Z
A Z
A Z
A Y
B Z
B Z
B Z
A Z
A Z
B Y
A Y
A X
A Y
B Z
A Y
B Z
A Y
A Y
A Z
B Z
A Y
A X
A Y
A Y
A Y
A Y
A Y
C X
A Y
B Z
A Z
A Y
A Y
A Y
C X
A Y
A Y
A Y
A Y
B Z
A Z
A Y
B X
B Z
B Z
B Z
B Z
A Y
A Y
C X
A Y
A Y
B Z
C X
B Z
B Y
A Y
A Y
A Y
B Z
B Z
B Z
A Y
B Z
B X
B Z
A Y
A Y
B Z
A Y
B Z
C Z
A Y
B Z
A Z
A Y
B Z
A Y
B Z
C X
B Z
A Y
A Y
A Y
A Z
B Z
C X
B X
A Y
A Y
A Y
C Y
B Z
B Z
A Y
A Y
C X
A Y
C Y
B Z
A Y
C X
A Z
A Y
A Y
A Z
A Y
A Y
B Z
B Z
C X
A Z
A Y
A Y
A X
A Y
C X
A Y
B Z
A X
A Z
B Z
C X
A Y
A Y
A Y
A Y
A Y
A Y
A Z
C X
A Y
A Y
B Z
A Z
A Z
A Z
A Y
B X
A Y
C X
A Y
A Y
A X
B Z
A Y
A Z
A Y
A Y
A Z
B Z
B Z
A Y
A Y
A Y
A Z
A X
A Z
C X
A Y
B Z
A Y
A X
A Y
A Z
A Y
A Z
A Y
B Z
A Y
B Z
B Z
B Z
A Y
C X
B Z
A Y
A Y
C X
A Y
A Z
B Z
B Z
A Y
C X
A Y
A Y
A Y
B Z
A Z
A Z
C X
B Z
C Y
B Z
A Y
A Y
A Y
A Y
A Y
C X
A X
A Z
A Y
A Y
B Z
A Y
B Z
B Z
A Y
C X
B Z
B Z
A Y
A Y
B X
C X
B Z
A Y
B Z
B Z
C Y
B Z
C X
A Y
A Y
A Y
C X
A Y
B Z
A Z
B Z
A Y
A Y
C X
A X
A Y
A Z
A Y
A Z
A Y
A Y
C X
A X
A X
C Y
A Y
A Y
A Y
A Y
B Z
C X
B Z
A X
A Y
A X
A X
A X
A Y
B Z
A Z
B X
C Y
C X
C X
C X
B Z
B Z
B Z
A X
A Y
A Y
B Z
C X
A Y
A Y
A Z
A Y
A Y
A X
C X
B Z
B Z
C X
A Y
A Y
A Y`;

const plays = parsePlays(input);
console.log(plays);
const score = plays.reduce((prev, cur) => prev + scorePlay(cur), 0);
console.log(score);

const plays2 = parsePlayWithScores(input);
console.log(plays2);
const score2 = plays2.reduce((prev, cur) => prev + scorePlay(playFromPlayToScore(cur)), 0);
console.log(score2);