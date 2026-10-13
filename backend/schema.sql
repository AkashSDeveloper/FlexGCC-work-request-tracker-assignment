CREATE TABLE WorkRequests (
 Id INTEGER PRIMARY KEY AUTOINCREMENT, Title NVARCHAR(100) NOT NULL,
 ClientName NVARCHAR(120) NOT NULL, Description NVARCHAR(2000) NULL,
 Priority INTEGER NOT NULL, Status INTEGER NOT NULL, DueDate TEXT NOT NULL,
 CreatedDate TEXT NOT NULL, UpdatedDate TEXT NULL, Notes NVARCHAR(4000) NULL
);
CREATE INDEX IX_WorkRequests_Status ON WorkRequests(Status);
CREATE INDEX IX_WorkRequests_ClientName_Title ON WorkRequests(ClientName, Title);
-- Status index speeds status-filtered lists. Client/title index may help equality/prefix searches;
-- substring searches can require a full scan with a conventional B-tree index.
