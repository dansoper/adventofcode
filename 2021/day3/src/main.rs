fn main() {
    let list: Vec<Vec<bool>> = include_str!("input.txt")
        .lines()
        .map(parse_to_bools)
        .collect();

    let inverted_list = invert_array(&list);
    let most_common = get_common_for_list(&inverted_list, get_most_common);
    let least_common = get_common_for_list(&inverted_list, get_least_common);
    println!("Part 1 multiple is {}", bools_to_int(most_common) * bools_to_int(least_common));

    let oxgen_rating = get_rating(&inverted_list, get_most_common);
    let lifsup_rating = get_rating(&inverted_list, get_least_common);
    println!("Part 2 multiple is {}", bools_to_int(oxgen_rating) * bools_to_int(lifsup_rating));
}

/// Counts how many of the Vec are true
fn count_trues(list: &Vec<bool>) -> usize {
    list.iter().filter(|a| a == &&true).count()
}
/// Turns 111000 to [true, true, true, false, false, false]
fn parse_to_bools(str: &str) -> Vec<bool> {
    str.chars().map(|c| c == '1').collect()
}
/// Turns [true, true] to its decimal equivalent (3)
fn bools_to_int(list: Vec<bool>) -> isize {
    let mut str = String::from("");
    for i in 0..list.len() {
        if list[i] {
            str.push_str("1");
        } else {
            str.push_str("0");
        }
    }
    isize::from_str_radix(&str, 2).unwrap()
}
/// Sometimes we have a Vec of Vec where the inner Vec is one set of bits for one decimal; sometimes the inner Vec is one set of bits for a bit position. This method swaps between the too. Quite inefficient, but we get the answer!
fn invert_array(list: &Vec<Vec<bool>>) -> Vec<Vec<bool>> {
    let mut new_list: Vec<Vec<bool>> = vec![vec![false; list.len()]; list[0].len()];
    for i in 0..list[0].len() {
        for j in 0..list.len() {
            new_list[i][j] = list[j][i];
        }
    }
    new_list
}
/// Gets the most common of true/false. If a tie, it returns true
fn get_most_common(list: &Vec<bool>) -> bool {
    let count_of_true = count_trues(&list);
    count_of_true as f64 >= (list.len() as f64 / 2 as f64) 
}
/// Gets the least common of true/false. If a tie, it returns false
fn get_least_common(list: &Vec<bool>) -> bool {
    let count_of_true = count_trues(&list);
    (count_of_true as f64) < (list.len() as f64 / 2 as f64) 
}

/// Part One
fn get_common_for_list(list: &Vec<Vec<bool>>, val_finder: fn(&Vec<bool>) -> bool) -> Vec<bool> {
    let mut new_list: Vec<bool> = Vec::new();
    for i in 0..list.len() {
        new_list.push(val_finder(&list[i]));
    }
    new_list
}

/// Part Two
fn get_rating(list: &Vec<Vec<bool>>, val_finder: fn(&Vec<bool>) -> bool) -> Vec<bool> {
    let mut copied_list = list.clone();
    for i in 0..list.len() {
        let val = val_finder(&copied_list[i]);
        let inverted_list = invert_array(&copied_list);
        let filtered_inverted_list = inverted_list.into_iter().filter(|a| a[i] == val);
        let mut filtered_inverted_list: Vec<Vec<bool>> = filtered_inverted_list.collect();
        if filtered_inverted_list.len() == 1 {
            return filtered_inverted_list.remove(0);
        }
        copied_list = invert_array(&filtered_inverted_list);
    }
    todo!();
}