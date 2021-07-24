module Score.Domain

type Score = {
   Id: System.Guid
   CreatedAt: System.DateTime
   Cpf: string
   Value: int
}

let minScore = 1
let maxScore = 1000
