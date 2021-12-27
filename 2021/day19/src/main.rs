use std::collections::{HashSet, HashMap};

fn main() {
    let str = include_str!("input.txt");
    let mut map = BeaconMap::from(str);
    map.find_beacons();
    println!("{}", map.beacons.len());
    println!("{}", map.find_distance());
    
}

#[derive(PartialEq, Eq, Hash, Clone, Debug)]
struct Coord3d {
    x: i32,
    y: i32,
    z: i32
}

impl Coord3d {
    fn from(x: i32, y: i32, z: i32) -> Coord3d {
        Coord3d {
            x, y, z
        }
    }
    fn vector(&self, other: &Coord3d) -> Coord3d {
        Coord3d {
            x: other.x - self.x, y: other.y - self.y, z: other.z - self.z
        }
    }
    fn translate(&self, trans: Coord3d) -> Coord3d {
        Coord3d {
            x: self.x + trans.x, y: self.y + trans.y, z: self.z + trans.z
        }
    }
    fn distance(&self, from: Coord3d) -> u32 {
        ((self.x - from.x).abs() + (self.y - from.y).abs() + (self.z - from.z).abs()).try_into().unwrap()
    }
}

struct BeaconMap {
    beacons: HashSet<Coord3d>,
    scanners: HashMap<usize, Coord3d>,
    scanner_beacons: HashMap<usize, Vec<Coord3d>>
}

impl BeaconMap {
    fn from(str: &str) -> BeaconMap {
        let mut scanner_beacons = HashMap::new();
        let mut scanner_id = 0;
        let mut scanners = HashMap::new();
        for line in str.lines() {
            if line == "" { continue; }
            else if line.starts_with("--- ") {
                scanner_id = line.replace("-", "").replace(" ", "").replace("scanner", "").parse::<usize>().unwrap();
                scanner_beacons.insert(scanner_id, Vec::new());
            } else {
                let mut split = line.split(',');
                scanner_beacons.entry(scanner_id).and_modify(|item| item.push(Coord3d::from(split.next().unwrap().parse::<i32>().unwrap(), split.next().unwrap().parse::<i32>().unwrap(), split.next().unwrap().parse::<i32>().unwrap())));
            }
        }
        scanners.insert(0, Coord3d::from(0,0,0));
        let mut beacons = HashSet::new();
        for beacon in scanner_beacons[&0].clone() {
            beacons.insert(beacon);
        }
        BeaconMap {
            scanner_beacons, 
            beacons,
            scanners
        }
    }

    fn find_distance(&mut self) -> u32 {
        let mut greatest_distance = 0;
        for scanner1 in self.scanners.clone() {
            for scanner2 in self.scanners.clone() {
                let d = scanner1.1.distance(scanner2.1);
                if d > greatest_distance {
                    greatest_distance = d;
                }
            }
        }
        greatest_distance
    }

    fn find_beacons(&mut self) {
        let mut vectors = self.get_vectors();
        let mut scanners_to_fix = Vec::new();
        for i in 1..self.scanner_beacons.len() {
            scanners_to_fix.push(i);
        }

        loop { 
            if scanners_to_fix.len() == 0 { break; }
            let mut found_rotation = None;
            let mut found_translation = None;
            let scanner = scanners_to_fix.remove(0);
            let s_beacons = self.scanner_beacons[&scanner].to_vec();
            let rotations = Rotations::get_all();
            for rotation in rotations.list {
                let result = test_rotation(&vectors, &s_beacons, rotation);
                if result != None {
                    found_rotation = Some(rotation);
                    found_translation = Some(result.unwrap());
                }
            }
            if found_translation == None {
                scanners_to_fix.push(scanner);
                if scanners_to_fix.len() == 1 { break; }
            } else {
                let s_beacons = rotate_beacons(s_beacons, found_rotation.unwrap());
                let s_beacons = translate_beacons(s_beacons, found_translation.clone().unwrap());
                for beacon in s_beacons {
                    self.beacons.insert(beacon);
                }
                vectors = self.get_vectors();
                self.scanners.insert(scanner, found_translation.unwrap());
            }
        }

    }

    fn get_vectors(&mut self) -> HashMap<Coord3d, Coord3d> {
        let mut list = HashMap::new();
        for b1 in self.beacons.clone() {
            for b2 in self.beacons.clone() {
                if b1 == b2 { continue; }
                let vec = b2.vector(&b1);
                if !list.contains_key(&vec) {
                    list.insert(vec, b2.clone());
                }
            }
        }
        list
    }

    
}

fn test_rotation(vectors: &HashMap<Coord3d, Coord3d>, s_beacons: &Vec<Coord3d>, rotation: fn(Coord3d) -> Coord3d) -> Option<Coord3d> {
    let mut matches = 0;
    for b1 in s_beacons.clone() {
        let rotated_b1 = rotation(b1.clone());
        for b2 in s_beacons.clone() {
            if b1 == b2 { continue; }
            let rotated_b2 = rotation(b2.clone());
            let vector = rotated_b1.vector(&rotated_b2);
            if vectors.contains_key(&vector) {
                matches += 1;
                if matches == 12 {
                    return Some(rotated_b1.vector(&vectors[&vector]))
                }
            }
        }
    }
    None
}

fn rotate_beacons(beacons: Vec<Coord3d>, rotation: fn(Coord3d) -> Coord3d) -> Vec<Coord3d> {
    let mut rotated = Vec::new();
    for beacon in beacons {
        rotated.push(rotation(beacon));
    }
    rotated
}
fn translate_beacons(beacons: Vec<Coord3d>, translation: Coord3d) -> Vec<Coord3d> {
    let mut translated = Vec::new();
    for beacon in beacons {
        translated.push(beacon.translate(translation.clone()));
    }
    translated
}



// TODO is there a better way to this?
struct Rotations {
    list: Vec<fn(Coord3d) -> Coord3d>
}
impl Rotations {
    fn get_all() -> Rotations {
        let list: Vec<fn(Coord3d) -> Coord3d> = vec![
            |c: Coord3d| Coord3d::from(c.x, c.y, c.z),
            |c: Coord3d| Coord3d::from(c.x, -c.z, c.y),
            |c: Coord3d| Coord3d::from(c.x, -c.y, -c.z),
            |c: Coord3d| Coord3d::from(c.x, c.z, -c.y),

            |c: Coord3d| Coord3d::from(-c.y, c.x, c.z),
            |c: Coord3d| Coord3d::from(c.z, c.x, c.y),
            |c: Coord3d| Coord3d::from(c.y, c.x, -c.z),
            |c: Coord3d| Coord3d::from(-c.z, c.x, -c.y),

            |c: Coord3d| Coord3d::from(-c.x, -c.y, c.z),
            |c: Coord3d| Coord3d::from(-c.x, -c.z, -c.y),
            |c: Coord3d| Coord3d::from(-c.x, c.y, -c.z),
            |c: Coord3d| Coord3d::from(-c.x, c.z, c.y),

            |c: Coord3d| Coord3d::from(c.y, -c.x, c.z),
            |c: Coord3d| Coord3d::from(c.z, -c.x, -c.y),
            |c: Coord3d| Coord3d::from(-c.y, -c.x, -c.z),
            |c: Coord3d| Coord3d::from(-c.z, -c.x, c.y),

            |c: Coord3d| Coord3d::from(-c.z, c.y, c.x),
            |c: Coord3d| Coord3d::from(c.y, c.z, c.x),
            |c: Coord3d| Coord3d::from(c.z, -c.y, c.x),
            |c: Coord3d| Coord3d::from(-c.y, -c.z, c.x),

            |c: Coord3d| Coord3d::from(-c.z, -c.y, -c.x),
            |c: Coord3d| Coord3d::from(-c.y, c.z, -c.x),
            |c: Coord3d| Coord3d::from(c.z, c.y, -c.x),
            |c: Coord3d| Coord3d::from(c.y, -c.z, -c.x)
        ];
        Rotations { list }
    }
}

// These are the ones that aren't needed
// One day I can try to work out why
//fn r2(c: Coord3d) -> Coord3d { Coord3d::from(c.x, c.y, -c.z) }
//fn r3(c: Coord3d) -> Coord3d { Coord3d::from(c.x, -c.y, c.z) }
//fn r5(c: Coord3d) -> Coord3d { Coord3d::from(c.x, c.z, c.y) }
//fn r8(c: Coord3d) -> Coord3d { Coord3d::from(c.x, -c.z, -c.y) }


//fn r9(c: Coord3d) -> Coord3d { Coord3d::from(-c.x, c.y, c.z) }
//fn r12(c: Coord3d) -> Coord3d { Coord3d::from(-c.x, -c.y, -c.z) }
//fn r14(c: Coord3d) -> Coord3d { Coord3d::from(-c.x, c.z, -c.y) }
//fn r15(c: Coord3d) -> Coord3d { Coord3d::from(-c.x, -c.z, c.y) }

//fn r17(c: Coord3d) -> Coord3d { Coord3d::from(c.y, c.x, c.z) }
//fn r19(c: Coord3d) -> Coord3d { Coord3d::from(-c.y, -c.x, c.z) }
//fn r21(c: Coord3d) -> Coord3d { Coord3d::from(c.z, c.y, c.x) }
//fn r23(c: Coord3d) -> Coord3d { Coord3d::from(-c.z, -c.y, c.x) }








