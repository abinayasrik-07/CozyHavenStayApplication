--CREATE DATABASE CozyHavenStay
--USE CozyHavenStay

CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Locations (
    LocationId INT PRIMARY KEY IDENTITY(1,1),
    City VARCHAR(50) NOT NULL,
    Country VARCHAR(30) NOT NULL
);

CREATE TABLE ProofOfUser (
    ProofTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(50) NOT NULL,
    Email VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(30) UNIQUE,
	ProofTypeId INT DEFAULT NULL,
	RoleId INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
	FOREIGN KEY (ProofTypeId) REFERENCES ProofOfUser(ProofTypeId),
	FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
);

CREATE TABLE StarRating (
    StarRatingId INT PRIMARY KEY IDENTITY(1,1),
    Rating INT NOT NULL UNIQUE CHECK (Rating BETWEEN 1 AND 5) 
);

CREATE TABLE Hotels (
    HotelId INT PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL,
    HotelName NVARCHAR(100) NOT NULL,
    LocationId INT NOT NULL,
    [Description] NVARCHAR(MAX),
    StarRatingId INT NOT NULL,
    FOREIGN KEY (LocationId) REFERENCES Locations(LocationId) ON DELETE CASCADE,
	FOREIGN KEY (UserId) REFERENCES Users(UserId),
	FOREIGN KEY (StarRatingId) REFERENCES StarRating(StarRatingId)
);

CREATE TABLE RoomType (
    RoomTypeId INT IDENTITY(1,1) PRIMARY KEY,
    RoomTypeName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Rooms (
    RoomId INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    Size NVARCHAR(50) NOT NULL,
    RoomTypeId INT NOT NULL,
    BaseFare DECIMAL(10, 2) NOT NULL,
    MaxOccupancy INT NOT NULL,
    IsAC BIT DEFAULT 1,
    IsAvailable BIT DEFAULT 1,
    FOREIGN KEY (HotelId) REFERENCES Hotels(HotelId) ON DELETE CASCADE,
	FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId)
);

CREATE TABLE Amenities (
    AmenityId INT PRIMARY KEY IDENTITY(1,1),
    AmenityName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE HotelAmenities (
    HotelId INT NOT NULL,
    AmenityId INT NOT NULL,
    PRIMARY KEY (HotelId, AmenityId),
    FOREIGN KEY (HotelId) REFERENCES Hotels(HotelId) ON DELETE CASCADE,
    FOREIGN KEY (AmenityId) REFERENCES Amenities(AmenityId) ON DELETE CASCADE
);

CREATE TABLE Bookings (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    RoomId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    BookingStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    DateOfBooking DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);

CREATE TABLE Discounts (
    DiscountId INT IDENTITY(1,1) PRIMARY KEY,
	BookingId INT NOT NULL,
    DiscountCode NVARCHAR(50) UNIQUE NOT NULL,
    DiscountPercentage DECIMAL(5,2) CHECK (DiscountPercentage BETWEEN 0 AND 100) NOT NULL,
    AppliedAt DATETIME2 DEFAULT SYSDATETIME(),
    ExpiryDate DATETIME NOT NULL,
	FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE
);

CREATE TABLE PaymentMethod (
    PaymentMethodId INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    BookingId INT NOT NULL,
	DiscountId INT NULL, 
    Amount DECIMAL(10, 2) NOT NULL,
	FinalAmount DECIMAL(10, 2) NOT NULL DEFAULT 0,
    PaymentMethodId INT NOT NULL,
    PaymentStatus NVARCHAR(50) NOT NULL CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed')),
    TransactionId NVARCHAR(100) NOT NULL,
    PaymentDate DATETIME2 DEFAULT SYSDATETIME(),
	FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethod(PaymentMethodId),
	FOREIGN KEY (DiscountId) REFERENCES Discounts(DiscountId),
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE
);

CREATE TABLE Reviews (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    BookingId INT NOT NULL,
    HotelId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    DatePosted DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE,
    FOREIGN KEY (HotelId) REFERENCES Hotels(HotelId)
);

CREATE TABLE Refunds (
    RefundId INT IDENTITY(1,1) PRIMARY KEY,
    PaymentId INT NOT NULL FOREIGN KEY REFERENCES Payments(PaymentId),
    RefundAmount DECIMAL(10,2) NOT NULL,
    RefundStatus NVARCHAR(20) CHECK (RefundStatus IN ('Processing', 'Completed', 'Rejected')) NOT NULL DEFAULT 'Processing',
    RefundDate DATETIME2 DEFAULT SYSDATETIME()
);

CREATE TABLE BookingCancellation (
    CancellationId INT PRIMARY KEY IDENTITY(1,1),
    BookingId INT NOT NULL,
    CancellationDate DATETIME2 DEFAULT SYSDATETIME(),
    ReasonForCancellation NVARCHAR(255),
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE
);

SELECT 
    bc.CancellationId,
    bc.BookingId,
    bc.ReasonForCancellation,
    r.RefundId,
    r.RefundAmount,
    r.RefundStatus
FROM BookingCancellation bc
JOIN Bookings b ON bc.BookingId = b.BookingId
JOIN Payments p ON p.BookingId = b.BookingId
LEFT JOIN Refunds r ON r.PaymentId = p.PaymentId
WHERE b.BookingStatus = 'Cancelled';

GO
CREATE PROCEDURE usp_ProcessPayment
    @BookingId INT,
    @Amount DECIMAL(10, 2),
    @PaymentMethodId INT,
    @TransactionId NVARCHAR(100),
    @DiscountId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @FinalAmount DECIMAL(10, 2) = @Amount;
    -- Apply discount if provided
    IF @DiscountId IS NOT NULL
    BEGIN
        SELECT @FinalAmount = @Amount * (1 - DiscountPercentage/100)
        FROM Discounts
        WHERE DiscountId = @DiscountId;
    END
    INSERT INTO Payments (
        BookingId, DiscountId, Amount, 
        FinalAmount, PaymentMethodId, 
        PaymentStatus, TransactionId
    )
    VALUES (
        @BookingId, @DiscountId, @Amount,
        @FinalAmount, @PaymentMethodId,
        'Completed', @TransactionId
    );
    SELECT SCOPE_IDENTITY() AS NewPaymentId;
END;