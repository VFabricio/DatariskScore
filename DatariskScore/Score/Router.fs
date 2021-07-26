module Score.Router

let router =
    Saturn.Router.router { forward "" Score.Controller.controller }
