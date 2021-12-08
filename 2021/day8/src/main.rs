use std::collections::HashMap;

fn main() {
    let list: Vec<[Vec<&str>; 2]> = include_str!("input.txt")
        .lines()
        .map(parse_line_to_values)
        .collect();

    let count = get_simple_count(&list);
    println!("{}", count);

    let mut big_total = 0;
    for i in 0..list.len() {
        let segment_to_letter = get_segment_translation(&list[i][0]);
        big_total += get_output_value(&list[i][1], segment_to_letter);
    }
    println!("{}", big_total);
}

struct SegmentTranslation {
    letter_to_segment: HashMap<char, i8>,
    segment_to_letter: HashMap<i8, char>,
    segment_group_to_letters: HashMap<(i8, i8), [char; 2]>,
}
impl SegmentTranslation {
    fn new() -> SegmentTranslation {
        SegmentTranslation {
            letter_to_segment: HashMap::new(),
            segment_group_to_letters: HashMap::new(),
            segment_to_letter: HashMap::new(),
        }
    }
}

fn parse_line_to_values(line: &str) -> [Vec<&str>; 2] {
    let split: Vec<&str> = line.split(" | ").collect();
    let first_half = split[0];
    let second_half = split[1];
    [
        first_half.split(' ').collect(),
        second_half.split(' ').collect(),
    ]
}

fn get_simple_count(list: &Vec<[Vec<&str>; 2]>) -> i32 {
    let mut count = 0;
    for i in 0..list.len() {
        let item = &list[i][1];
        for i in 0..item.len() {
            let value_length = item[i].len();
            if value_length == 2 || value_length == 3 || value_length == 4 || value_length == 7 {
                count += 1;
            }
        }
    }
    count
}

fn get_segment_translation(pattern: &Vec<&str>) -> HashMap<i8, char> {
    let mut translation = SegmentTranslation::new();
    let item = pattern;
    // Two digits
    let two_digit = item.iter().find(|item| item.len() == 2).unwrap();
    let mut chars = two_digit.chars();
    translation
        .segment_group_to_letters
        .insert((3, 6), [chars.next().unwrap(), chars.next().unwrap()]);

    // Three digits
    let three_digit = item.iter().find(|item| item.len() == 3).unwrap();
    let mut chars = three_digit.chars().filter(|item| {
        translation.segment_group_to_letters[&(3, 6)][0] != *item
            && translation.segment_group_to_letters[&(3, 6)][1] != *item
    });
    // SEGMENT ONE DONE
    let char = chars.next().unwrap();
    translation.segment_to_letter.insert(1, char);
    translation.letter_to_segment.insert(char, 1);

    // Four digits
    let four_digit = item.iter().find(|item| item.len() == 4).unwrap();
    let mut chars = four_digit.chars().filter(|item| {
        translation.segment_group_to_letters[&(3, 6)][0] != *item
            && translation.segment_group_to_letters[&(3, 6)][1] != *item
    });
    let arr = [chars.next().unwrap(), chars.next().unwrap()];
    translation
        .segment_group_to_letters
        .insert((2, 4), arr);

    // Six digits, 6 (doesn't contain both segments 3 and 6)
    let six_digits_6 = item
        .iter()
        .find(|item| {
            item.len() == 6
                && (!item.contains(translation.segment_group_to_letters[&(3, 6)][0])
                    || !item.contains(translation.segment_group_to_letters[&(3, 6)][1]))
        })
        .unwrap();
    let mut char_6 = six_digits_6.chars().filter(|item| {
        translation.segment_group_to_letters[&(3, 6)][0] == *item
            || translation.segment_group_to_letters[&(3, 6)][1] == *item
    });
    // SEGMENTS SIX AND THREE DONE
    let char = char_6.next().unwrap();
    translation.segment_to_letter.insert(6, char);
    translation.letter_to_segment.insert(char, 6);
    let char = translation.segment_group_to_letters[&(3, 6)]
        .iter()
        .find(|item| **item != char)
        .unwrap();
    translation.segment_to_letter.insert(3, *char);
    translation.letter_to_segment.insert(*char, 3);
    let mut chars = six_digits_6.chars().filter(|item| {
        *item != translation.segment_to_letter[&1]
            && *item != translation.segment_to_letter[&6]
            && *item != translation.segment_group_to_letters[&(2, 4)][0]
            && *item != translation.segment_group_to_letters[&(2, 4)][1]
    });
    let arr = [chars.next().unwrap(), chars.next().unwrap()];
    translation
        .segment_group_to_letters
        .insert((5, 7), arr);

    // Six digits, 0 (doesn't contain both segments 2 and 4)
    let six_digits_0 = item
        .iter()
        .find(|item| {
            item.len() == 6
                && (!item.contains(translation.segment_group_to_letters[&(2, 4)][0])
                    || !item.contains(translation.segment_group_to_letters[&(2, 4)][1]))
        })
        .unwrap();
    let mut char_0 = six_digits_0.chars().filter(|item| {
        translation.segment_group_to_letters[&(2, 4)][0] == *item
            || translation.segment_group_to_letters[&(2, 4)][1] == *item
    });
    // SEGMENTS TWO AND FOUR DONE
    let char = char_0.next().unwrap();
    translation.segment_to_letter.insert(2, char);
    translation.letter_to_segment.insert(char, 2);
    let char = translation.segment_group_to_letters[&(2, 4)]
        .iter()
        .find(|item| **item != char)
        .unwrap();
    translation.segment_to_letter.insert(4, *char);
    translation.letter_to_segment.insert(*char, 4);

    // Six digits, 0 (doesn't contain both segments 2 and 4)
    let six_digits_9 = item
        .iter()
        .find(|item| {
            item.len() == 6
                && (!item.contains(translation.segment_group_to_letters[&(5, 7)][0])
                    || !item.contains(translation.segment_group_to_letters[&(5, 7)][1]))
        })
        .unwrap();
    let mut char_9 = six_digits_9.chars().filter(|item| {
        translation.segment_group_to_letters[&(5, 7)][0] == *item
            || translation.segment_group_to_letters[&(5, 7)][1] == *item
    });
    // SEGMENTS FIVE AND SEVEN DONE
    let char = char_9.next().unwrap();
    translation.segment_to_letter.insert(7, char);
    translation.letter_to_segment.insert(char, 7);
    let char = translation.segment_group_to_letters[&(5, 7)]
        .iter()
        .find(|item| **item != char)
        .unwrap();
    translation.segment_to_letter.insert(5, *char);
    translation.letter_to_segment.insert(*char, 5);
    translation.segment_to_letter
}

fn get_output_value(output: &Vec<&str>, segment_to_letter: HashMap<i8, char>) -> i32 {
    let mut val = 0;
    let item = output;
    for i in 0..item.len() {
        let value_length = item[i].len();
        let value = item[i];
        if value_length == 2 {
            val = (val * 10) + 1;
        } else if value_length == 3 {
            val = (val * 10) + 7;
        } else if value_length == 4 {
            val = (val * 10) + 4;
        } else if value_length == 7 {
            val = (val * 10) + 8;
        } else if value_length == 6 {
            if value.contains(segment_to_letter[&1])
                && value.contains(segment_to_letter[&2])
                && value.contains(segment_to_letter[&4])
                && value.contains(segment_to_letter[&5])
                && value.contains(segment_to_letter[&6])
                && value.contains(segment_to_letter[&7])
            {
                // 1,2,4,5,6,7 == 6
                val = (val * 10) + 6;
            } else if value.contains(segment_to_letter[&1])
                && value.contains(segment_to_letter[&2])
                && value.contains(segment_to_letter[&3])
                && value.contains(segment_to_letter[&5])
                && value.contains(segment_to_letter[&6])
                && value.contains(segment_to_letter[&7])
            {
                // 1,2,3,5,6,7 == 0
                val = (val * 10) + 0;
            } else if value.contains(segment_to_letter[&1])
                && value.contains(segment_to_letter[&2])
                && value.contains(segment_to_letter[&3])
                && value.contains(segment_to_letter[&4])
                && value.contains(segment_to_letter[&6])
                && value.contains(segment_to_letter[&7])
            {
                // 1,2,3,4,6,7 == 9
                val = (val * 10) + 9;
            }
        } else if value_length == 5 {
            if value.contains(segment_to_letter[&1])
                && value.contains(segment_to_letter[&3])
                && value.contains(segment_to_letter[&4])
                && value.contains(segment_to_letter[&5])
                && value.contains(segment_to_letter[&7])
            {
                // 1,3,4,5,7 == 2
                val = (val * 10) + 2;
            } else if value.contains(segment_to_letter[&1])
                && value.contains(segment_to_letter[&2])
                && value.contains(segment_to_letter[&4])
                && value.contains(segment_to_letter[&6])
                && value.contains(segment_to_letter[&7])
            {
                // 1,2,4,6,7 == 5
                val = (val * 10) + 5;
            } else if value.contains(segment_to_letter[&1])
                && value.contains(segment_to_letter[&3])
                && value.contains(segment_to_letter[&4])
                && value.contains(segment_to_letter[&6])
                && value.contains(segment_to_letter[&7])
            {
                // 1,3,4,6,7 == 3
                val = (val * 10) + 3;
            }
        }
    }
    val
}

// My notes
//  1
// 2 3
//  4
// 5 6
//  7

// 2 digits, number 1, we know 3&6
// 3 digits, number 7, we know 1
// 4 digits, number 4, we know 2&4
// 6 digits, are numbers 0, 6, 9;
// if it doesn't have both 3&6, then it is a 6, with 1,2,4,5,6,7; from this we know 6, 3, 5&7
// if it doesn't have both 2&4, then it is a 0, with 1,2,3,5,6,7; from this we know 2, 4
// else, it is a 9, with 1,2,3,4,6,7; from this we know 5, 7

// 7 digits, number 8
// 5 digits, 1,3,4,5,7, number 2
// 1,2,4,6,7, number 5
// 1,3,4,6,7, number 3
