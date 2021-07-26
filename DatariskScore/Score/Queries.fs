module Score.Queries

open Score.Domain
open Score.Repository

let getScore (connectionString: string) (rawCpf: string) =
    Cpf.create rawCpf |> getByCpf connectionString
