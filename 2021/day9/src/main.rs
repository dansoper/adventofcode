use std::collections::HashMap;

fn main() {
    let grid: Vec<Vec<u32>> = include_str!("input.txt")
        .lines()
        .map(parse_line_to_grid)
        .collect();

    let mut sum = 0;
    for y in 0..grid.len() {
        for x in 0..grid[y].len() {
            let item = grid[y][x];
            if (y == 0 || grid[y - 1][x] > item)
                && (x == 0 || grid[y][x - 1] > item)
                && (y == grid.len() - 1 || grid[y + 1][x] > item)
                && (x == grid[y].len() - 1 || grid[y][x + 1] > item) {
                sum += item + 1;
            }
        }
    }
    println!("{}", sum);
    let mut b = Basins::new(grid);
    b.part_two();
}

struct Basins {
    bottoms: HashMap<[u8; 2], [u8; 2]>,
    grid: Vec<Vec<u32>>
}
impl Basins {
    fn new(grid: Vec<Vec<u32>>) -> Basins {
        Basins {
            bottoms: HashMap::new(),
            grid
        }
    }

    fn part_two(&mut self) {
        // This puts in the bottoms and their adjacents
        self.get_initial_bottoms();

        // Now we keep going to the adjacents until they are all found
        loop {
            // Temporary array. These will be added to self.bottoms in at the end of each loop run
            let mut bottoms_to_add: HashMap<[u8; 2], [u8; 2]> = HashMap::new();
            let keys = self.bottoms.keys();
            for key in keys {
                let item = self.bottoms[key];
                let x = key[0];
                let y = key[1];
                if x != 0 {
                    let new_key = [x - 1, y];
                    if self.first_larger_than_second(new_key, *key) && !self.bottoms.contains_key(&new_key) && !bottoms_to_add.contains_key(&new_key) { bottoms_to_add.insert(new_key, item); }
                }
                if x != ((self.grid[y as usize].len() - 1) as u8) {
                    let new_key = [x + 1, y];
                    if self.first_larger_than_second(new_key, *key) && !self.bottoms.contains_key(&new_key) && !bottoms_to_add.contains_key(&new_key) { bottoms_to_add.insert(new_key, item); }
                }
                if y != 0 {
                    let new_key = [x, y - 1];
                    if self.first_larger_than_second(new_key, *key) && !self.bottoms.contains_key(&new_key) && !bottoms_to_add.contains_key(&new_key) { bottoms_to_add.insert(new_key, item); }
                }
                if y != ((self.grid.len() - 1) as u8) {
                    let new_key = [x, y + 1];
                    if self.first_larger_than_second(new_key, *key) && !self.bottoms.contains_key(&new_key) && !bottoms_to_add.contains_key(&new_key) { bottoms_to_add.insert(new_key, item); }
                }
            }
            let keys = bottoms_to_add.keys();
            for key in keys {
                self.bottoms.insert(*key, bottoms_to_add[key]);
            }
            if self.bottoms.len() >= (self.grid.len() * self.grid[0].len()) {
                break;
            }
        }

        let n = self.get_top_basin_multiple();
        println!("{}", n);
    }

    fn first_larger_than_second(&self, first: [u8; 2], second: [u8; 2]) -> bool {
        let first_item: u32 = self.grid[first[1] as usize][first[0] as usize];
        let second_item: u32 = self.grid[second[1] as usize][second[0] as usize];
        first_item >= second_item
    }

    fn get_top_basin_multiple(&self) -> u32 {
        let mut basins: HashMap<[u8; 2], u32> = HashMap::new();
        for key in self.bottoms.keys() {
            let item = self.bottoms[key];
            if self.grid[key[1] as usize][key[0] as usize] != 9 {
                *basins.entry(item).or_insert(0) += 1;
            }
        }
        let mut n: Vec<u32> = basins.iter().map(|item| (item.1).clone()).collect();
        n.sort_by(|a, b| b.cmp(a));
        n[0] * n[1] * n[2]
    }

    fn get_initial_bottoms(&mut self) {
        for y in 0..self.grid.len() {
            for x in 0..self.grid[y].len() {
                let item = self.grid[y][x];
                if (y == 0 || self.grid[y - 1][x] > item)
                    && (x == 0 || self.grid[y][x - 1] > item)
                    && (y == self.grid.len() - 1 || self.grid[y + 1][x] > item)
                    && (x == self.grid[y].len() - 1 || self.grid[y][x + 1] > item) {
                        if x != 0 { self.bottoms.insert([(x - 1) as u8, y as u8], [x as u8, y as u8]); }
                        if y != 0 { self.bottoms.insert([x as u8, (y - 1) as u8], [x as u8, y as u8]); }
                        if x != self.grid[y].len() - 1 { self.bottoms.insert([(x + 1) as u8, y as u8], [x as u8, y as u8]); }
                        if y != self.grid.len() - 1 { self.bottoms.insert([x as u8, (y + 1) as u8], [x as u8, y as u8]); }
                        self.bottoms.insert([x as u8, y as u8], [x as u8, y as u8]);
                }
            }
        }
    }
}



fn parse_line_to_grid(line: &str) -> Vec<u32> {
    line.chars().map(|item| char::to_digit(item, 10).unwrap()).collect()
}