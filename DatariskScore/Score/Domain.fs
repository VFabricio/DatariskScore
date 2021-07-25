module Score.Domain

open System.Text.RegularExpressions

module Cpf =
   type T = Cpf of string with
      override this.ToString() =
         match this with
            Cpf c -> c

   let create rawCpf = Regex.Replace(rawCpf, "[^\d]", "") |> Cpf

type Score = {
   Id: System.Guid
   CreatedAt: System.DateTime
   Cpf: Cpf.T
   Value: int
}

let minScore = 1
let maxScore = 1000
