## Register user
POST

/users

Request body example:

{
    “email”: “john@doe.com”,
    “firstName”: “John”,
    “lastName”: “Doe”,
    “password”: “JohnDoe@1”
}

Response status code: 201

Response body: created user object

## Log in user
POST

/users/login

Request body example:

{
    “email”: “john@doe.com”,
    “password”: “JohnDoe@1”
}

Response status code: 200

Response body: JWT

## Get user
GET

/users/{userId}

Request must include following header:

Authorization: Bearer {JWT}

Response status code: 200

Response body: the user object

## Update user
PUT

/users/{userId}

Request body example:

{
    “firstName”: “Johnathan”,
    “lastName”: “Doe”
}

Request must include following header:

Authorization: Bearer {JWT}

Response status code: 200 OK

Response body: updated user object

## Additional info
Requests’ bodies must adopt JSON format, responses’ bodies do as well. In case of an error the following response will be sent:

Response status code: 400

Response body:

{
    “Success”: false,
    “ErrorMessage”: “Relevant error message!”
}

UserId is the id from the user object, not the e-mail.
