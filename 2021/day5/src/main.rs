fn main() {
    let list: Vec<Instruction> = include_str!("input.txt")
        .lines()
        .map(parse_line_to_instruction)
        .collect();

    let highest_x = list.iter().fold(0, |iter, item| {
        if item.from.x > iter && item.from.x >= item.to.x {
            return item.from.x;
        } else if item.to.x > iter {
            return item.to.x;
        } else {
            return iter;
        }
    }) + 1;

    // This is a lot of duplication.  Need to learn how to do this better!
    let highest_y = list.iter().fold(0, |iter, item| {
        if item.from.y > iter && item.from.y >= item.to.y {
            return item.from.y;
        } else if item.to.y > iter {
            return item.to.y;
        } else {
            return iter;
        }
    }) + 1;

    // Part one
    let mut grid = vec![vec![0; highest_y as usize]; highest_x as usize];
    process_instructions_horiz_vert(&mut grid, &list);
    let count = count_intersections(&grid);
    println!("Part one: {}", count);

    // Part two
    let mut grid = vec![vec![0; highest_y as usize]; highest_x as usize];
    process_instructions(&mut grid, &list, true);
    let count = count_intersections(&grid);
    println!("Part two: {}", count);
}

struct Coord2d {
    x: i32,
    y: i32,
}
struct Instruction {
    from: Coord2d,
    to: Coord2d,
}

fn parse_line_to_instruction(str: &str) -> Instruction {
    let mut from_and_to: Vec<&str> = str.split(" -> ").collect();
    let from = from_and_to.remove(0);
    let to = from_and_to.remove(0);
    Instruction {
        from: parse_coords(from),
        to: parse_coords(to),
    }
}
fn parse_coords(str: &str) -> Coord2d {
    let mut coords: Vec<i32> = str
        .split(',')
        .map(str::parse::<i32>)
        .map(Result::unwrap)
        .collect();
    Coord2d {
        x: coords.remove(0),
        y: coords.remove(0),
    }
}
fn process_instructions_horiz_vert(grid: &mut Vec<Vec<i32>>, instructions: &Vec<Instruction>) {
    process_instructions(grid, &instructions, false);
}
fn process_instructions(
    grid: &mut Vec<Vec<i32>>,
    instructions: &Vec<Instruction>,
    include_diagonal: bool,
) {
    for i in 0..instructions.len() {
        let instruction = &instructions[i];
        let mut move_x = 1;
        let mut move_y = 1;
        if instruction.from.x != instruction.to.x && instruction.from.y != instruction.to.y {
            // diagonal
            if !include_diagonal {
                continue;
            }
            if instruction.from.x > instruction.to.x {
                move_x = -1;
            }
            if instruction.from.y > instruction.to.y {
                move_y = -1;
            }
        } else if instruction.from.x == instruction.to.x {
            // vertical
            move_x = 0;
            if instruction.from.y > instruction.to.y {
                move_y = -1;
            }
        } else if instruction.from.y == instruction.to.y {
            // horizontal
            move_y = 0;
            if instruction.from.x > instruction.to.x {
                move_x = -1;
            }
        }
        let mut x = instruction.from.x;
        let mut y = instruction.from.y;
        loop {
            grid[x as usize][y as usize] = grid[x as usize][y as usize] + 1;
            if y == instruction.to.y && x == instruction.to.x {
                break;
            }
            x = x + move_x;
            y = y + move_y;
        }
    }
}
fn count_intersections(grid: &Vec<Vec<i32>>) -> u32 {
    let mut count_intersections = 0;
    for x in 0..grid.len() {
        for y in 0..grid[x].len() {
            if grid[x][y] > 1 {
                count_intersections = count_intersections + 1;
            }
        }
    }
    count_intersections
}
