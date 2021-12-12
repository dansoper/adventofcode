use std::collections::HashMap;

// NB This was really slow
// I got my answer
// I tried to make it quicker
// It was even slower
// I might improve it further in the futureS

fn main() {
    let list: Vec<&str> = include_str!("input.txt").lines().collect();
    let mut g = Graph::from(list);
    let p = g.get_paths();
    println!("{:?}", p.len());
}

struct Node {
    letter: String,
    is_large: bool,
    connections: Vec<String>,
    paths_from_here: Vec<Path>,
}
impl Node {
    fn from_string(s: &str) -> Node {
        Node {
            letter: s.to_string(),
            is_large: s.to_uppercase() == s,
            connections: Vec::new(),
            paths_from_here: Vec::new(),
        }
    }
}

struct Path {
    path: Vec<String>,
    hash: String
}
impl Path {
    fn from_vec(path: Vec<String>) -> Path {
        Path {
            hash: Path::path_to_string(&path),
            path
        }
    }
    fn by_adding(&self, letter: String) -> Path {
        let mut cloned_path = self.path.to_vec();
        cloned_path.push(letter);
        Path {
            hash: Path::path_to_string(&cloned_path),
            path: cloned_path
        }
    }
    fn contains_double_lowercase(&self) -> bool {
        self.path.iter()
            .filter(|item| {
                &item.to_lowercase() == *item
                    && self.path.iter().filter(|a| item == a).count() > 1
            })
            .count()
            > 0
    }
    fn contains_node_no_more_than_one(&self, node: String) -> bool {
        self.path.iter()
            .filter(|a| a == &&node)
            .count()
            <= 1
    }
    fn contains_node(&self, node: String) -> bool {
        self.path.contains(&node)
    }
    fn path_to_string(v: &Vec<String>) -> String {
        v
            .iter()
            .fold("".to_string(), |iter, item| format!("{}{}", iter, item))
    }
}
impl Clone for Path {
    fn clone(&self) -> Path {
        Path {
            path: self.path.to_vec(),
            hash: self.hash.to_string()
        }
    }
}

// The cache was an attempt to speed it up
// But it still takes ages
// With more time, I might have tried a different cache
struct Graph {
    nodes: HashMap<String, Node>,
    cache: HashMap<String, Vec<Path>>,
}
impl Graph {
    fn from(string_list: Vec<&str>) -> Graph {
        let mut g = Graph {
            nodes: HashMap::new(),
            cache: HashMap::new(),
        };
        // First add the nodes
        for i in 0..string_list.len() {
            let string_item = string_list[i];
            let split = string_item.split('-');
            for s in split {
                if !g.nodes.contains_key(s) {
                    g.nodes.insert(s.to_string(), Node::from_string(s));
                }
            }
        }
        // Then add the connections
        for i in 0..string_list.len() {
            let string_item = string_list[i];
            let split: Vec<&str> = string_item.split('-').collect();
            let string_one = split[0].to_string();
            let string_two = split[1].to_string();

            if !g.nodes[&string_one].connections.contains(&string_two) {
                g.nodes
                    .entry(string_one.to_string())
                    .and_modify(|e| e.connections.push(string_two.to_string()));
            }
            if !g.nodes[&string_two].connections.contains(&string_one) {
                g.nodes
                    .entry(string_two.to_string())
                    .and_modify(|e| e.connections.push(string_one.to_string()));
            }
        }
        g
    }

    fn get_paths(&mut self) -> Vec<Path> {
        self.get_paths_from_node(String::from("start"), Path::from_vec(Vec::new()))
    }

    fn get_paths_from_node(
        &mut self,
        node_string: String,
        path_so_far: Path,
    ) -> Vec<Path> {
        let key = format!("{}|{}", node_string, path_so_far.hash);
        if !self.cache.contains_key(&*key) {
            let node = self.nodes.get_mut(&node_string).unwrap();
            let mut all_paths = Vec::new();
            let connections = node.connections.to_vec();
            for i in 0..connections.len() {
                let connection = connections[i].to_string();
                let connecting_node = self.nodes.get(&connection).unwrap();
                // for part 1 change this to true; part 2, false
                let mut no_small_revisits = false;
                if path_so_far.contains_double_lowercase()
                {
                    no_small_revisits = true;
                }
                if connection != "start"
                    && (connecting_node.is_large
                        || (!no_small_revisits
                            && path_so_far.contains_node_no_more_than_one(connecting_node.letter.to_string()))
                        || (no_small_revisits && !path_so_far.contains_node(connecting_node.letter.to_string())))
                {
                    let cloned_path = path_so_far.by_adding(connecting_node.letter.to_string());
                    if connection == "end" {
                        all_paths.push(cloned_path);
                    } else {
                        let paths = self
                            .get_paths_from_node(connecting_node.letter.to_string(), cloned_path);
                        for i in 0..paths.len() {
                            if !all_paths.iter().any(|a| a.hash == paths[i].hash) {
                                all_paths.push(paths[i].clone());
                            }
                        }
                    }
                }
            }
            let node = self.nodes.get_mut(&node_string).unwrap();
            node.paths_from_here = all_paths.to_vec();
            let key = format!("{}|{}", node_string, path_so_far.hash);
            self.cache.insert(key, all_paths);
        }
        self.cache[&key].to_vec()
    }
}
