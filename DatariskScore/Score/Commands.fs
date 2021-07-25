module Score.Commands

open Score.Domain
open Score.Repository
open FSharp.Control.Tasks
open System.Threading.Tasks

type CreateScoreDto = {
    // fsharplint:disable-next-line RecordFieldNames
    cpf: string
}

let createScore (connectionString: string) (dto: CreateScoreDto): Task<Result<unit, RepositoryError>> =
    task {
        let guid = System.Guid.NewGuid()
        let now = System.DateTime.UtcNow
        let scoreValue = System.Random().Next(minScore, maxScore + 1)

        let score = {
            Id = guid
            Cpf = Cpf.create dto.cpf
            CreatedAt = now
            Value = scoreValue
        }

        return! Score.Repository.insert connectionString score
    }
