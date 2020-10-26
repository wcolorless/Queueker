use hyper;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug)]
pub struct ProtocolMessage {
    pub login: String,
    pub pass: String,
    pub direct: String,
    pub quename: String,
    pub command: String,
    pub data: Vec<u8>,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct ProtocolAnswer {
    pub result: String,
    pub message: String,
    pub data: Vec<u8>,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct ProtocolInnerMessage {
    pub subcommand: String,
    pub message: String,
}

pub struct ServerResult {
    pub result_type: String,
    pub message: String,
    pub data: Vec<u8>,
}

impl ProtocolAnswer {
    pub fn new() -> ProtocolAnswer {
        ProtocolAnswer {
            result : String::from(""),
            message : String::from(""),
            data : Vec::new()
        }
    }
}

impl ProtocolMessage {
    pub fn new_with_err() -> ProtocolMessage {
        ProtocolMessage {
            login: "error".to_string(),
            pass: "error".to_string(),
            direct: "error".to_string(),
            command: "error".to_string(),
            quename: "error".to_string(),
            data: Vec::new(),
        }
    }
}

impl ServerResult {
    pub fn new() -> ServerResult {
        ServerResult {
            result_type: String::from(""),
            message: String::from(""),
            data: Vec::new(),
        }
    }
}

pub fn check(input_data: &Result<Vec<u8>, hyper::Error>) -> ServerResult {
    match input_data {
        Ok(v) => {
            let new_message: ProtocolMessage = match serde_json::from_slice::<ProtocolMessage>(v) {
                Ok(res) => res,
                _ => ProtocolMessage::new_with_err(),
            };
            let result = crate::request_logic::request_analyze(&new_message);
            println!(
                "Direction: {}; Queue: {};",
                new_message.direct, new_message.quename
            );
            result
        }
        Err(e) => {
            println!("error parsing header: {:?}", e);
            ServerResult {
                result_type: "Error".to_string(),
                message: "".to_string(),
                data: Vec::new(),
            }
        }
    }
}
