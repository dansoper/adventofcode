fn main() {
    let list = include_str!("input.txt")
        .lines()
        .map(str::parse::<u64>)
        .map(Result::unwrap)
        .collect();

    let increase_count = get_ascent_count(&list);
    println!("Part 1 count is {}", increase_count);

    let measurements = get_three_measurements(list);
    let increase_count = get_ascent_count(&measurements);
    println!("Part 2 count is {}", increase_count);
}

fn get_ascent_count(list: &Vec<u64>) -> u64 {
    let mut count = 0;
    for i in 0 .. list.len() {
        if i > 0 && list[i] > list[i-1] {
            count = count + 1;
        }
    }
    count
}
fn get_three_measurements(list: Vec<u64>) -> Vec<u64> {
    let mut new_list = Vec::new();
    for i in 2 .. list.len() {
        new_list.push(list[i-2] + list[i-1] + list[i]);
    }
    new_list
}