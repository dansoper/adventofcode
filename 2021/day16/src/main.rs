use std::collections::HashMap;

fn main() {
    let str = include_str!("input.txt");
    let mut a = decode_hex(str);

    let d = PacketDecoder { part_one: true };
    println!("{}", d.decode_packet(&mut a));
}

fn operate(one: u64, two: u64, packet_id: u32) -> u64 {
    if packet_id == 0 {
        return one + two;
    } else if packet_id == 1 {
        return one * two;
    } else if packet_id == 2 {
        if one > two {
            return two;
        } else {
            return one;
        }
    } else if packet_id == 3 {
        if one < two {
            return two;
        } else {
            return one;
        }
    } else if packet_id == 5 {
        if one > two {
            return 1;
        } else {
            return 0;
        }
    } else if packet_id == 6 {
        if one < two {
            return 1;
        } else {
            return 0;
        }
    } else if packet_id == 7 {
        if one == two {
            return 1;
        } else {
            return 0;
        }
    }
    0
}

struct PacketDecoder {
    part_one: bool,
}

impl PacketDecoder {
    fn decode_packet(&self, p: &mut Vec<bool>) -> u64 {
        let mut operated = 0;
        let packet_id = bin_to_int(p[3..=5].to_vec());
        if packet_id == 4 {
            return self.literal_value(p);
        } else {
            if self.part_one {
                let packet_version = bin_to_int(p[0..=2].to_vec());
                operated += packet_version as u64;
            }
            p.drain(0..=5);
            let length_type_id = p.remove(0);
            if length_type_id == false {
                let bit_length = bin_to_int(p.drain(0..15).collect());
                let mut bit_count = 0;
                loop {
                    let last_len = p.len();
                    let v = self.decode_packet(p);
                    if self.part_one {
                        operated += v;
                    } else {
                        if bit_count == 0 {
                            operated = v;
                        } else {
                            operated = operate(operated, v, packet_id);
                        }
                    }
                    bit_count += last_len - p.len();
                    if bit_count as u32 >= bit_length {
                        break;
                    }
                }
            } else {
                let packet_count = bin_to_int(p.drain(0..11).collect());
                for i in 0..packet_count {
                    let v = self.decode_packet(p);
                    if self.part_one {
                        operated += v;
                    } else {
                        if i == 0 {
                            operated = v;
                        } else {
                            operated = operate(operated, v, packet_id);
                        }
                    }
                }
            }
        }
        operated
    }

    fn literal_value(&self, p: &mut Vec<bool>) -> u64 {
        let packet_version = bin_to_int(p.drain(0..=2).collect());
        p.drain(0..=2);
        let mut str = String::from("");
        loop {
            let first_bit = p.remove(0);
            let number = p.drain(0..=3);
            for i in number {
                if i {
                    str.push_str("1");
                } else {
                    str.push_str("0");
                }
            }
            if first_bit == false {
                break;
            }
        }
        if self.part_one {
            packet_version as u64
        } else {
            u64::from_str_radix(&str, 2).unwrap()
        }
    }
}

fn bin_to_int(a: Vec<bool>) -> u32 {
    let mut str = String::from("");
    for i in 0..a.len() {
        if a[i] {
            str.push_str("1");
        } else {
            str.push_str("0");
        }
    }
    u32::from_str_radix(&str, 2).unwrap()
}

fn decode_hex(s: &str) -> Vec<bool> {
    let mut decode = HashMap::new();
    decode.insert('0', "0000");
    decode.insert('1', "0001");
    decode.insert('2', "0010");
    decode.insert('3', "0011");
    decode.insert('4', "0100");
    decode.insert('5', "0101");
    decode.insert('6', "0110");
    decode.insert('7', "0111");
    decode.insert('8', "1000");
    decode.insert('9', "1001");
    decode.insert('A', "1010");
    decode.insert('B', "1011");
    decode.insert('C', "1100");
    decode.insert('D', "1101");
    decode.insert('E', "1110");
    decode.insert('F', "1111");

    let arr: Vec<&str> = s.chars().map(|i| decode[&i]).collect();

    let mut a = Vec::new();
    for item in arr {
        for char in item.chars() {
            if char == '1' {
                a.push(true);
            } else {
                a.push(false);
            }
        }
    }
    let remains = a.len() % 4;
    for _i in 0..remains {
        a.insert(0, false);
    }
    a
}
