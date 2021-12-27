//  This took a very long time to run.  But it worked eventually...

fn main() {
    let corridor = [0,0,0,0,0,0,0,0,0,0,0];
    let mut a_room = Room::from(1, 2, true);
    let mut b_room = Room::from(2, 4, true);
    let mut c_room = Room::from(3, 6, true);
    let mut d_room = Room::from(4, 8, true);

    //let mut a_room = Room::from(1, 2);
    //let mut b_room = Room::from(2, 4);
    //let mut c_room = Room::from(3, 6);
    //let mut d_room = Room::from(4, 8);


    // setup
    //a_room.deeper = char_to_num('A');
    //a_room.near_entrance = char_to_num('B');
    //b_room.deeper = char_to_num('D');
    //b_room.near_entrance = char_to_num('C');
    //c_room.deeper = char_to_num('C');
    //c_room.near_entrance = char_to_num('B');
    //d_room.deeper = char_to_num('A');
    //d_room.near_entrance = char_to_num('D');

    a_room.deeper = char_to_num('C');
    a_room.near_entrance = char_to_num('B');
    b_room.deeper = char_to_num('D');
    b_room.near_entrance = char_to_num('A');
    c_room.deeper = char_to_num('D');
    c_room.near_entrance = char_to_num('B');
    d_room.deeper = char_to_num('A');
    d_room.near_entrance = char_to_num('C');

    a_room.extra_near = char_to_num('D');
    a_room.extra_deeper = char_to_num('D');
    b_room.extra_near = char_to_num('C');
    b_room.extra_deeper = char_to_num('B');
    c_room.extra_near = char_to_num('B');
    c_room.extra_deeper = char_to_num('A');
    d_room.extra_near = char_to_num('A');
    d_room.extra_deeper = char_to_num('C');

    let mut w = Worker::new();

    w.go_for_it(State { corridor: corridor, rooms: [a_room, b_room, c_room, d_room], energy: 0, worthy: 0, potential_energy: 0 });
}


#[derive(Clone, PartialEq, Debug)]
struct State {
    corridor: [i32; 11], rooms: [Room; 4], energy: u64, worthy: i32, potential_energy: u64
}

impl State {
    fn count_occupants(&self) -> i32 {
        self.corridor.iter().fold(0, |iter, item| iter + one_if_true(*item > 0))
        + self.rooms[0].count_occupants() + self.rooms[1].count_occupants() + self.rooms[2].count_occupants() + self.rooms[3].count_occupants()
    }

    fn minimum_completion(&self) -> u64 {
        let mut score: u64 = 0;
        for i in self.corridor {
            if i > 0 {
                score += ((i - corridor_location_for_room(i) as i32).abs() as u64 * energy_for_char(i)) as u64;
            }
        }
        for room in &self.rooms {
            if room.near_entrance != room.room_type {
                score += ((room.corridor_location - corridor_location_for_room(room.near_entrance)).abs() as u64 * energy_for_char(room.near_entrance)) as u64;
            }
            if room.deeper != room.room_type {
                score += ((room.corridor_location - corridor_location_for_room(room.deeper)).abs() as u64 * energy_for_char(room.deeper)) as u64;
            }
        }
        score
    }

}

fn is_available_stop(from: i8, to: i8, corridor: &[i32; 11]) -> bool {
    let mut range = from+1..to+1;
    if to < from {
        range = to..from;
    }
    //println!("{} {}", from, to);
    for pos in range {
        //println!("{}", pos);
        if corridor[pos as usize] != 0 {
            //println!("{} {:?}", pos, corridor);
            return false;
        }
    }
    true
}

struct Worker {
    best_score: u64,
    beens: Vec<State>
}

impl Worker {
    fn new() -> Worker {
        Worker { best_score: 0, beens: Vec::new() }
    }

    fn go_for_it(&mut self, state: State) {
        let mut options = vec![state];
        loop {
            if options.len() == 0 { break; }
            let option = options.remove(0);
            if options.len() % 50 == 0 {
                println!("option {} {} {} {}", options.len(), self.best_score, option.energy, option.worthy);
            }
            options.append(&mut self.get_options(option).to_vec());
            options.sort_by(|item, item2| item.potential_energy.cmp(&item2.potential_energy));
            options.sort_by(|item, item2| item2.worthy.cmp(&item.worthy));
            if options.len() > 1000 {
                //break;
            }
        }
        //println!("{:?}", options);
        println!("{}", self.best_score);
    }

    fn get_options(&mut self, state: State) -> Vec<State> {
        if self.beens.contains(&state) {
            //println!("Been here before");
            return Vec::new();
        }
        if self.best_score != 0 && state.potential_energy > self.best_score {
            return Vec::new();
        }
        self.beens.push(state.clone());
        if state.rooms.iter().fold(true, |iter, item| iter && item.is_complete()) {
            println!("complete");
            if self.best_score == 0 || self.best_score > state.energy {
                println!("great");
                self.best_score = state.energy;
            }
            return Vec::new();
        }
        let mut states = Vec::new();
        // For each room, is there anything that can come out? Can it go anywhere?
        let mut room_i = 0;
        for room in &state.rooms {
            room_i += 1;
            if room.is_complete() { continue; }
            if room.is_ready_for_end() { continue; }
            let coming_out = room.first_one_out();
            if coming_out == None { continue; }
            let coming_out = coming_out.unwrap();
            for i in 0..=10 { 
                //println!("cycling");
                if is_available_stop(room.corridor_location, i, &state.corridor) && i != 2 && i != 4 && i != 6 && i != 8
                {
                    //println!("Moving {} from {} to {} {} {:?}", coming_out.char, room.corridor_location, i, room_i, state);
                    let new_energy = state.energy + ((coming_out.moves + (i as i32 - room.corridor_location as i32).abs()) as u64 * energy_for_char(coming_out.char));
                    let mut new_state = state.clone();
                    if new_state.corridor.iter().filter(|item| **item == coming_out.char).count() > 1 {
                        //println!("Already done {:?}", new_state.corridor);
                    }
                    new_state.corridor[i as usize] = coming_out.char;
                    new_state.rooms[room_i - 1].remove_first_one_out();
                    new_state.energy = new_energy;
                    new_state.potential_energy = new_energy + new_state.minimum_completion();
                    new_state.worthy = new_state.rooms.iter().fold(0, |iter, item| iter + one_if_true(item.is_ready_for_end()) + one_if_true(item.is_complete()) * 2);
                    //println!("A {} {}", new_state.count_occupants(), room_i);
                    //if new_state.count_occupants() != 8 {
                    //    println!("Moving {:?}", new_state);
                    //    panic!("AAAAAA");
                   // }
                    if self.best_score == 0 || new_state.potential_energy < self.best_score  {
                        states.push(new_state);
                    }
                }
            }
            if state.rooms[coming_out.char as usize - 1].is_ready_for_end() {
                if is_available_stop(room.corridor_location, state.rooms[coming_out.char as usize - 1].corridor_location, &state.corridor) {
                    //println!("p");
                    let new_energy = state.energy + ((state.rooms[coming_out.char as usize - 1].moves_to_in() + (state.rooms[coming_out.char as usize - 1].corridor_location as i32 - room.corridor_location as i32).abs() + coming_out.moves) as u64 * energy_for_char(coming_out.char));
                    let mut new_state = state.clone();
                    new_state.rooms[room_i - 1].remove_first_one_out();
                    new_state.rooms[coming_out.char as usize - 1].put_in(coming_out.char);
                    new_state.energy = new_energy;
                    new_state.potential_energy = new_energy + new_state.minimum_completion();
                    new_state.worthy = new_state.rooms.iter().fold(0, |iter, item| iter + one_if_true(item.is_ready_for_end()) + one_if_true(item.is_complete()) * 2);
                    //println!("B {}", new_state.count_occupants());
                    if self.best_score == 0 || new_state.potential_energy < self.best_score {
                        states.push(new_state);
                    }
                }
            }
        }

        // For each corridor location, is there anything in there? Can it go to its destination?
        for i in 0..=10 {
            if state.corridor[i] > 0 { 
                //println!("m");
                let char = state.corridor[i];
                let r = state.rooms.iter().find(|item| item.room_type == char);
                if r.is_some() { 
                    let r = r.unwrap();
                    //println!("s {} {:?} {:?}", char, r, state.corridor);
                    if r.is_ready_for_end() {
                        //println!("e {} {}", i, r.corridor_location);
                        if is_available_stop(i as i8, r.corridor_location, &state.corridor) {
                            //println!("p");
                            let new_energy = state.energy + ((r.moves_to_in() + (i as i32 - r.corridor_location as i32).abs()) as u64 * energy_for_char(char));
                            let mut new_state = state.clone();
                            new_state.corridor[i] = 0;
                            new_state.rooms[char as usize - 1].put_in(char);
                            new_state.energy = new_energy;
                            new_state.potential_energy = new_energy + new_state.minimum_completion();
                            new_state.worthy = new_state.rooms.iter().fold(0, |iter, item| iter + one_if_true(item.is_ready_for_end()) + one_if_true(item.is_complete()) * 2);
                            //println!("B {}", new_state.count_occupants());
                            if self.best_score == 0 || new_state.potential_energy < self.best_score {
                                states.push(new_state);
                            }
                        }
                    }
                }
            }
        }
        states
    }
}

fn one_if_true(t: bool) -> i32 {
    if t == true { return 1; }
    0
}

#[derive(Clone, PartialEq, Debug)]
struct Room {
    room_type: i32,
    corridor_location: i8,
    near_entrance: i32,
    extra_near: i32,
    extra_deeper: i32,
    deeper: i32,
    part_two: bool
}

impl Room {
    fn from(room_type: i32, corridor_location: i8, part_two: bool) -> Room {
        Room { room_type, corridor_location, near_entrance: 0, deeper: 0, extra_deeper: 0, extra_near: 0, part_two }
    }

    fn is_complete(&self) -> bool {
        //println!("{} {} {} {}", self.room_type == self.near_entrance && self.room_type == self.deeper, self.room_type, self.near_entrance, self.deeper);
        if self.part_two {
            self.room_type == self.near_entrance && self.room_type == self.deeper && self.room_type == self.extra_near && self.room_type == self.extra_deeper
        } else {
            self.room_type == self.near_entrance && self.room_type == self.deeper
        }
    }

    fn is_empty(&self) -> bool {
        if self.part_two {
            self.near_entrance == 0 && self.deeper == 0 && self.extra_near == 0 && self.extra_deeper == 0
        } else {
            self.near_entrance == 0 && self.deeper == 0
        }
    }

    fn is_ready_for_end(&self) -> bool {
        //println!("{} {} {} {}", (self.room_type == self.near_entrance || self.near_entrance == 0) && (self.room_type == self.deeper || self.deeper == 0), self.room_type, self.near_entrance, self.deeper);
        if self.part_two {
            (self.room_type == self.near_entrance || self.near_entrance == 0) &&
            (self.room_type == self.deeper || self.deeper == 0) &&
            (self.room_type == self.extra_near || self.extra_near == 0) &&
            (self.room_type == self.extra_deeper || self.extra_deeper == 0)
    
        } else {
            (self.room_type == self.near_entrance || self.near_entrance == 0) &&
            (self.room_type == self.deeper || self.deeper == 0)
        }
    }

    fn count_occupants(&self) -> i32 {
        one_if_true(self.deeper > 0) + one_if_true(self.near_entrance > 0) + one_if_true(self.extra_near > 0) + one_if_true(self.extra_deeper > 0)
    }

    fn first_one_out(&self) -> Option<Out> {
        if self.near_entrance != 0 {
            return Some(Out { char: self.near_entrance, moves: 1 });
        } else if self.part_two && self.extra_near != 0 {
            return Some(Out { char: self.extra_near, moves: 2 });
        } else if self.part_two && self.extra_deeper != 0 {
            return Some(Out { char: self.extra_deeper, moves: 3 });
        } else if self.deeper != 0 {
            if self.part_two {
                return Some(Out { char: self.deeper, moves: 4 });
            } else {
                return Some(Out { char: self.deeper, moves: 2 });
            }
        }
        None
    }
    fn remove_first_one_out(&mut self) {
        if self.part_two {
            if self.near_entrance != 0 {
                self.near_entrance = 0;
            } else if self.extra_near != 0 {
                self.extra_near = 0;
            } else if self.extra_deeper != 0 {
                self.extra_deeper = 0;
            } else if self.deeper != 0 {
                self.deeper = 0;
            }
        } else {
            if self.near_entrance != 0 {
                self.near_entrance = 0;
            } else if self.deeper != 0 {
                self.deeper = 0;
            }

        }
    }
    fn moves_to_in(&self) -> i32 {
        if self.part_two {
            if self.deeper == 0 {
                return 4;
            } else if self.extra_deeper == 0 {
                return 3;
            } else if self.extra_near == 0 {
                return 2;
            } else if self.near_entrance == 0 {
                return 1;
            }
        } else {
            if self.deeper == 0 {
                return 2;
            } else if self.near_entrance == 0 {
                return 1;
            }
        }
        0
    }
    fn put_in(&mut self, char: i32) {
        if self.part_two {
            if self.deeper == 0 {
                self.deeper = char;
            } else if self.extra_deeper == 0 {
                self.extra_deeper = char;
            } else if self.extra_near == 0 {
                self.extra_near = char;
            } else if self.near_entrance == 0 {
                self.near_entrance = char;
            }
        } else {
            if self.deeper == 0 {
                self.deeper = char;
            } else if self.near_entrance == 0 {
                self.near_entrance = char;
            }
        }
    }
}

#[derive(PartialEq)]
struct Out {
    char: i32,
    moves: i32
}

fn char_to_num(c: char) -> i32 {
    match c {
        'A' => 1,
        'B' => 2,
        'C' => 3,
        'D' => 4,
        _ => 0
    }
}

fn energy_for_char(c: i32) -> u64 {
    match c {
        1 => 1,
        2 => 10,
        3 => 100,
        4 => 1000,
        _ => 0
    }
}

fn corridor_location_for_room(c: i32) -> i8 {
    match c {
        1 => 2,
        2 => 4,
        3 => 6,
        4 => 8,
        _ => 0
    }
}