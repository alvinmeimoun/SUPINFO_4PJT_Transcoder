﻿CREATE TABLE [dbo].[MODE_PAIEMENT] (
    [PK_ID_MODE_PAIEMENT] INT           IDENTITY (1, 1) NOT NULL,
    [NOM_MODE_PAIEMENT]   VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_MODE_PAIEMENT] PRIMARY KEY CLUSTERED ([PK_ID_MODE_PAIEMENT] ASC)
);

