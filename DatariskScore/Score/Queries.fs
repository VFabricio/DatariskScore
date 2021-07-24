module Score.Queries

open Score.Repository

let getScore (connectionString: string) (cpf: string) =
    getByCpf connectionString cpf
