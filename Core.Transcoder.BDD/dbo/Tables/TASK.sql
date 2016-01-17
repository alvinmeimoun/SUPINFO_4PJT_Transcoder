CREATE TABLE [dbo].[TASK] (
    [PK_ID_TASK]              INT            IDENTITY (1, 1) NOT NULL,
    [FK_ID_USER]              INT            NULL,
    [STATUS]                  INT            NULL,
    [FILE_URL]                NVARCHAR (MAX) NULL,
    [FILE_URL_TEMP]           NVARCHAR (MAX) NULL,
    [FILE_URL_DESTINATION]    NVARCHAR (MAX) NULL,
    [FK_ID_FORMAT_BASE]       INT            NULL,
    [FK_ID_FORMAT_TO_CONVERT] INT            NULL,
    [IS_PAID]                 BIT            NULL,
    [THREAD_ID]               INT            NULL,
    [SERVER_ID]               INT            NULL,
    [DATE_BEGIN_CONVERSION]   DATETIME       NULL,
    [DATE_END_CONVERSION]     DATETIME       NULL,
    [FK_ID_PARENT_TASK]       INT            NULL,
    [LENGTH]                  INT            NULL,
    [FK_ID_PARAM_SPLIT]       INT            NULL,
    CONSTRAINT [PK_TASK] PRIMARY KEY CLUSTERED ([PK_ID_TASK] ASC)
);

