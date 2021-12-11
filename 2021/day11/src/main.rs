fn main() {
    let grid: Vec<Vec<u8>> = include_str!("input.txt")
        .lines()
        .map(parse_line_to_grid)
        .collect();

    let max = grid.len() * grid[0].len();
    let mut o = Octopuses::new(grid);
    let mut count = 0;
    let mut i = 0;
    loop {
        i +=1;
        let c = o.increment_octopuses();
        if c == max as u32 { println!("Maxed out at {}", i); break; }
        count += c;
        if i == 100 { println!("After 100: {}", count); }
    }
}
#[derive(PartialEq)]
struct Coord {
    x: u8,
    y: u8,
}
impl Coord {
    fn from(x: u8, y: u8) -> Coord {
        Coord { x, y }
    }
}

struct Octopuses {
    grid: Vec<Vec<u8>>,
}

impl Octopuses {
    fn new(grid: Vec<Vec<u8>>) -> Octopuses {
        Octopuses { grid }
    }

    fn increment_octopuses(&mut self) -> u32 {
        let mut flashes = 0;
        let mut to_flash = Vec::new();
        for y in 0..self.grid.len() {
            for x in 0..self.grid[y].len() {
                self.grid[y][x] += 1;
                if self.grid[y][x] > 9 {
                    to_flash.push(Coord::from(x as u8, y as u8));
                }
            }
        }
        let mut flashed = Vec::new();
        while !to_flash.is_empty() {
            //let item = to_flash.pop().unwrap();
            let item = to_flash.remove(0);
            // TODO
            flashed.push(Coord::from(item.x, item.y));
            flashes += 1;
            let coords = self.get_adjacent_coords(item);
            for coord in coords {
                self.grid[coord.y as usize][coord.x as usize] += 1;
                if self.grid[coord.y as usize][coord.x as usize] > 9 {
                    if !flashed.contains(&&coord) && !to_flash.contains(&coord) {
                        //println!("Flashing {}, {}", coord.x, coord.y);
                        to_flash.push(coord);
                    } else {
                        //println!("Blocking {}, {}", coord.x, coord.y);
                    }
                }
            }
        }
        for y in 0..self.grid.len() {
            for x in 0..self.grid[y].len() {
                if self.grid[y][x] > 9 {
                    self.grid[y][x] = 0;
                }
            }
        }
        flashes
    }

    fn get_adjacent_coords(&self, coord: Coord) -> Vec<Coord> {
        let mut ret = Vec::new();
        if coord.x != 0 {
            ret.push(Coord::from(coord.x - 1, coord.y));
            if coord.y != 0 { ret.push(Coord::from(coord.x - 1, coord.y - 1)); }
            if coord.y != self.grid.len() as u8 - 1 { ret.push(Coord::from(coord.x - 1, coord.y + 1)); }
        }
        if coord.x != self.grid[0].len() as u8 - 1 {
            ret.push(Coord::from(coord.x + 1, coord.y));
            if coord.y != 0 { ret.push(Coord::from(coord.x + 1, coord.y - 1)); }
            if coord.y != self.grid.len() as u8 - 1 { ret.push(Coord::from(coord.x + 1, coord.y + 1)); }
        }
        if coord.y != 0 { ret.push(Coord::from(coord.x, coord.y - 1)); }
        if coord.y != self.grid[0].len() as u8 - 1 { ret.push(Coord::from(coord.x, coord.y + 1)); }
        ret
    }
}

fn parse_line_to_grid(line: &str) -> Vec<u8> {
    line.chars()
        .map(|item| char::to_digit(item, 10).unwrap() as u8)
        .collect()
}
