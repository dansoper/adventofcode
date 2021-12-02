fn main() {
    let list: Vec<Instruction> = include_str!("input.txt")
        .lines()
        .map(parse_to_instruction)
        .collect();

    let position = process_instructions(&list);
    println!("Part 1 multiple is {}", position.depth * position.horizontal);

    let position = process_instructions_part_two(&list);
    println!("Part 2 multiple is {}", position.depth * position.horizontal);
}

#[derive(PartialEq)]
enum Direction {
    Up,
    Down,
    Forward
}

struct Instruction {
    direction: Direction,
    distance: i32
}

struct Position {
    horizontal: i32,
    depth: i32
}

struct PositionWithAim {
    horizontal: i32,
    depth: i32,
    aim: i32
}

fn parse_to_instruction(str: &str) -> Instruction {
    let bits = str.split(' ').collect::<Vec<&str>>();
    
    let instruction = Instruction {
        direction: parse_direction(bits[0]),
        distance: bits[1].parse().unwrap()
    };
    instruction
}

fn parse_direction(str: &str) -> Direction {
    match str {
        "down" => Direction::Down,
        "up" => Direction::Up,
        "forward" => Direction::Forward,
        &_ => Direction::Up
    }
}

fn process_instructions(list: &Vec<Instruction>) -> Position {
    let mut pos = Position {
        horizontal: 0, depth: 0
    };
    for i in 0 .. list.len() {
        let instruction = &list[i];
        if Direction::Up == instruction.direction {
            pos.depth = pos.depth - instruction.distance;
        } else if instruction.direction == Direction::Down {
            pos.depth = pos.depth + instruction.distance;
        } else if instruction.direction == Direction::Forward {
            pos.horizontal = pos.horizontal + instruction.distance;
        }
    }
    pos
}

fn process_instructions_part_two(list: &Vec<Instruction>) -> PositionWithAim {
    let mut pos = PositionWithAim {
        horizontal: 0, depth: 0, aim: 0
    };
    for i in 0 .. list.len() {
        let instruction = &list[i];
        if Direction::Up == instruction.direction {
            pos.aim = pos.aim - instruction.distance;
        } else if instruction.direction == Direction::Down {
            pos.aim = pos.aim + instruction.distance;
        } else if instruction.direction == Direction::Forward {
            pos.horizontal = pos.horizontal + instruction.distance;
            pos.depth = pos.depth + (instruction.distance * pos.aim)
        }
    }
    pos
}
