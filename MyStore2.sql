-- 1) Tìm tên PK hiện tại trên dbo.[User] nếu cần (tùy DB)
SELECT name FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('dbo.[User]');

-- 2) Nếu Username đang là PK, drop constraint trước (ví dụ tên PK là PK_User_Username)
ALTER TABLE dbo.[User] DROP CONSTRAINT PK_User_Username;  -- CHỈ chạy nếu Username đang là PK

-- 3) Thêm cột UserID
ALTER TABLE dbo.[User]
ADD UserID INT IDENTITY(1,1);

-- 4) Đặt UserID làm Primary Key
ALTER TABLE dbo.[User]
ADD CONSTRAINT PK_User_UserID PRIMARY KEY CLUSTERED (UserID);

-- 5) Thêm cột UserID tạm vào Customer (nullable)
ALTER TABLE dbo.[Customer] ADD UserID INT NULL;

-- 6) Cập nhật Customer.UserID bằng join qua Username
UPDATE c
SET c.UserID = u.UserID
FROM dbo.Customer c
JOIN dbo.[User] u ON c.Username = u.Username;

-- 7) (Tùy bạn) nếu không còn dùng cột Customer.Username làm FK, drop FK constraint và drop column
-- Drop FK trước (tìm tên FK):
SELECT fk.name, OBJECT_NAME(fk.parent_object_id) AS TableName
FROM sys.foreign_keys fk
WHERE fk.referenced_object_id = OBJECT_ID('dbo.[User]');

-- Ví dụ drop FK (tên giả sử 'FK_User_Customer'):
ALTER TABLE dbo.Customer DROP CONSTRAINT FK_User_Customer;

-- 8) (Tuỳ) nếu muốn, xóa cột Customer.Username sau khi UserID đã fill
ALTER TABLE dbo.Customer DROP COLUMN Username;

-- 9) Tạo FK mới từ Customer(UserID) -> User(UserID)
ALTER TABLE dbo.Customer
ADD CONSTRAINT FK_Customer_User_UserID FOREIGN KEY (UserID) REFERENCES dbo.[User](UserID);

-- 10) Nếu muốn, set NOT NULL cho Customer.UserID (nếu mọi record đã có)
ALTER TABLE dbo.Customer ALTER COLUMN UserID INT NOT NULL;
