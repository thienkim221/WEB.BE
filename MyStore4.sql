ALTER TABLE Category
ADD Description NVARCHAR(MAX) NULL;
ALTER TABLE [Order]
ADD 
    PaymentMethod NVARCHAR(255) NULL,
    ShippingMethod NVARCHAR(255) NULL,
    ShippingAddress NVARCHAR(MAX) NULL;
ALTER TABLE Customer
ADD CONSTRAINT FK_Customer_User
FOREIGN KEY (UserID) REFERENCES [User](UserID);
