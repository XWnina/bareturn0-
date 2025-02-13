# About Backend Functions And How to Use Them (From Backend)

*This file is for backend testing—covering database connections, API functionality, and backend logic.*

*All commands running under **/backend_express** directory*

## Content
- **A. Prerequisites and Testing**
- **B. All groups of Database**
  - user
  - savefiles  
- **C. Notes**

---

## A. Prerequisites and Testing
### 1. Simple Test: start the whole backend**  
```sh
node src/app.js
```
The MongoDB connection and backend root API should be connected.  
A successful start should print the following on the console:
```
Server starts successfully, running on http://localhost:3000
=====Request & Response Log (set on backend_express/src/utils/logger.js)=====
```
If not, continue with this part.

### 2. npm Install Checking
If the error statement contains "Error: Cannot find module 'express'" or "Error: Cannot find module 'mongoose'", it means the node_modules folder is missing or incomplete.  
Try:  
```sh
npm install
```
This would help the node_modules auto-downloading the packages needed.  

### 3. **/.env** file Checking  
Check the **.env** file, this file contains connection path with our MongoDB database with:  
```sh
MONGO_URI=mongodb+srv://wu:VU61r87uLSugBHe7@bareturn0.zxwhj.mongodb.net/?retryWrites=true&w=majority&appName=Bareturn0
``` 
And specifies our port of the local host of our backend with:  
```sh
PORT=3000
``` 
Also, the **JWT** secrete token is also defined here, but not related with the whole app connections at this moment:
```sh
JWT_SECRET=bareturn0
```

---

## B. All groups of Database

### 1. user group: `models/User.js`

#### Definition of User Group
**Parameters:**
- `username` (string) - The unique username for the user, unique, required.
- `password` (string) - The user’s password, required.

**Methods（Routes）:**
- `POST /users/register` → Register a new user.
- `POST /users/login` → Login and receive a token.
- `GET /users/me` → Get logged-in user details.
- `DELETE /users/:username` → Delete a user.

#### Functions
##### a. Create a new account:
```sh
curl -X POST http://localhost:3000/users/register \
     -H "Content-Type: application/json" \
     -d '{
           "username": "testuser",
           "password": "testpass123"
         }'
```
**Expected Output:**
```json
{
  "message": "User registered successfully"
}
```

##### b. Login:
```sh
curl -X POST http://localhost:3000/users/login \
     -H "Content-Type: application/json" \
     -d '{
           "username": "testuser",
           "password": "testpass123"
         }'
```
**Expected Output:**
```json
{
  "message": "Login successful",
  "token": "your_jwt_token_here"
}
```

##### c. Get Current User:
```sh
curl -X GET http://localhost:3000/users/me \
     -H "Authorization: Bearer your_jwt_token_here"
```
**Expected Output:**
```json
{
  "_id": "user_id_here",
  "username": "testuser"
}
```

##### d. Delete a User:
```sh
curl -X DELETE http://localhost:3000/users/testuser
```
**Expected Output:**
```json
{
  "message": "User 'testuser' deleted successfully"
}
```

### 2. savefiles group: `models/SaveFile.js`

#### Definition of SaveFile Group
**Parameters:**
- `saveName` (string) - Name of the save file.
- `progress` (integer) - Progress percentage.
- `coins` (integer) - Amount of coins.

**Methods:**
- `POST /savefiles` → Create a save file.
- `GET /savefiles` → Retrieve all save files.
- `PUT /savefiles/:saveName` → Update a save file.
- `DELETE /savefiles/:saveName` → Delete a save file.


##### a. Create a Save File:
```sh
curl -X POST http://localhost:3000/savefiles \
     -H "Authorization: Bearer your_jwt_token_here" \
     -H "Content-Type: application/json" \
     -d '{
           "saveName": "save1",
           "progress": 50,
           "coins": 100
         }'
```
**Expected Output:**
```json
{
  "message": "Save file created successfully",
  "saveName": "save1",
  "progress": 50,
  "coins": 100
}
```

##### b. Retrieve All Save Files:
```sh
curl -X GET http://localhost:3000/savefiles \
     -H "Authorization: Bearer your_jwt_token_here"
```
**Expected Output:**
```json
[
  {
    "saveName": "save1",
    "progress": 50,
    "coins": 100
  },
  {
    "saveName": "save2",
    "progress": 75,
    "coins": 200
  }
]
```

##### c. Update a Save File:
```sh
curl -X PUT http://localhost:3000/savefiles/save1 \
     -H "Authorization: Bearer your_jwt_token_here" \
     -H "Content-Type: application/json" \
     -d '{
           "progress": 80,
           "coins": 150
         }'
```
**Expected Output:**
```json
{
  "saveName": "save1",
  "progress": 80,
  "coins": 150
}
```

##### d. Delete a Save File:
```sh
curl -X DELETE http://localhost:3000/savefiles/save1 \
     -H "Authorization: Bearer your_jwt_token_here"
```
**Expected Output:**
```json
{
  "message": "Save deleted successfully"
}
```

---

## C. Notes
- For the Save File testing, replace `your_jwt_token_here` with the actual token from the login response accordingly.
- The `routes/userRoutes.js` also contains a function to get all current registered users--but we can always find other methods to check the whole database's status, so nvm.
- `Utils/logger.js` is a file that captures the request and response body and print it on the console, for easier testing and debugging. You can turn it off by setting the `LOGGING_ENABLED` variable to `False`.

---

