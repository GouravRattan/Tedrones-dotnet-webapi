USE pc_student;
CREATE TABLE IF NOT EXISTS pc_student.All_Drones (
    DroneId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(10, 2) NOT NULL,
    ImageUrl VARCHAR(255),
    ImageThumbnailUrl VARCHAR(255)
);
INSERT INTO pc_student.All_Drones (Name, Description, Price, ImageUrl) VALUES
('DJI Mavic 3 Pro', '4/3 CMOS Hasselblad Camera | Dual Tele Cameras | Cine Only Tri-Camera Apple ProRes Support | 43-Min Max Flight Time | Omnidirectional Obstacle Sensing | 15km HD Video Transmission', '200', 'path/to/p1.jpg'),
('DJI Mavic 3 Pro', '4/3 CMOS Hasselblad Camera | Dual Tele Cameras | Cine Only Tri-Camera Apple ProRes Support | 43-Min Max Flight Time | Omnidirectional Obstacle Sensing | 15km HD Video Transmission', '200', 'path/to/p2.jpg'),
('DJI Mavic 3 Pro', '4/3 CMOS Hasselblad Camera | Dual Tele Cameras | Cine Only Tri-Camera Apple ProRes Support | 43-Min Max Flight Time | Omnidirectional Obstacle Sensing | 15km HD Video Transmission', '200', 'path/to/p3.jpg'),
('DJI Mavic 3 Pro', '4/3 CMOS Hasselblad Camera | Dual Tele Cameras | Cine Only Tri-Camera Apple ProRes Support | 43-Min Max Flight Time | Omnidirectional Obstacle Sensing | 15km HD Video Transmission', '200', 'path/to/p4.jpg'),
('DJI Mavic 3 Pro', '4/3 CMOS Hasselblad Camera | Dual Tele Cameras | Cine Only Tri-Camera Apple ProRes Support | 43-Min Max Flight Time | Omnidirectional Obstacle Sensing | 15km HD Video Transmission', '20', 'path/to/p5.jpg'),
('DJI Mavic 3 Pro', '4/3 CMOS Hasselblad Camera | Dual Tele Cameras | Cine Only Tri-Camera Apple ProRes Support | 43-Min Max Flight Time | Omnidirectional Obstacle Sensing | 15km HD Video Transmission', '200', 'path/to/p6.jpg');
SELECT * FROM pc_student.All_Drones;

CREATE TABLE IF NOT EXISTS pc_student.TEDrones_Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(15),
    UserPassword VARCHAR(100) NOT NULL,
    Address TEXT,
    RegistrationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLogin DATETIME,
    Role ENUM('User', 'Admin') NOT NULL DEFAULT 'User',
    ProfilePic VARCHAR(100),
    IsEmailVerified BOOLEAN NOT NULL DEFAULT FALSE,
    IsPhoneVerified BOOLEAN NOT NULL DEFAULT FALSE,
    UNIQUE (Email),
    UNIQUE (Phone)
);
SELECT * FROM pc_student.TEDrones_Users;

CREATE TABLE IF NOT EXISTS pc_student.TEDrones_Carts (
    CartId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    DroneId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    Price DECIMAL(10, 2) NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    AddedDateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('active', 'purchased', 'removed') NOT NULL DEFAULT 'active',
    Color VARCHAR(50),
    Size VARCHAR(50),
    Discount DECIMAL(10, 2) DEFAULT 0.00,
    ShippingMethod VARCHAR(100),
    ShippingCost DECIMAL(10, 2),
    LastUpdatedDateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES TEDrones_Users(UserId),
    FOREIGN KEY (DroneId) REFERENCES All_Drones(DroneId)
);
UPDATE pc_student.TEDrones_Carts
SET Quantity = 2,
    Price = 600.00,
    TotalPrice = 1200.00
WHERE CartId = 8;
ALTER TABLE pc_student.TEDrones_Carts MODIFY TotalPrice DECIMAL(10, 2) AS (Price * Quantity) STORED;
ALTER TABLE pc_student.TEDrones_Carts ADD COLUMN DroneId INT NOT NULL AFTER UserId;
INSERT INTO pc_student.TEDrones_Carts (UserId, Quantity, Price, TotalPrice) VALUES (1, 3, 500.00, 1500.00);
SET @CartId = LAST_INSERT_ID();
DROP TABLE IF EXISTS pc_student.TEDrones_Carts;
SELECT * FROM pc_student.TEDrones_Carts;

CREATE TABLE IF NOT EXISTS pc_student.TEDrones_CartItems (
    CartItemId INT AUTO_INCREMENT PRIMARY KEY,
    CartId INT NOT NULL,
    DroneId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    Price DECIMAL(10, 2) NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    AddedDateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('active', 'purchased', 'removed') NOT NULL DEFAULT 'active',
    Color VARCHAR(50),
    Size VARCHAR(50),
    Discount DECIMAL(10, 2) DEFAULT 0.00,
    ShippingMethod VARCHAR(100),
    ShippingCost DECIMAL(10, 2),
    LastUpdatedDateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CartId) REFERENCES pc_student.TEDrones_Carts(CartId),
    FOREIGN KEY (DroneId) REFERENCES All_Drones(DroneId)
);
UPDATE pc_student.TEDrones_CartItems
SET DroneId = 4,
    Quantity = 1,
    Price = 600.00,
    TotalPrice = 600.00,
    Color = 'Blue',
    Size = 'Medium',
    Discount = 10.00,
    ShippingMethod = 'Standard',
    ShippingCost = 5.00
WHERE CartItemId = 1;
INSERT INTO pc_student.TEDrones_CartItems (CartId, DroneId, Quantity, Price, TotalPrice, Color, Size, Discount, ShippingMethod, ShippingCost)
VALUES 
(@CartId, 1, 1, 500.00, 500.00, 'Red', 'Small',  0.00, 'Express', 5.00),
(@CartId, 2, 1, 500.00, 500.00, 'Green', 'Medium',  0.00, 'Express', 5.00),
(@CartId, 3, 1, 500.00, 500.00, 'Blue', 'Large', 0.00, 'Express', 10.00);
SELECT * FROM pc_student.TEDrones_CartItems;

CREATE TABLE IF NOT EXISTS pc_student.TEDrones_Contacts (
    ContactId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL,
    Phone VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Topic VARCHAR(255) NOT NULL,
    Message VARCHAR(255) NOT NULL,
    SentOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	ReadStatus ENUM('Read', 'Unread') NOT NULL DEFAULT 'Unread',
	Address VARCHAR(255) NOT NULL
);
TRUNCate Table pc_student.TEDrones_Contacts;
SELECT * FROM pc_student.TEDrones_Contacts;

CREATE TABLE IF NOT EXISTS pc_student.TEDrones_Sessions (
    SessionId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UserStatus ENUM('Active', 'Inactive') NOT NULL DEFAULT 'Active',
    Token LONGTEXT NOT NULL,
    UserId VARCHAR(255) NOT NULL
);
TRUNCATE TABLE pc_student.TEDrones_Sessions;
SELECT * FROM pc_student.TEDrones_Sessions;

