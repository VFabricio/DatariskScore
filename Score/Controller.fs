module Score.Controller

open Saturn.ControllerHelpers

let getPersonalId ctx id =
    Controller.text ctx (sprintf "%d" id)

let postPersonalId ctx =
    Controller.text ctx "Hello, World!"

let controller = Saturn.Controller.controller {
    create postPersonalId
    show getPersonalId
}
