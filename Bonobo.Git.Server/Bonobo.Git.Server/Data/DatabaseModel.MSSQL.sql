-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Repository'
CREATE TABLE [dbo].[Repository] (
    [Name] nvarchar(255)  NOT NULL,
    [Description] nvarchar(255)  NULL,
    [Anonymous] bit  NOT NULL
);
GO

-- Creating table 'Role'
CREATE TABLE [dbo].[Role] (
    [Name] nvarchar(255)  NOT NULL,
    [Description] nvarchar(255)  NULL
);
GO

-- Creating table 'Team'
CREATE TABLE [dbo].[Team] (
    [Name] nvarchar(255)  NOT NULL,
    [Description] nvarchar(255)  NULL
);
GO

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [Name] nvarchar(255)  NOT NULL,
    [Surname] nvarchar(255)  NOT NULL,
    [Username] nvarchar(255)  NOT NULL,
    [Password] nvarchar(255)  NOT NULL,
    [Email] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'TeamRepository_Permission'
CREATE TABLE [dbo].[TeamRepository_Permission] (
    [Repositories_Name] nvarchar(255)  NOT NULL,
    [Teams_Name] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'UserRepository_Administrator'
CREATE TABLE [dbo].[UserRepository_Administrator] (
    [AdministratedRepositories_Name] nvarchar(255)  NOT NULL,
    [Administrators_Username] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'UserRepository_Permission'
CREATE TABLE [dbo].[UserRepository_Permission] (
    [Repositories_Name] nvarchar(255)  NOT NULL,
    [Users_Username] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'UserRole_InRole'
CREATE TABLE [dbo].[UserRole_InRole] (
    [Roles_Name] nvarchar(255)  NOT NULL,
    [Users_Username] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'UserTeam_Member'
CREATE TABLE [dbo].[UserTeam_Member] (
    [Teams_Name] nvarchar(255)  NOT NULL,
    [Members_Username] nvarchar(255)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Name] in table 'Repository'
ALTER TABLE [dbo].[Repository]
ADD CONSTRAINT [PK_Repository]
    PRIMARY KEY CLUSTERED ([Name] ASC);
GO

-- Creating primary key on [Name] in table 'Role'
ALTER TABLE [dbo].[Role]
ADD CONSTRAINT [PK_Role]
    PRIMARY KEY CLUSTERED ([Name] ASC);
GO

-- Creating primary key on [Name] in table 'Team'
ALTER TABLE [dbo].[Team]
ADD CONSTRAINT [PK_Team]
    PRIMARY KEY CLUSTERED ([Name] ASC);
GO

-- Creating primary key on [Username] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([Username] ASC);
GO

-- Creating primary key on [Repositories_Name], [Teams_Name] in table 'TeamRepository_Permission'
ALTER TABLE [dbo].[TeamRepository_Permission]
ADD CONSTRAINT [PK_TeamRepository_Permission]
    PRIMARY KEY NONCLUSTERED ([Repositories_Name], [Teams_Name] ASC);
GO

-- Creating primary key on [AdministratedRepositories_Name], [Administrators_Username] in table 'UserRepository_Administrator'
ALTER TABLE [dbo].[UserRepository_Administrator]
ADD CONSTRAINT [PK_UserRepository_Administrator]
    PRIMARY KEY NONCLUSTERED ([AdministratedRepositories_Name], [Administrators_Username] ASC);
GO

-- Creating primary key on [Repositories_Name], [Users_Username] in table 'UserRepository_Permission'
ALTER TABLE [dbo].[UserRepository_Permission]
ADD CONSTRAINT [PK_UserRepository_Permission]
    PRIMARY KEY NONCLUSTERED ([Repositories_Name], [Users_Username] ASC);
GO

-- Creating primary key on [Roles_Name], [Users_Username] in table 'UserRole_InRole'
ALTER TABLE [dbo].[UserRole_InRole]
ADD CONSTRAINT [PK_UserRole_InRole]
    PRIMARY KEY NONCLUSTERED ([Roles_Name], [Users_Username] ASC);
GO

-- Creating primary key on [Teams_Name], [Members_Username] in table 'UserTeam_Member'
ALTER TABLE [dbo].[UserTeam_Member]
ADD CONSTRAINT [PK_UserTeam_Member]
    PRIMARY KEY NONCLUSTERED ([Teams_Name], [Members_Username] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Repositories_Name] in table 'TeamRepository_Permission'
ALTER TABLE [dbo].[TeamRepository_Permission]
ADD CONSTRAINT [FK_TeamRepository_Permission_Repository]
    FOREIGN KEY ([Repositories_Name])
    REFERENCES [dbo].[Repository]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Teams_Name] in table 'TeamRepository_Permission'
ALTER TABLE [dbo].[TeamRepository_Permission]
ADD CONSTRAINT [FK_TeamRepository_Permission_Team]
    FOREIGN KEY ([Teams_Name])
    REFERENCES [dbo].[Team]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamRepository_Permission_Team'
CREATE INDEX [IX_FK_TeamRepository_Permission_Team]
ON [dbo].[TeamRepository_Permission]
    ([Teams_Name]);
GO

-- Creating foreign key on [AdministratedRepositories_Name] in table 'UserRepository_Administrator'
ALTER TABLE [dbo].[UserRepository_Administrator]
ADD CONSTRAINT [FK_UserRepository_Administrator_Repository]
    FOREIGN KEY ([AdministratedRepositories_Name])
    REFERENCES [dbo].[Repository]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Administrators_Username] in table 'UserRepository_Administrator'
ALTER TABLE [dbo].[UserRepository_Administrator]
ADD CONSTRAINT [FK_UserRepository_Administrator_User]
    FOREIGN KEY ([Administrators_Username])
    REFERENCES [dbo].[User]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRepository_Administrator_User'
CREATE INDEX [IX_FK_UserRepository_Administrator_User]
ON [dbo].[UserRepository_Administrator]
    ([Administrators_Username]);
GO

-- Creating foreign key on [Repositories_Name] in table 'UserRepository_Permission'
ALTER TABLE [dbo].[UserRepository_Permission]
ADD CONSTRAINT [FK_UserRepository_Permission_Repository]
    FOREIGN KEY ([Repositories_Name])
    REFERENCES [dbo].[Repository]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_Username] in table 'UserRepository_Permission'
ALTER TABLE [dbo].[UserRepository_Permission]
ADD CONSTRAINT [FK_UserRepository_Permission_User]
    FOREIGN KEY ([Users_Username])
    REFERENCES [dbo].[User]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRepository_Permission_User'
CREATE INDEX [IX_FK_UserRepository_Permission_User]
ON [dbo].[UserRepository_Permission]
    ([Users_Username]);
GO

-- Creating foreign key on [Roles_Name] in table 'UserRole_InRole'
ALTER TABLE [dbo].[UserRole_InRole]
ADD CONSTRAINT [FK_UserRole_InRole_Role]
    FOREIGN KEY ([Roles_Name])
    REFERENCES [dbo].[Role]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_Username] in table 'UserRole_InRole'
ALTER TABLE [dbo].[UserRole_InRole]
ADD CONSTRAINT [FK_UserRole_InRole_User]
    FOREIGN KEY ([Users_Username])
    REFERENCES [dbo].[User]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRole_InRole_User'
CREATE INDEX [IX_FK_UserRole_InRole_User]
ON [dbo].[UserRole_InRole]
    ([Users_Username]);
GO

-- Creating foreign key on [Teams_Name] in table 'UserTeam_Member'
ALTER TABLE [dbo].[UserTeam_Member]
ADD CONSTRAINT [FK_UserTeam_Member_Team]
    FOREIGN KEY ([Teams_Name])
    REFERENCES [dbo].[Team]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Members_Username] in table 'UserTeam_Member'
ALTER TABLE [dbo].[UserTeam_Member]
ADD CONSTRAINT [FK_UserTeam_Member_User]
    FOREIGN KEY ([Members_Username])
    REFERENCES [dbo].[User]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTeam_Member_User'
CREATE INDEX [IX_FK_UserTeam_Member_User]
ON [dbo].[UserTeam_Member]
    ([Members_Username]);
GO


-- Insert admin user into database
INSERT [dbo].[User] ([Name], [Surname], [Username], [Password], [Email]) VALUES (N'Administrator', N'admin', N'admin', N'!#/)zW??C?JJ??', N'test@test.com')
GO
INSERT [dbo].[Role] ([Name], [Description]) VALUES (N'Administrator', N'System administrator')
GO
INSERT [dbo].[UserRole_InRole] ([Roles_Name], [Users_Username]) VALUES (N'Administrator', N'admin')
GO
