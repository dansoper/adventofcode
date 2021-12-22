use std::cmp;

fn main() {
    let input: &str = include_str!("input.txt");
    let mut p = Picture::from(input);
    p.enhance(2);
    p.enhance(48);
}

struct Picture {
    grid: Grid,
    template: Vec<bool>,
}

impl Picture {
    fn from(str: &str) -> Picture {
        let mut sections = str.split("\n\n");
        let template_string = sections.next().unwrap();
        let template = parse_template_to_bools(template_string);
        let mut grid = Grid::new();
        parse_grid(sections.next().unwrap(), &mut grid);
        Picture { grid, template }
    }
    fn run_enhancement(&mut self) {
        let min_max = self.grid.min_max();
        let mut new_grid = Grid::new();
        // This intentionally goes too far each way - that's what is required
        for y in min_max.min_y-2..=min_max.max_y+2 {
            for x in min_max.min_x-2..=min_max.max_x+2 {
                let mut str = String::from("");
                for i in -1..=1 {
                    for j in -1..=1 {
                        let v = self.grid.get(x + j, y + i);
                        if v {
                            str.push_str("1");
                        } else {
                            str.push_str("0");
                        }
                    }
                }
                let num = usize::from_str_radix(&str, 2).unwrap();
                let new_b = self.template[num];
                new_grid.set(new_b, x, y);
            }
        }
        self.grid = new_grid;
    }

    fn enhance(&mut self, max: u8) {
        for _i in 0..max {
            self.run_enhancement();
        }
        println!("{:?}", self.grid.count_trues());
    }
}

fn parse_template_to_bools(str: &str) -> Vec<bool> {
    let mut v = Vec::new();
    for char in str.chars() {
        if char == '#' {
            v.push(true);
        } else {
            v.push(false);
        }
    }
    v
}

fn parse_grid(str: &str, grid: &mut Grid) {
    let mut y = 0;
    let mut max_x = 0;
    for line in str.lines() {
        let mut x = 0;
        for char in line.chars() {
            let mut b = false;
            if char == '#' {
                b = true;
            }
            grid.set(b, x, y);
            x += 1;
            max_x = x;
        }
        grid.set(false, x, y);
        y += 1;
    }
    for x in 0..max_x {
        grid.set(false, x, y);
    }
}

struct Grid {
    top_left: Vec<Vec<bool>>,
    top_right: Vec<Vec<bool>>,
    bottom_left: Vec<Vec<bool>>,
    bottom_right: Vec<Vec<bool>>,
}

impl Grid {
    fn new() -> Grid {
        Grid {
            top_left: vec![vec![false; 1]; 1],
            top_right: vec![vec![false; 1]; 1],
            bottom_left: vec![vec![false; 1]; 1],
            bottom_right: vec![vec![false; 1]; 1],
        }
    }

    fn set(&mut self, b: bool, x: i64, y: i64) {
        let yy = y.abs() as usize;
        let xx = x.abs() as usize;
        if x >= 0 && y >= 0 {
            // bottom right
            resize_grid_vec(&mut self.bottom_right, xx, yy );
            self.bottom_right[yy][xx] = b;
        } else if x >= 0 {
            // top right
            resize_grid_vec(&mut self.top_right, xx, yy);
            self.top_right[yy][xx] = b;
        } else if y < 0 {
            // top left
            resize_grid_vec(&mut self.top_left, xx, yy);
            self.top_left[yy][xx] = b;
        } else {
            // bottom left
            resize_grid_vec(&mut self.bottom_left, xx, yy);
            self.bottom_left[yy][xx] = b;
        }
    }
    fn get(&mut self, x: i64, y: i64) -> bool {
        let mut yy = y.abs() as usize;
        let mut xx = x.abs() as usize;
        if x >= 0 && y >= 0 {
            // bottom right
            let i = alternative_x_y(&mut self.bottom_right, xx, yy);
            xx = i.0;
            yy = i.1;
            return self.bottom_right[yy][xx];
        } else if x >= 0 {
            // top right
            let i = alternative_x_y(&mut self.top_right, xx, yy);
            xx = i.0;
            yy = i.1;
            return self.top_right[yy][xx];
        } else if y < 0 {
            // top left
            let i = alternative_x_y(&mut self.top_left, xx, yy);
            xx = i.0;
            yy = i.1;
            return self.top_left[yy][xx];
        } else {
            // bottom left
            let i = alternative_x_y(&mut self.bottom_left, xx, yy);
            xx = i.0;
            yy = i.1;
            return self.bottom_left[yy][xx];
        }
    }
    fn min_max(&self) -> MinMax {
        // min x
        let min_x = -(cmp::max(
            self.top_left.iter().fold(0, |iter, item| cmp::max(item.len(), iter)), 
            self.bottom_left.iter().fold(0, |iter, item| cmp::max(item.len(), iter))) as i64);
        let max_x = cmp::max(
            self.top_right.iter().fold(0, |iter, item| cmp::max(item.len(), iter)), 
            self.bottom_right.iter().fold(0, |iter, item| cmp::max(item.len(), iter))) as i64;
        let min_y = -(cmp::max(self.top_left.len(), self.bottom_left.len()) as i64);
        let max_y = cmp::max(self.top_right.len(), self.bottom_right.len()) as i64;
        MinMax {
            min_x, min_y, max_x, max_y
        }
    }
    fn count_trues(&self) -> u64 {
        count_trues(&self.bottom_left)
            + count_trues(&self.bottom_right)
            + count_trues(&self.top_left)
            + count_trues(&self.top_right)
    }
}

fn resize_grid_vec(arr: &mut Vec<Vec<bool>>, xx: usize, yy: usize) {
    if arr.len() <= yy {
        arr.resize(yy + 1, Vec::new());
    }
    if arr[yy].len() <= xx {
        arr[yy].resize(xx + 1, false);
    }
}

fn alternative_x_y(arr: &mut Vec<Vec<bool>>, xx: usize, yy: usize) -> (usize, usize) {
    let mut xxx = xx;
    let mut yyy = yy;
    if arr.len() <= yyy {
        yyy = arr.len() - 1;
    }
    if arr[yyy].len() <= xxx {
        xxx = arr[yyy].len() - 1;
    }
    (xxx, yyy)
}

fn count_trues(list: &Vec<Vec<bool>>) -> u64 {
    let mut count = 0;
    for y in 0..list.len() {
        for x in 0..list[y].len() {
            if list[y][x] {
                count += 1;
            }
        }
    }
    count
}

#[derive(Debug)]
struct MinMax {
    min_x: i64,
    min_y: i64,
    max_x: i64,
    max_y: i64
}

fn print_grid(grid: &mut Grid) {
    let min_max = grid.min_max();
    for y in min_max.min_y..min_max.max_y {
        for x in min_max.min_x..min_max.max_x {
            if grid.get(x, y) { print!("#"); } else { print!(" "); }
        }
        println!();
    }
}