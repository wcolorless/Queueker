use {
    crate::prime_logic::ProtocolInnerMessage, crate::prime_logic::ProtocolMessage,
    crate::prime_logic::ServerResult, crate::queue_logic, crate::settings_file,
};

pub fn request_analyze(message: &ProtocolMessage) -> ServerResult {
    let mut result = ServerResult::new();
    // check login and pass
    let cred_check_result = settings_file::check_cred(message.login.as_str(), message.pass.as_str());
    if !cred_check_result {
        println!("login or pass error");
        result.result_type = String::from("err");
        result.message = String::from("err: login or pass error"); 
        return result;
    }
    if message.direct == "in" {
        if message.command == "write" {
            crate::queue_logic::enqueue_item(&message.quename, &message.data);
            result.result_type = String::from("ok");
        }
    } else if message.direct == "out" {
        if message.command == "read" {
            let read_result = crate::queue_logic::dequeue_item(&message.quename);
            match read_result {
                Ok(item) => {
                    result.data = item;
                    result.result_type = String::from("data");
                }
                _ => {}
            }
        }
    } else if message.direct == "-" {
        let inner_message_result = serde_json::from_str::<ProtocolInnerMessage>(&message.command);
        let mut inner_message: ProtocolInnerMessage = ProtocolInnerMessage {
            subcommand: "".to_string(),
            message: "".to_string(),
        };
        let is_inner_message = match inner_message_result {
            Ok(is_true) => {
                inner_message = is_true;
                true
            }
            _ => false,
        };

        if message.command == "getques" {
            // Send list of queue names
            let info = crate::queue_logic::get_queue_list_info();
            result.result_type = String::from("ok");
            result.message = info;
        } else if message.command == "addque" {
            // Add new queue
            let message_name = message.quename.clone();
            let parts: Vec<&str> = message_name.split(":").collect();
            let add_result =
                queue_logic::add_new_queue(&parts[0].to_string(), parts[1].parse().unwrap());
            result.result_type = add_result.clone();
        } else if message.command == "removeque" {
            // Remove queue
            let message_name = message.quename.clone();
            let parts: Vec<&str> = message_name.split(":").collect();
            queue_logic::remove_queue(&parts[0].to_string());
            result.result_type = String::from("ok");
        } else if message.command == "purge" {
            // Purge queue
            let message_name = message.quename.clone();
            let parts: Vec<&str> = message_name.split(":").collect();
            queue_logic::purge_queue(&parts[0].to_string());
            result.result_type = String::from("ok");
        } else if is_inner_message && inner_message.subcommand == "chpass" {
            // Change login & password
            let parts: Vec<&str> = inner_message.message.split(":").collect();
            let new_login = parts[0];
            let new_pass = parts[1];
            settings_file::change_cred(new_login, new_pass);
            result.result_type = String::from("ok");
        }
    }
    result
}
