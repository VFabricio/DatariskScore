module Config

type Config = {
    ConnectionString: string
}

let initialize () = {
    ConnectionString = Database.getConnectionString()
}

let initializeForTest () = {
    ConnectionString = Database.getTestConnectionString()
}
