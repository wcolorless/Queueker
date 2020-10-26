use futures::TryStreamExt;
use {
    hyper::{
        service::{make_service_fn, service_fn},
        Body,
        Request,
        Response,
        Server,
        Method
    },
    std::net::SocketAddr,
};

mod queue_logic;
mod prime_logic;
mod request_logic;
mod settings_file;

#[tokio::main]
async fn main() {
  // Load settings
  settings_file::load_settings();
  settings_file::load_queues_list();
  settings_file::init_queue_list();
  // Start server
  let my_host = settings_file::get_host_params();
  let addr = SocketAddr::from((my_host.0, my_host.1));
  run_server(addr).await;
}

async fn serve_req(_req: Request<Body>) -> Result<Response<Body>, hyper::Error> {
    let (parts, body) = _req.into_parts();
    match (parts.method, parts.uri.path()) {
        (Method::POST, "/") => {
            let entire_body = body.try_fold(Vec::new(), |mut data, chunk| async move {
                    data.extend_from_slice(&chunk);
                    Ok(data)
                }).await;
            // Send input data to protocol
            let result = prime_logic::check(&entire_body);
            // Create response
            let mut protocol_answer = prime_logic::ProtocolAnswer::new();

            entire_body.map(|_body| {
                if result.result_type == "ok" {
                    protocol_answer.result = String::from("ok");
                    protocol_answer.message = result.message;
                    let answer_ok = serde_json::to_string(&protocol_answer);
                    let body = Body::from(answer_ok.unwrap());
                    Response::new(body)
                } else if result.result_type == "err" {
                    protocol_answer.result = String::from("err");
                    protocol_answer.message = result.message;
                    let answer_err = serde_json::to_string(&protocol_answer);
                    let body = Body::from(answer_err.unwrap());
                    Response::new(body)
                } else if result.result_type == "data" {
                    protocol_answer.result = String::from("data");
                    protocol_answer.data = result.data;
                    let json = serde_json::to_string(&protocol_answer);
                    let body = Body::from(json.unwrap());
                    Response::new(body)
                } else {
                    let body = Body::from("Unhandled error");
                    Response::new(body)
                }
            })
        }
        _ => {
            let body = Body::from("Can only POST to /");
            Ok(Response::new(body))
        }
    }
}

async fn run_server(addr: SocketAddr) {
    println!("Starting Queueker on http://{}", addr);
    let serve_future = Server::bind(&addr).serve(make_service_fn(|_| async {Ok::<_, hyper::Error>(service_fn(serve_req))}));
    if let Err(e) = serve_future.await {
        eprintln!("Queueker server error: {}", e);
    }
}

