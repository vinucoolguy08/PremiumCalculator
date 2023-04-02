
# Monthly Premium Calculator

This project is a web application that allows members to calculate their monthly insurance premiums based on their
personal information, including their name, age, date of birth, occupation, and sum insured.

The application is built using .NET Core 6 as the backend, React as the frontend, and SQL Server as the database.

## Technologies Used
* .NET Core 6
* React
* SQL Server
* MediatR
* Fluent Validation
* XUnit Framework

## Deployment

The application is hosted on Azure Web App and can be accessed using below url
####  https://calculatepremiumtest.azurewebsites.net/

### Features
* User-friendly UI that accepts the user's personal information and occupation to calculate monthly premiums
* Dropdown list of occupations with their corresponding rating factors
* Premium calculation based on the user's information using the provided formula
* Input validation using Fluent Validation
* Exception handling using middlewares
* CQRS design pattern using MediatR
* Unit testing using XUnit

### Calculation

Calculation
For any given individual, the monthly premium is calculated using the below formula:

Death Premium = (Sum Insured * Occupation Rating Factor * Age) /1000 * 12

TPD Premium Monthly = (Sum Insured * Occupation Rating Factor * Age) /1234

All input fields are mandatory. Given all the input fields are specified, a change in the occupation dropdown triggers the premium calculation.


### How to Run the Application
1. Clone the repository
2. Navigate to the project directory in the terminal
3. Run dotnet restore to install the required packages
4. Run dotnet run to start the backend server
5. Navigate to the client directory using another terminal instance
6. Run npm install to install the required packages
7. Run npm start to start the frontend server

### How to Run the Application
1. Navigate to the project directory in the terminal
2. Run dotnet test to run the unit tests