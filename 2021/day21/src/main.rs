use std::collections::HashMap;

fn main() {
    let dice = DeterministicDice::new();

    let one_initial_position = 4;
    let two_initial_position = 6;
    
    let mut dice_roll = 0;
    let mut one_position = one_initial_position;
    let mut one_score: i64 = 0;
    let mut two_position = two_initial_position;
    let mut two_score: i64 = 0;

    loop {
        dice_roll += 1;
        let roll = dice.get_three_rolls(dice_roll);
        one_position = get_new_position(one_position, roll);
        one_score += one_position as i64;
        if one_score >= 1000 { break; }
        
        dice_roll += 1;
        let roll = dice.get_three_rolls(dice_roll);
        two_position = get_new_position(two_position, roll);
        two_score += two_position as i64;
        if two_score >= 1000 { break; }
    }
    if one_score >= 1000 {
        println!("One wins; two score is {}", two_score as usize * dice_roll * 3);
    } else {
        println!("Two wins; one score is {}", one_score as usize * dice_roll * 3);
    }

    let mut p2 = PartTwo::new();
    let w1 = p2.play(4, 0, 6, 0, 1, 1, 1);
    let w2 = p2.play(4, 0, 6, 0, 2, 1, 1);
    let w3 = p2.play(4, 0, 6, 0, 3, 1, 1);
    let w = Wins {
        one: w1.one + w2.one + w3.one,
        two: w1.two + w2.two + w3.two
    };
    println!("{:?}", w);
}

#[derive(Clone, Debug)]
struct Wins {
    one: u64,
    two: u64
}

struct PartTwo {
    cache: HashMap<GameCache, Wins>
}

impl PartTwo {
    fn new() -> PartTwo {
        PartTwo { cache: HashMap::new() }
    }

    fn play(&mut self, one_pos: i8, one_score: i64, two_pos: i8, two_score: i64, current_roll: u8, current_player: i8, roll_index: i8) -> Wins {
        let c = GameCache {
            one_pos, one_score, two_pos, two_score, current_roll, current_player, roll_index
        };
        if self.cache.contains_key(&c) {
            return self.cache[&c].clone();
        }
        let mut current_player = current_player;
        let mut one_pos = one_pos;
        let mut one_score = one_score;
        let mut two_pos = two_pos;
        let mut two_score = two_score;
        let mut roll_index = roll_index;

        if current_player == 1 {
            one_pos = get_new_position(one_pos, current_roll as i16);
            if roll_index == 3 {
                one_score = one_score + one_pos as i64;
                if one_score >= 21 {
                    let w = Wins { one: 1, two: 0 };
                    self.cache.insert(c, w.clone());
                    return w;
                }
                current_player += 1;
                if current_player == 2 {
                    current_player = 0;
                }        
            }
        } else {
            two_pos = get_new_position(two_pos, current_roll as i16);
            if roll_index == 3 {
                two_score = two_score + two_pos as i64;
                if two_score >= 21 {
                    let w = Wins { one: 0, two: 1 };
                    self.cache.insert(c, w.clone());
                    return w;
                }
                current_player += 1;
                if current_player == 2 {
                    current_player = 0;
                }
        
            }
        }
        roll_index += 1;
        if roll_index == 4 {
            roll_index = 1;
        }
        let w1 = self.play(one_pos, one_score, two_pos, two_score, 1, current_player, roll_index);
        let w2 = self.play(one_pos, one_score, two_pos, two_score, 2, current_player, roll_index);
        let w3 = self.play(one_pos, one_score, two_pos, two_score, 3, current_player, roll_index);
        let w = Wins {
            one: w1.one + w2.one + w3.one,
            two: w1.two + w2.two + w3.two
        };
        self.cache.insert(c, w.clone());
        w
    }
}


#[derive(PartialEq, Eq, Hash)]
struct GameCache {
    one_pos: i8,
    one_score: i64,
    two_pos: i8,
    two_score: i64,
    current_roll: u8,
    current_player: i8,
    roll_index: i8
}

struct DeterministicDice {
    dice: Vec<i16>
}

impl DeterministicDice {
    fn new() -> DeterministicDice {
        let mut v = Vec::new();
        for i in 1..=100 {
            v.push(i);
        }
        DeterministicDice {
            dice: v
        }
    }

    fn get_roll(&self, i: usize) -> i16 {
        let mut i = i;
        if i > 100 {
            i = i % 100;
        }
        if i == 0 {
            i = 100;
        }
        self.dice[i-1]
    }

    fn get_three_rolls(&self, i: usize) -> i16 {
        let i = (i - 1) * 3 + 1;
        self.get_roll(i) + self.get_roll(i + 1) + self.get_roll(i + 2)
    }
}

fn get_new_position(old_position: i8, moves: i16) -> i8 {
    let mut moved_to = old_position as i16 + moves;
    if moved_to > 10 {
        moved_to = moved_to % 10;
    }
    if moved_to == 0 {
        moved_to = 10;
    }
    //println!("{}", moved_to);
    moved_to as i8
}