# Project - Stopify

## Type - Web Store

## Description

This is a simple Web Store project which 
sells technical products such as TVs, Laptops, 
Boilers, Air Conditioners etc. Guest Users can register
and login to their accounts.
Regular Users can view and order products with quantity.
Regular Users can order products by creating a contract 
with a Credit company.
The project also supports Administration. 
Administrators have all rights a Regular User has.
Administrators can also Promote and Demote Users.
Administrators can also add, edit or delete products to / from the shop. 
Administrators can also add, edit and delete Credit companies.
Credit companies provide convenient contracts to the Users.
Credit companies are just data entities moderated by an Administrator.

## Entities

### User
  - Id (string)
  - Username (string)
  - Password (string)
  - Email (string)
  - Full Name (string)
  - Phone Number (string)
### Product
  - Id (string)
  - Name (string)
  - Type (enum) (TV/AirConditioner/WashingMachine etc . . .)
  - Price (decimal)
  - ManufactoredOn (dateTime)
  - In Stock - (int:: quantity in stock)
### Order
  - Id (string)
  - IssuedOn (dateTime)
  - Quantity (int)
  - Product (Product)
  - Issuer (User)
### Credit Company
  - Id (string)
  - Name (string)
  - Active Since (dateTime)
  - Contracts (list of Contract)
### Credit Contracts
  - Id (string)
  - Issued On (dateTime)
  - Active Until (dateTime)
  - Price per Month (decimal)
  - Company (Credit Company)
  - Order (Order)
  - Contractor (User)