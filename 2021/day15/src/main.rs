use std::collections::HashMap;

fn main() {
    let grid: Vec<Vec<u8>> = include_str!("input.txt")
        .lines()
        .map(parse_line_to_grid)
        .collect();

    let mut p = PathFinder::from_grid(grid);
    // only for part 2
    p.expand_grid();
    p.find_paths();
}

struct PathFinder {
    grid: Vec<Vec<u8>>,
    best_scores: HashMap<Coord, u32>
}

impl PathFinder {
    fn from_grid(grid: Vec<Vec<u8>>) -> PathFinder {
        PathFinder {
            grid,
            best_scores: HashMap::new()
        }
    }

    fn expand_grid(&mut self) {
        let y_size = self.grid.len();
        let x_size = self.grid[0].len();
        // expand down
        for i in 1..5 {
            for y in 0..y_size {
                self.grid.push(Vec::new());
                for x in 0..x_size {
                    let mut n = self.grid[y][x] + i;
                    if n > 9 { n -= 9; }
                    self.grid[(y_size * i as usize) + y].push(n);
                }
            }
        }
        // expand right
        let y_size = self.grid.len();
        for i in 1..5 {
            for y in 0..y_size {
                for x in 0..x_size {
                    let mut n = self.grid[y][x] + i;
                    if n > 9 { n -= 9; }
                    self.grid[y].push(n);
                }
            }
        }
    }

    fn find_paths(&mut self) {
        let mut to_do: Vec<(Coord, Vec<Coord>, u32)> = Vec::new();
        to_do.push((Coord::from(0, 0), Vec::new(), 0));
        loop {
            if to_do.len() == 0 { break; }
            let item = to_do.remove(0);
            let mut tasks = self.find_path_from(item.0, item.1, item.2);
            to_do.append(&mut tasks);
            to_do.sort_by(|item, item2| item.2.cmp(&item2.2));
        }
        println!("Best: {}", self.best_scores[&Coord::from(self.grid[0].len() as u16 - 1, self.grid.len() as u16 - 1)])
    }

    fn find_path_from(&mut self, coord: Coord, beens: Vec<Coord>, score: u32) -> Vec<(Coord, Vec<Coord>, u32)> {
        let adjacents = self.get_adjacent_coords(&coord);
        let mut to_do = Vec::new();
        for adj in adjacents {
            let x = adj.x as usize;
            let y = adj.y as usize;
            if self.best_scores.contains_key(&adj) && self.best_scores[&adj] <= (score + self.grid[y][x] as u32) {
                continue;
            }
            if beens.contains(&adj) {
                continue;
            }
            let mut new_beens = beens.to_vec();
            new_beens.push(adj.clone());
            *self.best_scores.entry(adj).or_insert(0) = score + self.grid[y][x] as u32;
            to_do.push((Coord::from(x as u16, y as u16), new_beens, score + self.grid[y][x] as u32))
        }
        to_do
    }

    fn get_adjacent_coords(&self, coord: &Coord) -> Vec<Coord> {
        let mut ret = Vec::new();
        if coord.x != 0 { ret.push(Coord::from(coord.x - 1, coord.y)); }
        if coord.x != self.grid[0].len() as u16 - 1 { ret.push(Coord::from(coord.x + 1, coord.y)); }
        if coord.y != 0 { ret.push(Coord::from(coord.x, coord.y - 1)); }
        if coord.y != self.grid[0].len() as u16 - 1 { ret.push(Coord::from(coord.x, coord.y + 1)); }
        ret
    }
}

#[derive(PartialEq, Eq, Hash, Clone, Debug)]
struct Coord {
    x: u16,
    y: u16,
}
impl Coord {
    fn from(x: u16, y: u16) -> Coord {
        Coord { x, y }
    }
}

fn parse_line_to_grid(line: &str) -> Vec<u8> {
    line.chars()
        .map(|item| char::to_digit(item, 10).unwrap() as u8)
        .collect()
}