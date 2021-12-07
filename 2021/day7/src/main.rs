fn main() {
    let list: Vec<i32> = include_str!("input.txt")
        .split(',')
        .map(str::parse::<i32>)
        .map(Result::unwrap)
        .collect();

    let lowest = list.iter().fold(0, |iter, item| {
        if item < &iter {
            return item.clone();
        } else {
            return iter;
        }
    });

    let highest = list.iter().fold(0, |iter, item| {
        if item > &iter {
            return item.clone();
        } else {
            return iter;
        }
    });

    let mut lowest_fuel = 0;
    for i in lowest..=highest {
        let fuel = list.iter().fold(0, |iter, item| {
            return iter + (item - i).abs();
        });
        if lowest_fuel == 0 || fuel < lowest_fuel {
            lowest_fuel = fuel;
        }
    }
    println!("{}", lowest_fuel);

    let mut lowest_fuel = 0;
    for i in lowest..=highest {
        let fuel = list.iter().fold(0, |iter, item| {
            return iter + splat((item - i).abs());
        });
        if lowest_fuel == 0 || fuel < lowest_fuel {
            lowest_fuel = fuel;
        }
    }
    println!("{}", lowest_fuel);
}
fn splat(num: i32) -> i32 {
    return num * (num + 1) / 2;
    // https://en.wikipedia.org/wiki/Binomial_coefficient
    // I forgot about this, so originally did it the long way
    //let mut n = 0;
    // for i in 1..=num {
    //     n += i;
    // }
    // n
}