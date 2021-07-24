module Config

type Config = {
    ConnectionString: string
}

let initialize () = {
    ConnectionString = Database.getConnectionString()
}
