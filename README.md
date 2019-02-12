# Introduction
Q-Construction is a prototype for formative test item (question) construction. It supports a light weight workflow enabling test experts and item authors to collaborate remotely using a predefined construction process that supports quality control.

 * Disclaimer: This is a prototype by [CitoLab](https://www.cito.nl/kennis-en-innovatie/citolab). This software should **not** be used as an production tool as is.

# Getting Started - Local Development

## Prerequisites

Q-Construction is built using the following frameworks and services:

1.  [MongoDB](https://www.mongodb.com/)
2.  [.NET Core](https://www.microsoft.com/net/download/)
3.  [Angular-cli](https://cli.angular.io/)
4.  [Auth0](https://auth0.com/)
5.  [Node.js](https://nodejs.org/)

## Setup

1.  Clone this repository
2.  Create an (free) account on Auth0 See: (https://auth0.com/docs/quickstart/spa/angular2/01-login)
3.  Create a least 2 Auth0 application:
      - An application for client authentication: Application Type = Single page application
      - An application for the back end: Application Type = Machine to machine. Make sure: Advanced Setting -> Grant Types --> Client Credentials in checked.
      - OIDC Conformant won't work on localhost. Optionally you could create an Single page application with OIDC Conformant = false for local development
4.  set OIDC Conformant to false and create at least one user and add the following json to app_metadata:
```
{
  "roles": [
    "Admin"
  ]
} 
```
5.  Create an appsettings.local.json file and override all settings of the appsettings.json file.
6.  Provision a mongo database.
7.  The created user (step 4) should be in the mongo database. From the project Tools/Tasks there is a task: SyncAuth0Users which adds all Auth0 Users to the mongo database.

From \backend\Tools\Tasks (make sure all settings in appsettings.json are correct) run: 
```sh
dotnet restore
dotnet run
```

## Build and run

1.  From backend/src run: 
```sh
dotnet restore
dotnet run
```
2.  From frontend
```sh
npm i
npm run start
```
3. Optionally you can seed the database with demo data by running the seed in Tools/Seed. The seed also contains example wishlist and screening list files.
