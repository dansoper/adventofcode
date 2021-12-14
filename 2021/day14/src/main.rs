use std::{collections::HashMap, borrow::BorrowMut};

fn main() {
    let input: &str = include_str!("input.txt");
    let mut sections = input.split("\n\n");
    let template_string = sections.next().unwrap();
    let mut template: Vec<char> = template_string.chars().collect();
    let rules = sections.next().unwrap();
    let rules = Rules::from_string(rules);
    // part 1 (we could also use the second one)
    println!("{:?}", brute_force(&mut template, &rules, 10));
    // part 2
    let mut template: Vec<char> = template_string.chars().collect();
    println!("{:?}", counting_pairs(&mut template, &rules, 40));
}

struct Rules {
    rules: HashMap<String, char>,
}
impl Rules {
    fn from_string(str: &str) -> Rules {
        let mut r = HashMap::new();
        for line in str.lines() {
            let mut bits = line.split(" -> ");
            let one = bits.next().unwrap();
            let two = bits.next().unwrap();
            r.insert(one.to_string(), two.chars().next().unwrap());
        }
        Rules { rules: r }
    }
}

fn brute_force(template: &mut Vec<char>, rules: &Rules, run_times: u16) -> u64 {
    for _count in 0..run_times {
        let mut to_add = Vec::new();
        for i in 0..template.len() - 1 {
            let c = format!("{}{}", template[i], template[i + 1]);
            if rules.rules.contains_key(&c) {
                to_add.push((i, rules.rules[&c]));
            }
        }
        let mut i = 0;
        for add in to_add {
            template.insert(add.0 + i + 1, add.1);
            i += 1;
        }
    }
    get_greatest_less_least(get_counts_per_letter(&template))
}
fn counting_pairs(template: &mut Vec<char>, rules: &Rules, run_times: u16) -> u64 {
    let mut pairs: HashMap<String, i64> = HashMap::new();
    for i in 0..template.len() - 1 {
        let c = format!("{}{}", template[i], template[i + 1]);
        *pairs.entry(c).or_insert(0) += 1;
    }
    for _count in 0..run_times {
        let mut to_add: Vec<(String, i64)> = Vec::new();
        for rule in &rules.rules {
            if pairs.contains_key(&rule.0.to_string()) {
                to_add.push((rule.0.to_string(), -pairs[&rule.0.to_string()]));
                let mut chars = rule.0.chars();
                let new_pair = format!("{}{}", chars.next().unwrap(), rule.1);
                to_add.push((new_pair, pairs[rule.0]));
                let new_pair = format!("{}{}", rule.1, chars.next().unwrap());
                to_add.push((new_pair, pairs[rule.0]));
            }
        }
        for add in to_add {
            *pairs.entry(add.0).or_insert(0) += add.1;
        }
    }
    get_greatest_less_least(count_letters_from_pairs(&pairs))
}

fn count_letters_from_pairs(pairs: &HashMap<String, i64>) -> HashMap<char, u64> {
    let mut h = HashMap::new();
    for item in pairs {
        let mut chars = item.0.chars();
        *h.entry(chars.next().unwrap()).or_insert(0) += *item.1 as u64;
        *h.entry(chars.next().unwrap()).or_insert(0) += *item.1 as u64;
    }
    for k in h.borrow_mut() {
        *k.1 = (*k.1 + 1) / 2;
    }
    h
}

fn get_counts_per_letter(template: &Vec<char>) -> HashMap<char, u64> {
    let mut h = HashMap::new();
    for c in template {
        *h.entry(*c).or_insert(0) += 1;
    }
    h
}

fn get_greatest_less_least(counts: HashMap<char, u64>) -> u64 {
    let mut vals: Vec<&u64> = counts.values().collect();
    vals.sort();
    *vals.last().unwrap() - *vals.first().unwrap()
}
