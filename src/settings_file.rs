use crate::queue_logic::init_from_settings;
use serde::{Deserialize, Serialize};
use std::{env, fs};

#[derive(Serialize, Deserialize, Debug)]
pub struct AppSettings {
    pub host: String,
    pub login: String,
    pub password: String,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct QueueParam {
    pub name: String,
    pub limit: i64,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct QueuesList {
    pub list: Vec<QueueParam>,
}

static mut APP_SETTINGS: Option<AppSettings> = None;
static mut QUEUE_LIST: Option<QueuesList> = None;

pub fn load_settings() {
    let dir = env::current_dir().unwrap();
    let path = format!("{}\\appsettings.json", dir.display());
    let settings = fs::read_to_string(path).expect("Something went wrong reading the file");
    let settings_obj = serde_json::from_str::<AppSettings>(&settings);
    println!("{}", &settings);
    match settings_obj {
        Ok(v) => {
            println!("Complete: Load App settings");
            unsafe {
                APP_SETTINGS = Some(AppSettings {
                    login: v.login.clone(),
                    password: v.password.clone(),
                    host: v.host.clone(),
                });
            }
        }
        Err(e) => {
            eprintln!("Error: Load App settings {}", e);
        }
    }
}

pub fn get_host_params() -> (std::net::Ipv4Addr, u16) {
    unsafe {
        match &APP_SETTINGS {
            Some(settings) => {
                let parts: Vec<&str> = settings.host.split(":").collect();
                let addrr: Vec<&str> = parts[0].split(".").collect();
                let result = std::net::Ipv4Addr::new(
                    addrr[0].parse::<u8>().unwrap(),
                    addrr[1].parse::<u8>().unwrap(),
                    addrr[2].parse::<u8>().unwrap(),
                    addrr[3].parse::<u8>().unwrap(),
                );
                println!(
                    "load host address: {}.{}.{}.{}:{}",
                    addrr[0], addrr[1], addrr[2], addrr[3], parts[1]
                );
                (result, parts[1].parse::<u16>().unwrap())
            }
            None => (std::net::Ipv4Addr::new(127, 0, 0, 1), 3000),
        }
    }
}

fn save_settings(json: String) {
    let dir = env::current_dir().unwrap();
    let path = format!("{}\\appsettings.json", dir.display());
    let save_result = fs::write(path, json);
    match save_result {
        _ => {
            
        }
    }
}

pub fn check_cred(login: &str, password: &str) -> bool {
    unsafe {
        println!("Input Cred check: login: {}; pass: {}", login, password);
        match &APP_SETTINGS {
            Some(settings) => {
                if settings.login.as_str() == login {
                    return true;
                }
                return false;
            }
            None => {}
        }
    }
    false
}

pub fn change_cred(login: &str, password: &str) {
    unsafe {
        println!("Change Cred: login: {}; pass: {}", login, password);
        let old_host = match &APP_SETTINGS {
            Some(value) => value.host.clone(),
            _ => "".to_string(),
        };
        let new_settings = AppSettings {
            login: login.to_string().clone(),
            password: password.to_string().clone(),
            host: old_host,
        };
        let new_file_json = serde_json::to_string(&new_settings).unwrap();
        save_settings(new_file_json);
        APP_SETTINGS = Some(new_settings);
    }
}

pub fn load_queues_list() {
    let dir = env::current_dir().unwrap();
    let path = format!("{}\\queuelist.json", dir.display());
    let queueslist = fs::read_to_string(path).expect("Something went wrong reading the file");
    let queueslist_obj = serde_json::from_str::<QueuesList>(&queueslist);
    match queueslist_obj {
        Ok(v) => {
            println!("Complete: Load queues list");
            for queue in &v.list {
                println!("Queue Name: {}; size limit: {}", queue.name, queue.limit);
            }
            unsafe {
                QUEUE_LIST = Some(v);
            }
        }
        Err(e) => {
            eprintln!("Error: Load App settings {}", e);
        }
    }
}

pub fn init_queue_list() {
    unsafe {
        match &QUEUE_LIST {
            Some(queues) => {
                let mut init_list: Vec<QueueParam> = Vec::new();
                for param in &queues.list {
                    init_list.push(QueueParam {
                        name: param.name.clone(),
                        limit: param.limit,
                    });
                }
                init_from_settings(&init_list);
            }
            None => {
                println!("Queue List if empty");
            }
        }
    }
}

pub fn save_queue_list(queues_list: QueuesList) {
    let dir = env::current_dir().unwrap();
    let path = format!("{}\\queuelist.json", dir.display());
    let json = serde_json::to_string(&queues_list);
    match json {
        Ok(result) => {
            let _save_result = fs::write(path, result);
        }
        Err(err) => {
            println!("save_queue_list error: {}", err);
        }
    }
}
