# WalkthisWay Online Shoe Store

## Database Tables

### StoreEmployee Table
```sql
CREATE TABLE StoreEmployee (
    empId CHAR(7) NOT NULL CHECK (LENGTH(empId) = 7),
    eName TEXT NOT NULL,
    eAddress TEXT NULL,
    ePostCode CHAR(4) NULL CHECK (LENGTH(ePostCode) = 4),
    eEmail TEXT NULL,
    eMobPh TEXT NULL,
    eStartDate DATE NULL,
    storeId CHAR(7) NOT NULL,
    PRIMARY KEY (empId),
    FOREIGN KEY (storeId) REFERENCES Store(storeId)
);

CREATE TABLE Customer (
    custId CHAR(7) NOT NULL CHECK(LENGTH(custId) = 7),
    cName TEXT NOT NULL,
    cMobilePh TEXT NULL,
    cEmail TEXT NULL,
    cBirthDate DATE NULL,
    PRIMARY KEY (custId)
);

CREATE TABLE `Order` (
    orderId CHAR(7) NOT NULL CHECK(LENGTH(orderId) = 7),
    oDate DATE NULL,
    total INT NULL,
    GST INT NULL,
    deliveryAddress TEXT NULL,
    orderStatus TEXT NULL,
    custId CHAR(7) NOT NULL,
    PRIMARY KEY (orderId),
    FOREIGN KEY (custId) REFERENCES Customer(custId)
);

CREATE TABLE OrderDetail (
    orderId CHAR(7) NOT NULL CHECK(LENGTH(orderId) = 7),
    prodId CHAR(7) NOT NULL CHECK(LENGTH(prodId) = 7),
    quantity INT NULL,
    retailPrice INT NULL,
    CONSTRAINT PK_OrderDetails PRIMARY KEY (orderId, prodId),
    FOREIGN KEY (orderId) REFERENCES `Order`(orderId),
    FOREIGN KEY (prodId) REFERENCES Product(prodId)
);

SELECT * FROM OrderDetail;

CREATE TABLE Payment (
    payId CHAR(7) NOT NULL CHECK(LENGTH(payId) = 7),
    Type TEXT NULL,
    amount INT NULL,
    pDate DATE NULL,
    bankTransactNo TEXT NULL,
    orderId CHAR(7) NOT NULL,
    PRIMARY KEY (payId),
    FOREIGN KEY (orderId) REFERENCES `Order`(orderId)
);

CREATE TABLE Product (
    prodId CHAR(7) NOT NULL CHECK(LENGTH(prodId) = 7),
    size TEXT NULL,
    colour TEXT NULL,
    style TEXT NULL,
    qtyOnHand INT NULL,
    reorderQty INT NULL,
    retailPrice INT NULL,
    suplrId CHAR(7) NOT NULL,
    PRIMARY KEY (prodId),
    FOREIGN KEY (suplrId) REFERENCES Supplier(suplrId)
);

CREATE TABLE ProductSupplier (
    prodId CHAR(7) NOT NULL,
    suplrId CHAR(7) NOT NULL,
    CONSTRAINT PK_ProdSuplr PRIMARY KEY (prodId, suplrId),
    FOREIGN KEY (prodId) REFERENCES Product(prodId),
    FOREIGN KEY (suplrId) REFERENCES Supplier(suplrId)
);

CREATE TABLE Supplier (
    suplrId CHAR(7) NOT NULL CHECK(LENGTH(suplrId) = 7),
    suplrName TEXT NOT NULL,
    suplrAddress TEXT NULL,
    suplrPostCode CHAR(4) NULL CHECK(LENGTH(suplrPostCode) = 4),
    suplrEmail TEXT NULL,
    suplrMobPh TEXT NULL,
    PRIMARY KEY (suplrId)
);
```

### Guests who visited more than twice
```sql
SELECT a.guestNo, b.guestName
FROM Booking a, Guest b
WHERE a.guestNo = b.guestNo
GROUP BY b.guestNo
HAVING COUNT(b.guestNo) > 2;

```
### Guest names who visited more than twice
```sql
SELECT e.guestNAME
FROM Guest e 
WHERE e.guestNO IN (
    SELECT d.guestNO
    FROM Booking d
    GROUP BY d.guestNO
    HAVING COUNT(d.guestNO) > 2
);

```
### Total income from Ritz Hotel bookings
```sql
SELECT SUM(b.price * DATEDIFF(a.dateTo, a.dateFrom)) AS TotalSum_RitzHotel
FROM Booking a
JOIN Room b ON a.hotelNo = b.hotelNo AND a.roomNo = b.roomNo
WHERE a.hotelNo IN (SELECT hotelNo FROM Hotel WHERE hotelName = 'Ritz Hotel')
HAVING SUM(b.price * DATEDIFF(a.dateTo, a.dateFrom));

```
### Hotels with 2 or more Family rooms
```sql
SELECT hotelNo
FROM Room
WHERE roomType = 'Family'
GROUP BY hotelNo
HAVING COUNT(roomType) >= 2
ORDER BY hotelNo;

```
### Create index on guestName
```sql
CREATE INDEX guestName ON Guest (guestName);
SHOW INDEX FROM Guest;

```sql
### Create users
```sql
CREATE USER 'Jane'@'localhost' IDENTIFIED BY 'new_password';
CREATE USER 'Alex'@'localhost' IDENTIFIED BY 'old_password' PASSWORD EXPIRE;

```
### Show users and grants
```sql
SELECT * FROM mysql.user;
SHOW GRANTS FOR 'Jane'@'localhost';

```
### Insert sample hotel data
```sql
INSERT INTO Hotel (hotelNo, hotelName, city) VALUES ('H8', 'The Delightful Hotel', 'Brisbane');
INSERT INTO Room (roomNo, hotelNo, roomType, price) VALUES ('R1', 'H8', 'Executive', '130');
INSERT INTO Guest (guestNo, guestName, guestAddress) VALUES ('G6', 'Matt Damon', 'New York');
INSERT INTO Booking (hotelNo, guestNo, dateFrom, dateTo, roomNo) VALUES ('H8', 'G6', '2023-08-12', '2023-08-15', 'R1');

```
### Update room prices
```sql
UPDATE Room SET price = price * 1.25;
SELECT * FROM Room;

```
### Create and query view for Longreach hotels
```sql
CREATE VIEW Longreach_Hotel_Info AS
SELECT a.hotelName, b.roomType, COUNT(c.roomNo) AS total_roomBooked
FROM Hotel a
JOIN Room b ON a.hotelNo = b.hotelNo
JOIN Booking c ON b.roomNo = c.roomNo
WHERE a.city = 'Longreach'
GROUP BY a.hotelName, b.roomType
ORDER BY a.hotelName;

SELECT * FROM Longreach_Hotel_Info;

DROP VIEW Longreach_Hotel_Info;

