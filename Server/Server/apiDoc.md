# WritingServer API Documentation

This document outlines all available API endpoints in the WritingServer application.

## Table of Contents
- [Authentication](#authentication)
- [Admin API](#admin-api)
    - [User Management](#admin-user-management)
    - [Question Set Management](#admin-question-set-management)
    - [Question Management](#admin-question-management)
    - [Response Management](#admin-response-management)
- [User API](#user-api)
    - [Profile Management](#user-profile-management)
    - [Questions](#user-questions)
    - [Responses](#user-responses)

## Authentication

### Admin Login
Authenticates an admin user and returns an access token.

**Endpoint:** `POST /api/admin/login`

**Request Body:**
```json
{
  "loginId": "string"
}
```

**Response:**
```json
{
  "success": true,
  "accessToken": "string",
  "errorMessage": null
}
```

### User Login
Authenticates a user with their PIN and returns an access token.

**Endpoint:** `POST /api/user/login`

**Request Body:**
```json
{
  "pin": "string"
}
```

**Response:**
```json
{
  "success": true,
  "accessToken": "string",
  "errorMessage": null
}
```

## Admin API

All admin endpoints require authentication using the `Authorization: Bearer {token}` header.

### Admin User Management

#### List Users
Returns a list of all users.

**Endpoint:** `GET /api/admin/users`

**Response:**
```json
{
  "success": true,
  "users": {
    "users": [
      {
        "username": "string",
        "pin": "string",
        "resetState": false
      }
    ]
  },
  "errorMessage": null
}
```

#### Add User
Creates a new user with the specified username.

**Endpoint:** `POST /api/admin/users`

**Request Body:**
```json
{
  "userName": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

#### Edit User
Updates a user's username.

**Endpoint:** `PUT /api/admin/users`

**Request Body:**
```json
{
  "userName": "string",
  "newUserName": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

#### Reset User
Resets a user's responses and sets their reset state.

**Endpoint:** `PUT /api/admin/users/reset`

**Request Body:**
```json
{
  "userName": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

#### Delete User
Deletes a user and all associated responses.

**Endpoint:** `DELETE /api/admin/users`

**Request Body:**
```json
{
  "userName": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

### Admin Question Set Management

#### List Question Sets
Returns all question sets.

**Endpoint:** `GET /api/admin/questionsets`

**Response:**
```json
{
  "success": true,
  "questionSets": {
    "questionSets": [
      {
        "questionSetId": "string",
        "questionSetName": "string",
        "questionSetOrder": 0,
        "locked": false
      }
    ]
  },
  "errorMessage": null
}
```

#### Create Question Set
Creates a new question set.

**Endpoint:** `POST /api/admin/questionsets`

**Request Body:**
```json
{
  "questionSetName": "string"
}
```

**Response:**
```json
{
  "success": true,
  "questionSet": {
    "questionSetId": "string",
    "questionSetName": "string",
    "questionSetOrder": 0,
    "locked": false
  },
  "errorMessage": null
}
```

#### Update Question Set Name
Updates a question set's name.

**Endpoint:** `PUT /api/admin/questionsets/name`

**Request Body:**
```json
{
  "questionSetId": "string",
  "newQuestionSetName": "string"
}
```

**Response:**
```json
{
  "success": true,
  "questionSet": {
    "questionSetId": "string",
    "questionSetName": "string",
    "questionSetOrder": 0,
    "locked": false
  },
  "errorMessage": null
}
```

#### Update Question Set Order
Updates a question set's display order.

**Endpoint:** `PUT /api/admin/questionsets/order`

**Request Body:**
```json
{
  "questionSetId": "string",
  "questionSetOrder": 0
}
```

**Response:**
```json
{
  "success": true,
  "questionSet": {
    "questionSetId": "string",
    "questionSetName": "string",
    "questionSetOrder": 0,
    "locked": false
  },
  "errorMessage": null
}
```

#### Lock/Unlock Question Set
Toggles a question set's locked status.

**Endpoint:** `PUT /api/admin/questionsets/lock`

**Request Body:**
```json
{
  "questionSetId": "string",
  "locked": true
}
```

**Response:**
```json
{
  "success": true,
  "questionSet": {
    "questionSetId": "string",
    "questionSetName": "string",
    "questionSetOrder": 0,
    "locked": true
  },
  "errorMessage": null
}
```

#### Delete Question Set
Deletes a question set and all associated questions.

**Endpoint:** `DELETE /api/admin/questionsets`

**Request Body:**
```json
{
  "questionSetId": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

### Admin Question Management

#### List Questions in a Set
Returns all questions in a specified question set.

**Endpoint:** `GET /api/admin/questions?questionSetId={questionSetId}`

**Response:**
```json
{
  "success": true,
  "questions": {
    "questions": [
      {
        "questionId": "string",
        "questionSetId": "string",
        "questionText": "string",
        "expectedResultText": "string",
        "questionOrder": 0,
        "minWordLength": 0,
        "maxWordLength": 0
      }
    ]
  },
  "errorMessage": null
}
```

#### Create Question
Creates a new question in a question set.

**Endpoint:** `POST /api/admin/questions`

**Request Body:**
```json
{
  "questionSetId": "string",
  "questionText": "string",
  "expectedResultText": "string",
  "questionOrder": 0,
  "minWordLength": 0,
  "maxWordLength": 0
}
```

**Response:**
```json
{
  "success": true,
  "questionModels": {
    "questionId": "string",
    "questionSetId": "string",
    "questionText": "string",
    "expectedResultText": "string",
    "questionOrder": 0,
    "minWordLength": 0,
    "maxWordLength": 0
  },
  "errorMessage": null
}
```

#### Update Question
Updates an existing question.

**Endpoint:** `PUT /api/admin/questions`

**Request Body:**
```json
{
  "questionId": "string",
  "questionText": "string",
  "expectedResultText": "string",
  "questionOrder": 0,
  "minWordLength": 0,
  "maxWordLength": 0
}
```

**Response:**
```json
{
  "success": true,
  "questionModels": {
    "questionId": "string",
    "questionSetId": "string",
    "questionText": "string",
    "expectedResultText": "string",
    "questionOrder": 0,
    "minWordLength": 0,
    "maxWordLength": 0
  },
  "errorMessage": null
}
```

#### Delete Question
Deletes a question and associated responses.

**Endpoint:** `DELETE /api/admin/questions`

**Request Body:**
```json
{
  "questionId": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

### Admin Response Management

#### Get Responses for a Question
Returns all user responses for a specific question.

**Endpoint:** `GET /api/admin/responses?questionId={questionId}`

**Response:**
```json
{
  "success": true,
  "responses": {
    "responses": [
      {
        "responseId": "string",
        "questionId": "string",
        "userName": "string",
        "responseText": "string",
        "timestamp": "string"
      }
    ]
  },
  "errorMessage": null
}
```

## User API

All user endpoints require authentication using the `Authorization: Bearer {token}` header.

### User Profile Management

#### Get Username
Returns the current user's username.

**Endpoint:** `GET /api/user/username`

**Response:**
```json
{
  "success": true,
  "username": "string",
  "errorMessage": null
}
```

#### Get Reset State
Returns whether the user's account has been reset.

**Endpoint:** `GET /api/user/state`

**Response:**
```json
{
  "success": true,
  "resetState": false,
  "errorMessage": null
}
```

### User Questions

#### Get Question Sets
Returns all unlocked question sets.

**Endpoint:** `GET /api/user/questionsets`

**Response:**
```json
{
  "success": true,
  "questionSets": {
    "questionSets": [
      {
        "questionSetId": "string",
        "questionSetName": "string",
        "questionSetOrder": 0,
        "locked": false
      }
    ]
  },
  "errorMessage": null
}
```

#### Get Questions from Set
Returns all questions in a specified question set.

**Endpoint:** `GET /api/user/questions?questionSetId={questionSetId}`

**Response:**
```json
{
  "success": true,
  "questions": {
    "questions": [
      {
        "questionId": "string",
        "questionSetId": "string",
        "questionText": "string",
        "expectedResultText": "string",
        "questionOrder": 0,
        "minWordLength": 0,
        "maxWordLength": 0
      }
    ]
  },
  "errorMessage": null
}
```

### User Responses

#### Submit Response
Submits a user's response to a question.

**Endpoint:** `POST /api/user/responses`

**Request Body:**
```json
{
  "questionId": "string",
  "responseText": "string"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null
}
```

#### Get User Responses
Returns all of the current user's responses to a specific question.

**Endpoint:** `GET /api/user/responses?questionId={questionId}`

**Response:**
```json
{
  "success": true,
  "responses": {
    "responses": [
      {
        "responseId": "string",
        "questionId": "string",
        "userName": "string",
        "responseText": "string",
        "timestamp": "string"
      }
    ]
  },
  "errorMessage": null
}
```