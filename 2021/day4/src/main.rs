fn main() {
    let mut list: Vec<&str> = include_str!("input.txt").lines().collect();
    let first_line = list.remove(0);
    let drawn_numbers: Vec<u16> = first_line
        .split(',')
        .map(str::parse::<u16>)
        .map(Result::unwrap)
        .collect();

    let boards = parse_to_boards(list);
    let first_and_last = get_winning_board(boards, drawn_numbers);
    println!("{:?}", first_and_last);
}

struct BoardPosition {
    x: u16,
    y: u16,
    number: u16,
    marked: bool,
}
// I'm using the rows and cols property to quickly mark items as marked. Using the bits in a number we can see which ones are marked
struct Board {
    positions: Vec<BoardPosition>,
    rows: Vec<u32>,
    cols: Vec<u32>,
    win_index: i32,
}

fn parse_to_boards(lines: Vec<&str>) -> Vec<Board> {
    let mut positions = Vec::new();
    let mut boards = Vec::new();
    let mut row = 0;
    let mut col = 0;
    for i in 0..lines.len() {
        if lines[i] == "" {
            if positions.len() > 0 {
                boards.push(process_board(positions, col, row));
                positions = Vec::new();
                row = 0;
            }
        } else {
            let mut chars: Vec<char> = lines[i].chars().collect();
            let mut j = 0;
            while chars.len() > 0 {
                let mut l = 3;
                if chars.len() == 2 {
                    l = 2;
                }
                let num: String = chars.drain(0..l).collect();
                let num = num.trim().parse::<u16>().unwrap();
                positions.push(BoardPosition {
                    y: row as u16,
                    x: j as u16,
                    number: num,
                    marked: false,
                });
                j = j + 1;
                col = j;
            }
            row = row + 1;
        }
    }
    // Last board
    boards.push(process_board(positions, col, row));
    boards
}
fn process_board(positions: Vec<BoardPosition>, x: usize, y: usize) -> Board {
    Board {
        positions,
        rows: vec![0; y],
        cols: vec![0; x],
        win_index: -1,
    }
}
// This assumes a square board - then returns the binary equivalent of all rows matched (i.e. 5 rows, 11111, 31)
fn get_complete_row_number(boards: &Vec<Board>) -> u32 {
    let num_rows = (boards[0].positions.len() as f64).sqrt() as u16;
    let mut dec = 0;
    for i in 0..num_rows {
        dec = dec | 1 << i;
    }
    dec
}
fn get_winning_board(mut boards: Vec<Board>, drawn_numbers: Vec<u16>) -> (u32, u32) {
    let complete = get_complete_row_number(&boards);
    let mut win = 0;
    let mut first_win: Option<u32> = None;
    for draw_i in 0..drawn_numbers.len() {
        let num = drawn_numbers[draw_i];
        for line_i in 0..boards.len() {
            let board = &mut boards[line_i];
            // A board that has already won is ignored
            if board.win_index >= 0 {
                continue;
            }
            let found_position = board.positions.iter_mut().find(|p| p.number == num);
            if let Some(p) = found_position {
                p.marked = true;
                board.rows[p.y as usize] = board.rows[p.y as usize] | (1 << p.x);
                board.cols[p.x as usize] = board.cols[p.x as usize] | (1 << p.y);
                if board.rows[p.y as usize] == complete || board.cols[p.x as usize] == complete {
                    let sum = board
                        .positions
                        .iter()
                        .filter(|pos| pos.marked == false)
                        .fold(0, |iter, item| iter + item.number);
                    board.win_index = draw_i as i32;
                    win = sum as u32 * num as u32;
                    if first_win.is_none() {
                        first_win = Some(win);
                    }
                }
            }
        }
    }
    return (first_win.unwrap(), win);
}
