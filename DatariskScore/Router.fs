module Router

let router =
    Saturn.Router.router { forward "/score" Score.Router.router }
