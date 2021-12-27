fn main() {
    let str = include_str!("input.txt");
    let mut lines = str.lines();

    let mut s = SnailNumbers::new();
    s.parse(lines.next().unwrap());
    s.explode_and_split();
    let mut res = s.to_string();
    let mut mag = 0;
    for line in lines {
        let mut s = SnailNumbers::by_adding(res.as_str(), line);
        s.explode_and_split();
        res = s.to_string();
        mag = s.get_magnitude();
        println!("{:?}", s.to_string());
    }
    println!("{:?}", mag);


    let str = include_str!("input.txt");
    let lines = str.lines();
    let mut mag = 0;
    for line in lines {
        let lines2 = str.lines();
        for line2 in lines2 {
            if line == line2 { continue; }
            let mut s = SnailNumbers::by_adding(line, line2);
            s.explode_and_split();
            let this_mag = s.get_magnitude();
            if this_mag > mag {
                mag = this_mag;
            }
        }
    }
    println!("{:?}", mag);


    
}
#[derive(Debug)]
struct SnailNumbers {
    all_numbers: Vec<SnailNumber>,
}
#[derive(PartialEq)]
enum LeftRight {
    Left,
    Right,
}

impl SnailNumbers {
    fn new() -> SnailNumbers {
        SnailNumbers {
            all_numbers: Vec::new(),
        }
    }

    fn by_adding(one: &str, two: &str) -> SnailNumbers {
        let mut s = SnailNumbers::new();
        s.parse(format!("[{},{}]", one, two).as_str());
        s
    }

    fn parse(&mut self, str: &str) {
        let v = self.parse_string_to_number(&str[1..str.len()]);
        //println!("{:?}", v);
        let i = self.push(v.0);
        self.assign_parent_ids(i);
        //println!("{}", self.to_string());
    }

    fn assign_parent_ids(&mut self, i: usize) {
        let item = &self.all_numbers[i];
        if item.left != None {
            let item_i = item.left.unwrap();
            self.all_numbers[item_i].parent = Some(i);
            self.assign_parent_ids(self.all_numbers[item_i].index)
        }
        let item = &self.all_numbers[i];
        if item.right != None {
            let item_i = item.right.unwrap();
            self.all_numbers[item_i].parent = Some(i);
            self.assign_parent_ids(self.all_numbers[item_i].index)
        }
    }

    fn to_string(&self) -> String {
        self.number_to_string(self.find_top_level())
    }

    fn number_to_string(&self, i: usize) -> String {
        if self.all_numbers[i].is_pair {
            return format!(
                "[{},{}]",
                self.number_to_string(self.all_numbers[i].left.unwrap()),
                self.number_to_string(self.all_numbers[i].right.unwrap())
            );
        } else {
            return format!("{}", self.all_numbers[i].val);
        }
    }

    fn get_magnitude(&self) -> u64 {
        let i = self.find_top_level();
        self.get_magnitude_for_item(i)
    }

    fn get_magnitude_for_item(&self, i: usize) -> u64 {
        if self.all_numbers[i].is_pair {
            return (3 * self.get_magnitude_for_item(self.all_numbers[i].left.unwrap())) + (2 * self.get_magnitude_for_item(self.all_numbers[i].right.unwrap()));
        } else {
            return self.all_numbers[i].val as u64;
        }
    }

    fn explode_and_split(&mut self) {
        loop {
            if !self.explode() && !self.split() {
                break;
            }
        }
    }

    fn explode(&mut self) -> bool {
        let o = self.find_first_to_explode();
        if o != None {
            //println!("Explode");
            let o = o.unwrap();
            let left = self.find_sibling_to_left(o);
            let right = self.find_sibling_to_right(o);
            if left != None {
                self.all_numbers[left.unwrap()].val +=
                    self.all_numbers[self.all_numbers[o].left.unwrap()].val;
            }
            if right != None {
                self.all_numbers[right.unwrap()].val +=
                    self.all_numbers[self.all_numbers[o].right.unwrap()].val;
            }
            self.all_numbers[o].is_pair = false;
            self.all_numbers[o].left = None;
            self.all_numbers[o].right = None;
            self.all_numbers[o].val = 0;
            return true;
        }
        false
    }
    fn split(&mut self) -> bool {
        let o = self.find_first_to_split();
        if o != None {
            //println!("Split");
            let o = o.unwrap();
            let v = self.all_numbers[o].val as f64;
            let left = f64::floor(v / 2.0);
            let mut left_item = SnailNumber::from_number(left as u32);
            left_item.parent = Some(o);
            let right = f64::ceil(v / 2.0);
            let mut right_item = SnailNumber::from_number(right as u32);
            right_item.parent = Some(o);
            self.all_numbers[o].val = 0;
            self.all_numbers[o].is_pair = true;
            self.all_numbers[o].left = Some(self.push(left_item));
            self.all_numbers[o].right = Some(self.push(right_item));
            return true;
        }
        false
    }

    fn find_first_to_split(&self) -> Option<usize> {
        self.find_split(self.find_top_level())
    }

    fn find_top_level(&self) -> usize {
        self
            .all_numbers
            .iter()
            .find(|i| i.parent == None)
            .unwrap()
            .index
    }

    fn find_split(&self, i: usize) -> Option<usize> {
        if !self.all_numbers[i].is_pair {
            if self.all_numbers[i].val >= 10 {
                return Some(i);
            }
        } else {
            let left = self.find_split(self.all_numbers[i].left.unwrap());
            if left != None {
                return left;
            }
            let right = self.find_split(self.all_numbers[i].right.unwrap());
            if right != None {
                return right;
            }
        }
        None
    }

    fn find_first_to_explode(&self) -> Option<usize> {
        self.find_depth(self.find_top_level(), 0)
    }

    fn find_depth(&self, i: usize, depth: u8) -> Option<usize> {
        if depth == 5 {
            return Some(self.all_numbers[i].parent.unwrap());
        } else {
            if self.all_numbers[i].left != None {
                let left = self.find_depth(self.all_numbers[i].left.unwrap(), depth + 1);
                if left != None {
                    return left;
                }
                let right = self.find_depth(self.all_numbers[i].right.unwrap(), depth + 1);
                if right != None {
                    return right;
                }
            }
        }
        None
    }

    fn find_sibling_to_left(&self, i: usize) -> Option<usize> {
        // Look at parent. Find out if this was the left or right
        let parent_index = self.all_numbers[i].parent;
        if parent_index != None {
            let parent_index = parent_index.unwrap();
            // If this was the left, repeat
            if self.all_numbers[parent_index].left.unwrap() == i {
                return self.find_sibling_to_left(parent_index);
            } else {
                // If this was the right, look at the left: if it is a number, use that
                if !self.all_numbers[self.all_numbers[parent_index].left.unwrap()].is_pair {
                    return Some(
                        self.all_numbers[self.all_numbers[parent_index].left.unwrap()].index,
                    );
                } else {
                    // If not, get the far right
                    return Some(self.far_right_number(
                        self.all_numbers[self.all_numbers[parent_index].left.unwrap()].index,
                    ));
                }
            }
        }
        None
    }

    fn find_sibling_to_right(&self, i: usize) -> Option<usize> {
        // Look at parent. Find out if this was the left or right
        let parent_index = self.all_numbers[i].parent;
        if parent_index != None {
            let parent_index = parent_index.unwrap();
            // If this was the right, repeat
            if self.all_numbers[parent_index].right.unwrap() == i {
                return self.find_sibling_to_right(parent_index);
            } else {
                // If this was left, look ath the right: if it is a number, use that
                if !self.all_numbers[self.all_numbers[parent_index].right.unwrap()].is_pair {
                    return Some(
                        self.all_numbers[self.all_numbers[parent_index].right.unwrap()].index,
                    );
                } else {
                    // If not, get the far left
                    return Some(self.far_left_number(
                        self.all_numbers[self.all_numbers[parent_index].right.unwrap()].index,
                    ));
                }
            }
        }
        None
    }

    fn far_right_number(&self, i: usize) -> usize {
        if self.all_numbers[i].is_pair {
            return self.far_right_number(self.all_numbers[i].right.unwrap());
        } else {
            return self.all_numbers[i].index;
        }
    }

    fn far_left_number(&self, i: usize) -> usize {
        if self.all_numbers[i].is_pair {
            return self.far_left_number(self.all_numbers[i].left.unwrap());
        } else {
            return self.all_numbers[i].index;
        }
    }

    fn parse_string_to_number(&mut self, str: &str) -> (SnailNumber, Vec<char>) {
        let mut me = SnailNumber::empty();
        me.is_pair = true;
        let mut position = LeftRight::Left;
        //let me_index = self.push(me);
        let mut chars: Vec<char> = str.chars().collect();
        loop {
            if chars.len() == 0 {
                break;
            }
            let first = chars.remove(0);
            if first.is_numeric() {
                let v = SnailNumber::from_number(first.to_digit(10).unwrap());
                let v = self.push(v);
                if position.eq(&LeftRight::Left) {
                    me.left = Some(v);
                } else {
                    me.right = Some(v);
                }
            } else if first == '[' {
                let v = self.parse_string_to_number(String::from_iter(chars).as_str());
                chars = v.1;
                let v = self.push(v.0);
                if position.eq(&LeftRight::Left) {
                    me.left = Some(v);
                } else {
                    me.right = Some(v);
                }
            } else if first == ',' {
                position = LeftRight::Right;
            } else if first == ']' {
                return (me, chars.clone());
            }
        }
        (me, chars.clone())
    }

    fn push(&mut self, s: SnailNumber) -> usize {
        self.all_numbers.push(s);
        let i = self.all_numbers.len() - 1;
        self.all_numbers[i].index = i;
        i
    }
}

#[derive(Debug)]
struct SnailNumber {
    is_pair: bool,
    left: Option<usize>,
    right: Option<usize>,
    val: i16,
    parent: Option<usize>,
    index: usize,
}

impl SnailNumber {
    fn from_number(n: u32) -> SnailNumber {
        SnailNumber {
            is_pair: false,
            left: None,
            right: None,
            val: n as i16,
            parent: None,
            index: 0,
        }
    }
    fn empty() -> SnailNumber {
        SnailNumber {
            is_pair: false,
            left: None,
            right: None,
            val: 0,
            parent: None,
            index: 0,
        }
    }
}
