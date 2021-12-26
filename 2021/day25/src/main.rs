fn main() {
    let input: &str = include_str!("input.txt");
    let mut grid = Grid::from(input);

    let mut runs = 0;
    loop {
        runs += 1;
        let moves = do_move(&mut grid);
        if moves == 0 {
            break;
        }
    }
    println!("Count {}", runs);
}

fn do_move(grid: &mut Grid) -> usize {
    let mut total_moves = 0;

    let mut moves = Vec::new();
    for x in 0..grid.grid.len() {
        for y in 0..grid.grid[x].len() {
            if grid.grid[x][y].is_some() {
                if grid.grid[x][y].unwrap() == Cucumber::Right {
                    let new_coords = grid.make_safe(x + 1, y);
                    if grid.grid[new_coords.x][new_coords.y].is_none() {
                        moves.push(Move {
                            move_type: Some(Cucumber::Right),
                            coord: new_coords
                        });
                        moves.push(Move {
                            move_type: None,
                            coord: Coord { x, y }
                        });
                    }
                }
            }
        }
    }
    total_moves += moves.len();
    for the_move in moves {
        grid.grid[the_move.coord.x][the_move.coord.y] = the_move.move_type;
    }

    let mut moves = Vec::new();
    for x in 0..grid.grid.len() {
        for y in 0..grid.grid[x].len() {
            if grid.grid[x][y].is_some() {
                if grid.grid[x][y].unwrap() == Cucumber::Down {
                    let new_coords = grid.make_safe(x, y + 1);
                    if grid.grid[new_coords.x][new_coords.y].is_none() {
                        moves.push(Move {
                            move_type: Some(Cucumber::Down),
                            coord: new_coords
                        });
                        moves.push(Move {
                            move_type: None,
                            coord: Coord { x, y }
                        });
                    }
                }
            }
        }
    }
    total_moves += moves.len();
    for the_move in moves {
        grid.grid[the_move.coord.x][the_move.coord.y] = the_move.move_type;
    }
    total_moves
}


struct Move {
    move_type: Option<Cucumber>,
    coord: Coord
}

struct Coord {
    x: usize,
    y: usize
}

#[derive(Debug, Clone, PartialEq, Copy)]
enum Cucumber {
    Right,
    Down
}

#[derive(Debug)]
struct Grid {
    grid: Vec<Vec<Option<Cucumber>>>
}

impl Grid {
    fn from(str: &str) -> Grid {
        let mut grid = Vec::new();
        let mut y = 0;
        for line in str.lines() {
            let mut x = 0;
            for char in line.chars() {
                if grid.len() < x + 1 {
                    grid.resize(x + 1, vec![None]);
                }
                if grid[x].len() < y + 1 {
                   grid[x].resize(y + 1, None);
                }
                let mut c = None;
                if char == 'v' {
                    c = Some(Cucumber::Down)
                } else if char == '>' {
                    c = Some(Cucumber::Right)
                }
                grid[x][y] = c;
                x += 1;
            }
            y += 1;
        }
        Grid { grid }
    }
    fn make_safe(&self, x: usize, y: usize) -> Coord {
        let mut x = x;
        let mut y = y;
        while x >= self.grid.len() {
            x = x - self.grid.len();
        }
        while y >= self.grid[x].len() {
            y = y - self.grid[x].len();
        }
        Coord{ x, y }
    }
}