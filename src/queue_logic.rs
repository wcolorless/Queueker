use crate::settings_file::{save_queue_list, QueueParam};
use serde::{Deserialize, Serialize};
use std::collections::*;

struct QueueItem {
    name: String,
    limit: i64,
    queue: VecDeque<Vec<u8>>,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct QueueItemInfo {
    name: String,
    limit: i64,
    items: i64,
}

struct QueuesList {
    queues: Vec<QueueItem>,
}

static mut PRIME_QUEUE_LIST: QueuesList = QueuesList { queues: Vec::new() };

pub fn get_queue_list_info() -> String {
    unsafe {
        let mut list_info: Vec<QueueItemInfo> = Vec::new();
        for que in &PRIME_QUEUE_LIST.queues {
            list_info.push(QueueItemInfo {
                name: que.name.clone(),
                limit: que.limit,
                items: que.queue.len() as i64,
            });
        }
        match serde_json::to_string(&list_info) {
            Ok(value) => {
                return value;
            }
            _ => {
                return  String::from("err");
            }
        };
    }
}

pub fn add_new_queue(_name: &String, _limit: i64) -> String {
    unsafe {
        let find_result = PRIME_QUEUE_LIST.queues.iter().find(|&x| &x.name == _name);
        match find_result {
            Some(_exist_queue) => {
                return String::from("err");
            }
            None => {
                PRIME_QUEUE_LIST.queues.push(QueueItem {
                    name: _name.clone(),
                    limit: _limit,
                    queue: VecDeque::new(),
                });
                // Save all queue to file
                let mut queues_list = crate::settings_file::QueuesList { list: Vec::new() };
                for que in &PRIME_QUEUE_LIST.queues {
                    queues_list.list.push(QueueParam {
                        name: que.name.clone(),
                        limit: que.limit,
                    });
                }
                save_queue_list(queues_list);
                // Send ok result
                return String::from("ok");
            }
        }
    }
}

pub fn init_from_settings(list: &Vec<QueueParam>) {
    for queue in list {
        add_new_queue(&queue.name, queue.limit);
        println!("add_new_queue: {}", &queue.name);
    }
    println!("Init queue list: Complete");
}

pub fn remove_queue(_name: &String) {
    unsafe {
        let find_result = PRIME_QUEUE_LIST
            .queues
            .iter()
            .position(|x| &x.name == _name);
        match find_result {
            Some(index) => {
                PRIME_QUEUE_LIST.queues.remove(index);
            }
            None => {}
        }
    }
}

pub fn dequeue_item(_name: &String) -> Result<Vec<u8>, &str> {
    unsafe {
        let find_result = PRIME_QUEUE_LIST
            .queues
            .iter()
            .any(|e| e.name == _name.as_str());
        if find_result {
            let queue_find_result = PRIME_QUEUE_LIST
                .queues
                .iter_mut()
                .find(|e| e.name == _name.as_str());
            match queue_find_result {
                Some(queue_item) => {
                    let item_result = queue_item.queue.pop_front();
                    match item_result {
                        Some(item) => {
                            return Ok(item);
                        }
                        _ => {}
                    }
                }
                None => {}
            }
        } else {
            println!("This queue {} not found", _name);
        }
        Err("err")
    }
}

pub fn enqueue_item(_name: &std::string::String, data: &std::vec::Vec<u8>) {
    unsafe {
        let find_result = PRIME_QUEUE_LIST
            .queues
            .iter()
            .any(|e| e.name == _name.as_str());
        if !find_result {
            PRIME_QUEUE_LIST.queues.push(QueueItem {
                name: _name.clone(),
                limit: -1i64,
                queue: VecDeque::new(),
            });
        };
        let queue_find_result = PRIME_QUEUE_LIST
            .queues
            .iter_mut()
            .find(|e| e.name == _name.as_str());
        match queue_find_result {
            Some(queue_item) => {
                queue_item.queue.push_back(data.to_vec());
            }
            None => {}
        };
    }
}

pub fn purge_queue(_name: &String) {
    unsafe {
        let find_result = PRIME_QUEUE_LIST
            .queues
            .iter_mut()
            .find(|x| &x.name == _name);
        match find_result {
            Some(queue_box) => {
                queue_box.queue.clear();
            }
            None => {}
        }
    }
}
