CREATE TABLE score (
    id uuid NOT NULL,
    PRIMARY KEY (id),
    cpf TEXT NOT NULL UNIQUE,
    score SMALLINT NOT NULL,
    created_at timestamptz NOT NULL
);
