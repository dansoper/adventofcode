fn main() {
    let input: &str = include_str!("input.txt");
    let mut sections = input.split("\n\n");
    let grid_string = sections.next().unwrap();
    let fold_instructions = sections.next().unwrap();
    let highest_coords = get_highest_coord(grid_string);
    let mut grid = vec![vec![false; highest_coords.x as usize + 1]; highest_coords.y as usize + 1];

    update_grid_from_coords(grid_string, &mut grid);
    process_fold_instructions(fold_instructions, &mut grid);
    // Part 2
    print_grid(&grid);
    // Part 1
    //println!("{:?}", count_trues(&grid));
}

struct Coord {
    x: u16,
    y: u16,
}
impl Coord {
    fn from(x: u16, y: u16) -> Coord {
        Coord { x, y }
    }
}

fn process_fold_instructions(fold_instructions: &str, grid: &mut Vec<Vec<bool>>) {
    for line in fold_instructions.lines() {
        if line.starts_with("fold along y=") {
            let num = line.replace("fold along y=", "");
            let num = num.parse::<u16>().unwrap();
            for y in num as usize..grid.len() {
                let new_y = num as usize - (y - num as usize);
                for x in 0..grid[0].len() {
                    if grid[y][x] {
                        grid[new_y][x] = true;
                    }
                }
            }
            grid.resize(num as usize, vec![false; 0]);
        }
        if line.starts_with("fold along x=") {
            let num = line.replace("fold along x=", "");
            let num = num.parse::<u16>().unwrap();
            for y in 0..grid.len() {
                for x in num as usize..grid[y].len() {
                    let new_x = num as usize - (x - num as usize);
                    if grid[y][x] {
                        grid[y][new_x] = true;
                    }
                }
                grid[y].resize(num as usize, false);
            }
        }
        // Part 1 - add the break
        //break;
    }
}

fn count_trues(list: &Vec<Vec<bool>>) -> u32 {
    let mut count = 0;
    for y in 0..list.len() {
        for x in 0..list[0].len() {
            if list[y][x] {
                count += 1;
            }
        }
    }
    count
}

fn print_grid(grid: &Vec<Vec<bool>>) {
    for y in 0..grid.len() {
        for x in 0..grid[0].len() {
            if grid[y][x] { print!("#"); } else { print!(" "); }
        }
        println!();
    }
}

fn update_grid_from_coords(str: &str, grid: &mut Vec<Vec<bool>>) {
    str.lines()
        .map(get_coord_from_line)
        .for_each(|item| grid[item.y as usize][item.x as usize] = true);
}

fn get_coord_from_line(ll: &str) -> Coord {
    let xy: Vec<&str> = ll.split(',').collect();
    let x = str::parse::<u16>(xy[0]).unwrap();
    let y = str::parse::<u16>(xy[1]).unwrap();
    Coord::from(x, y)
}

fn get_highest_coord(string: &str) -> Coord {
    let mut x = 0;
    let mut y = 0;
    let mut lines = string.lines();
    loop {
        let l = lines.next();
        if l == None { break; }
        let c = get_coord_from_line(l.unwrap());
        if c.x > x { x = c.x; }
        if c.y > y { y = c.y; }
    }
    Coord::from(x, y)
}