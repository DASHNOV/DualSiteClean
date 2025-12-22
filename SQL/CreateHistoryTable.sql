CREATE TABLE dbo.historique_analyses (
    id INT IDENTITY(1,1) PRIMARY KEY,
    date_analyse DATETIME NOT NULL,
    total_employes_39c INT NOT NULL,
    total_employes_19m INT NOT NULL,
    nb_codes_differents INT NOT NULL,
    nb_numeros_differents INT NOT NULL,
    nb_cas_ambigus INT NOT NULL,
    CONSTRAINT DF_historique_analyses_date DEFAULT (GETDATE()) FOR date_analyse
);

CREATE INDEX IX_historique_analyses_date ON dbo.historique_analyses(date_analyse DESC);
