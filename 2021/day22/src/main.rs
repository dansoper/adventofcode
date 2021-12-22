use std::cmp;

use regex::Regex;

fn main() {
    let list: Vec<Instruction> = include_str!("input.txt")
        .lines()
        .map(parse_line_to_instruction)
        .collect();

    // We could use the Part 2 code for part 1 but I'd already written it...
    let mut g = Grid3d::new();
    for ins in &list {
        if is_small(&ins) {
            for x in ins.min_x..=ins.max_x {
                for y in ins.min_y..=ins.max_y {
                    for z in ins.min_z..=ins.max_z {
                        g.set(ins.on, x, y, z);
                    }
                }
            }
        }
    }
    println!("Part 1: {}", g.count_trues());

    let mut cuboids: Vec<Cuboid> = Vec::new();
    for ins in &list {
        let mut new_cuboids: Vec<Cuboid> = Vec::new();
        for existing in &cuboids {
            new_cuboids.append(&mut existing.create_cuboids_by_subtracting(Cuboid::from(ins.min_x, ins.max_x, ins.min_y, ins.max_y, ins.min_z, ins.max_z)).clone());
        }
        if ins.on {
            new_cuboids.push(Cuboid::from(ins.min_x, ins.max_x, ins.min_y, ins.max_y, ins.min_z, ins.max_z));
        }
        cuboids = new_cuboids;
    }
    println!("Part 2: {}", cuboids.iter().fold(0, |iter, item| iter + item.volume()));
}


#[derive(Clone)]
struct Cuboid {
    min_x: i64,
    max_x: i64,
    min_y: i64,
    max_y: i64,
    min_z: i64,
    max_z: i64
}

impl Cuboid {
    fn from(min_x: i64, max_x: i64, min_y: i64, max_y: i64, min_z: i64, max_z: i64) -> Cuboid {
        Cuboid { min_x, max_x, min_y, max_y, min_z, max_z }
    }

    fn create_cuboids_by_subtracting(&self, subtract: Cuboid) -> Vec<Cuboid> {
        if !self.overlaps(&subtract) {
            return vec![self.clone()];
        }
        let mut new_cuboids = Vec::new();
        if subtract.min_x > self.min_x {
            new_cuboids.push(Cuboid::from(self.min_x, subtract.min_x - 1, self.min_y, self.max_y, self.min_z, self.max_z));
        }
        if subtract.max_x < self.max_x {
            new_cuboids.push(Cuboid::from(subtract.max_x + 1, self.max_x, self.min_y, self.max_y, self.min_z, self.max_z));
        }
        let new_min_x = cmp::max(self.min_x, subtract.min_x);
        let new_max_x = cmp::min(self.max_x, subtract.max_x);
        if subtract.min_y > self.min_y {
            new_cuboids.push(Cuboid::from(new_min_x, new_max_x, self.min_y, subtract.min_y - 1, self.min_z, self.max_z));
        }
        if subtract.max_y < self.max_y {
            new_cuboids.push(Cuboid::from(new_min_x, new_max_x, subtract.max_y + 1, self.max_y, self.min_z, self.max_z));
        }
        let new_min_y = cmp::max(self.min_y, subtract.min_y);
        let new_max_y = cmp::min(self.max_y, subtract.max_y);
        if subtract.min_z > self.min_z {
            new_cuboids.push(Cuboid::from(new_min_x, new_max_x, new_min_y, new_max_y, self.min_z, subtract.min_z - 1));
        }
        if subtract.max_z < self.max_z {
            new_cuboids.push(Cuboid::from(new_min_x, new_max_x, new_min_y, new_max_y, subtract.max_z + 1, self.max_z));
        }
        new_cuboids
    }

    fn overlaps(&self, other: &Cuboid) -> bool {
        other.min_x <= self.max_x && self.min_x <= other.max_x
        && other.min_y <= self.max_y && self.min_y <= other.max_y
        && other.min_z <= self.max_z && self.min_z <= other.max_z
    }

    fn volume(&self) -> i64 {
        (1 + self.max_x - self.min_x) * (1 + self.max_y - self.min_y) * (1 + self.max_z - self.min_z)
    }
}

struct Instruction {
    on: bool,
    min_x: i64,
    max_x: i64,
    min_y: i64,
    max_y: i64,
    min_z: i64,
    max_z: i64
}

fn is_small(ins: &Instruction) -> bool {
    ins.min_x >= -50 && ins.max_x <= 50 && ins.min_y >= -50 && ins.max_y <= 50 && ins.min_z >= -50 && ins.max_z <= 50
}

fn parse_line_to_instruction(str: &str) -> Instruction {
    let re = Regex::new(r"^(on|off) x=([\-0-9]*)..([\-0-9]*),y=([\-0-9]*)..([\-0-9]*),z=([\-0-9]*)..([\-0-9]*)$").unwrap();
    let cap = re.captures_iter(str).next().unwrap();
    //println!("On: {} Xm: {} X: {} Ym: {} Y: {} Zm: {} Z: {}", &cap[1], &cap[2], &cap[3], &cap[4], &cap[5], &cap[6], &cap[7]);
    Instruction {
        on: &cap[1] == "on",
        min_x: str::parse::<i64>(&cap[2]).unwrap(),
        max_x: str::parse::<i64>(&cap[3]).unwrap(),
        min_y: str::parse::<i64>(&cap[4]).unwrap(),
        max_y: str::parse::<i64>(&cap[5]).unwrap(),
        min_z: str::parse::<i64>(&cap[6]).unwrap(),
        max_z: str::parse::<i64>(&cap[7]).unwrap(),
    }
}

struct Grid3d {
    top_left_near: Vec<Vec<Vec<bool>>>,
    top_right_near: Vec<Vec<Vec<bool>>>,
    bottom_left_near: Vec<Vec<Vec<bool>>>,
    bottom_right_near: Vec<Vec<Vec<bool>>>,
    top_left_far: Vec<Vec<Vec<bool>>>,
    top_right_far: Vec<Vec<Vec<bool>>>,
    bottom_left_far: Vec<Vec<Vec<bool>>>,
    bottom_right_far: Vec<Vec<Vec<bool>>>
}

impl Grid3d {
    fn new() -> Grid3d {
        Grid3d {
            top_left_near: vec![vec![vec![false; 1]; 1]; 1],
            top_right_near: vec![vec![vec![false; 1]; 1]; 1],
            bottom_left_near: vec![vec![vec![false; 1]; 1]; 1],
            bottom_right_near: vec![vec![vec![false; 1]; 1]; 1],
            top_left_far: vec![vec![vec![false; 1]; 1]; 1],
            top_right_far: vec![vec![vec![false; 1]; 1]; 1],
            bottom_left_far: vec![vec![vec![false; 1]; 1]; 1],
            bottom_right_far: vec![vec![vec![false; 1]; 1]; 1],
        }
    }

    fn set(&mut self, b: bool, x: i64, y: i64, z: i64) {
        let xx = x.abs() as usize;
        let yy = y.abs() as usize;
        let zz = z.abs() as usize;
        if x >= 0 && y >= 0 && z >= 0 {
            // bottom right near
            resize_grid_vec(&mut self.bottom_right_near, xx, yy, zz);
            self.bottom_right_near[xx][yy][zz] = b;
        } else if x < 0 && y >= 0 && z >= 0 {
            // bottom left near
            resize_grid_vec(&mut self.bottom_left_near, xx, yy, zz);
            self.bottom_left_near[xx][yy][zz] = b;
        } else if x >= 0 && y < 0 && z >= 0 {
            // top right near
            resize_grid_vec(&mut self.top_right_near, xx, yy, zz);
            self.top_right_near[xx][yy][zz] = b;
        } else if x < 0 && y < 0 && z >= 0 {
            // top left near
            resize_grid_vec(&mut self.top_left_near, xx, yy, zz);
            self.top_left_near[xx][yy][zz] = b;
        } else if x >= 0 && y >= 0 && z < 0 {
            // bottom right far
            resize_grid_vec(&mut self.bottom_right_far, xx, yy, zz);
            self.bottom_right_far[xx][yy][zz] = b;
        } else if x < 0 && y >= 0 && z < 0 {
            // bottom left far
            resize_grid_vec(&mut self.bottom_left_far, xx, yy, zz);
            self.bottom_left_far[xx][yy][zz] = b;
        } else if x >= 0 && y < 0 && z < 0 {
            // top right far
            resize_grid_vec(&mut self.top_right_far, xx, yy, zz);
            self.top_right_far[xx][yy][zz] = b;
        } else if x < 0 && y < 0 && z < 0 {
            // top left far
            resize_grid_vec(&mut self.top_left_far, xx, yy, zz);
            self.top_left_far[xx][yy][zz] = b;
        }
    }

    fn get(&mut self, x: i64, y: i64, z: i64) -> bool {
        let xx = x.abs() as usize;
        let yy = y.abs() as usize;
        let zz = z.abs() as usize;

        if x >= 0 && y >= 0 && z >= 0 {
            // bottom right near
            resize_grid_vec(&mut self.bottom_right_near, xx, yy, zz);
            return self.bottom_right_near[xx][yy][zz];
        } else if x < 0 && y >= 0 && z >= 0 {
            // bottom left near
            resize_grid_vec(&mut self.bottom_left_near, xx, yy, zz);
            return self.bottom_left_near[xx][yy][zz];
        } else if x >= 0 && y < 0 && z >= 0 {
            // top right near
            resize_grid_vec(&mut self.top_right_near, xx, yy, zz);
            return self.top_right_near[xx][yy][zz];
        } else if x < 0 && y < 0 && z >= 0 {
            // top left near
            resize_grid_vec(&mut self.top_left_near, xx, yy, zz);
            return self.top_left_near[xx][yy][zz];
        } else if x >= 0 && y >= 0 && z < 0 {
            // bottom right far
            resize_grid_vec(&mut self.bottom_right_far, xx, yy, zz);
            return self.bottom_right_far[xx][yy][zz];
        } else if x < 0 && y >= 0 && z < 0 {
            // bottom left far
            resize_grid_vec(&mut self.bottom_left_far, xx, yy, zz);
            return self.bottom_left_far[xx][yy][zz];
        } else if x >= 0 && y < 0 && z < 0 {
            // top right far
            resize_grid_vec(&mut self.top_right_far, xx, yy, zz);
            return self.top_right_far[xx][yy][zz];
        } else if x < 0 && y < 0 && z < 0 {
            // top left far
            resize_grid_vec(&mut self.top_left_far, xx, yy, zz);
            return self.top_left_far[xx][yy][zz];
        }
        return false;
    }
    fn count_trues(&self) -> u64 {
        count_trues(&self.bottom_left_near)
            + count_trues(&self.bottom_right_near)
            + count_trues(&self.top_left_near)
            + count_trues(&self.top_right_near)
            + count_trues(&self.bottom_left_far)
            + count_trues(&self.bottom_right_far)
            + count_trues(&self.top_left_far)
            + count_trues(&self.top_right_far)
    }
}

fn resize_grid_vec(arr: &mut Vec<Vec<Vec<bool>>>, xx: usize, yy: usize, zz: usize) {
    if arr.len() <= xx {
        arr.resize(xx + 1, Vec::new());
    }
    if arr[xx].len() <= yy {
        arr[xx].resize(yy + 1, Vec::new());
    }
    if arr[xx][yy].len() <= zz {
        arr[xx][yy].resize(zz + 1, false);
    }
}
fn count_trues(list: &Vec<Vec<Vec<bool>>>) -> u64 {
    let mut count = 0;
    for x in 0..list.len() {
        for y in 0..list[x].len() {
            for z in 0..list[x][y].len() {
                if list[x][y][z] {
                    count += 1;
                }
            }
        }
    }
    count
}