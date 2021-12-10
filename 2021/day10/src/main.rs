fn main() {
    let list: Vec<&str> = include_str!("input.txt")
        .lines()
        .collect();

    let mut sum = 0;
    let mut complete_scores = Vec::new();
    for i in 0..list.len() {
        let mut died = false;
        let line = list[i];
        let mut opened_brackets: Vec<char> = Vec::new();
        for char in line.chars() {
            if is_opening_bracket(char) {
                opened_brackets.push(char);
            } else if is_closing_bracket(char) {
                let a = opened_brackets.last().unwrap();
                if matching_close(a) != char {
                    // die
                    sum += points_for_char(char);
                    died = true;
                    break;
                } else {
                    opened_brackets.pop();
                }
            }
        }
        if died == false {
            let res = calc_completion_score(&mut opened_brackets);
            complete_scores.push(res);
        }
    }
    println!("{}", sum);
    complete_scores.sort();
    println!("{}", complete_scores[complete_scores.len() / 2]);
}

fn is_opening_bracket(c: char) -> bool {
    c == '(' || c == '[' || c == '{' || c == '<'
}
fn is_closing_bracket(c: char) -> bool {
    c == ')' || c == ']' || c == '}' || c == '>'
}
fn matching_close(c: &char) -> char {
    match *c {
        '(' => ')',
        '[' => ']',
        '{' => '}',
        '<' => '>',
        _ => '0'
    }
}
fn points_for_char(c: char) -> u32 {
    match c {
        ')' => 3,
        ']' => 57,
        '}' => 1197,
        '>' => 25137,
        _ => 0
    }
}
fn part_two_points_for_char(c: char) -> u32 {
    match c {
        ')' => 1,
        ']' => 2,
        '}' => 3,
        '>' => 4,
        _ => 0
    }
}
fn calc_completion_score(list: &mut Vec<char>) -> u64 {
    let mut score: u64 = 0;
    list.reverse();
    for c in list {
        score = (score * 5) + part_two_points_for_char(matching_close(c)) as u64;
    }
    score
}