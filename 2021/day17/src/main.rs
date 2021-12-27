use cached::proc_macro::cached;

// this takes a long time and I would have liked to spend more time making it more efficient

fn main() {
    let mut v = VelocityFinder::from(257, 286, -101, -57);
    v.find_paths();
}

struct State {
    current_position: Coord,
    current_velocity: Coord,
    original_velocity: Coord,
    highest_so_far: i32
}
        

struct VelocityFinder {
    min_x: i32,
    max_x: i32,
    min_y: i32,
    max_y: i32,
    highest_velocity: (i32, Coord),
    done_count: i32
}
impl VelocityFinder {
    fn from(min_x: i32, max_x: i32, min_y: i32, max_y: i32) -> VelocityFinder {
        VelocityFinder {
            min_x, max_x, min_y, max_y,
            highest_velocity: (0, Coord::from(0,0)),
            done_count: 0
        }
    }

    fn find_paths(&mut self) {
        // current position, current velocity, original velocity, highestsofar
        let mut to_do: Vec<State> = Vec::new();
        // min y for our loop is max y for the target
        //  max y is abs of that
        // min x for our loop is 0 and max is the min x for the target
        for x in 1..=self.max_x {
            for y in self.min_y..=self.min_y.abs() {
                    to_do.push(State { current_position: Coord::from(0,0), current_velocity: Coord::from(x, y), original_velocity: Coord::from(x,y), highest_so_far: 0 });
            }
        }
        to_do.reverse();

        loop {
            if to_do.len() == 0 { break; }
            let item = to_do.remove(0);
            //println!("Doing {}, {:?}", to_do.len(), item);
            let last_x = item.current_position.x;
            let last_y = item.current_position.y;
            let new = move_one_step(item.current_position, item.current_velocity, item.highest_so_far);
            if new.2 < item.highest_so_far && self.highest_velocity.0 > new.2 && self.highest_velocity.1 != item.original_velocity {
                continue;
            }
            if last_x == new.0.x && last_y < new.0.y {
                //println!("giving up {}", to_do.len());
                //continue;
            }
            if new.0.x <= self.max_x && new.0.y >= self.min_y {
                // Path is still before the target

                // If we are in target, mark it down
                if new.0.x >= self.min_x && new.0.x <= self.max_x && new.0.y >= self.min_y && new.0.y <= self.max_y {
                    println!("Done: {}", self.done_count);
                    self.done_count += 1;
                    if self.highest_velocity.0 < new.2 {
                        self.highest_velocity = (new.2, item.original_velocity);
                    }
                } else {
                    to_do.push(State { current_position: new.0, current_velocity: new.1, original_velocity: item.original_velocity, highest_so_far: new.2 });
                }
            }
            to_do.sort_by(|item, item2| item2.highest_so_far.cmp(&item.highest_so_far));
        }
        println!("{:?}", self.highest_velocity);
        println!("{}", self.done_count);
    }
}
#[derive(Debug, PartialEq, Hash, Eq, Clone)]
struct Coord {
    x: i32,
    y: i32,
}
impl Coord {
    fn from(x: i32, y: i32) -> Coord {
        Coord { x, y }
    }
}

#[cached]
fn move_one_step(coord: Coord, current_velocity: Coord, high_point: i32) -> (Coord, Coord, i32) {
    let mut new_high_point = high_point;
    let new_coord = Coord::from(coord.x + current_velocity.x, coord.y + current_velocity.y);
    if high_point < new_coord.y {
        new_high_point = new_coord.y;
    }
    let mut new_vel_x = current_velocity.x;
    if new_vel_x > 0 {
        new_vel_x -= 1;
    } else if new_vel_x < 0 {
        new_vel_x += 1;
    }
    (new_coord, Coord::from(new_vel_x, current_velocity.y - 1), new_high_point)
}