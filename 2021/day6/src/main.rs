use std::collections::HashMap;

fn main() {
    let list: Vec<u16> = include_str!("input.txt")
        .split(',')
        .map(str::parse::<u16>)
        .map(Result::unwrap)
        .collect();

    // This is where the main counts happen
    let mut count_per_day: HashMap<u16, i64> = HashMap::new();
    for i in 0..list.len() {
        *count_per_day.entry(list[i]).or_insert(0) += 1;
    }

    // Babies gestate for 3 days before being added to the relevant point in count_per_day
    let mut babies_grouped: HashMap<u16, i64> = HashMap::new();
    babies_grouped.insert(0, 0);
    babies_grouped.insert(1, 0);
    babies_grouped.insert(2, 0);

    let mut day_count = 0;
    let mut day_of_week = 0;
    let stop_at_day = 256;
    let pause_at_day = 80;
    loop {
        if count_per_day.contains_key(&day_of_week) {
            // Add babies
            *babies_grouped.entry(2).or_insert(0) += count_per_day[&day_of_week];
        }
        // Move babies forward one day, and the oldest ones into the normal count per day
        *count_per_day.entry(day_of_week).or_insert(0) += babies_grouped[&0];
        *babies_grouped.entry(0).or_insert(0) = babies_grouped[&1];
        *babies_grouped.entry(1).or_insert(0) = babies_grouped[&2];
        *babies_grouped.entry(2).or_insert(0) = 0;

        day_of_week += 1;
        if day_of_week == 7 { day_of_week = 0; }
        day_count += 1;
        if day_count == stop_at_day { break; }
        if day_count == pause_at_day {
            let count = double_count(&count_per_day, &babies_grouped);
            println!("80 days: {}", count);
        }
    }
    let count = double_count(&count_per_day, &babies_grouped);
    println!("256 days: {}", count);
}

fn double_count(one: &HashMap<u16, i64>, two: &HashMap<u16, i64>) -> i64 {
    one.keys().fold(0, |iter, item| iter + one[item]) + two.keys().fold(0, |iter, item| iter + two[item])
}
