# PaymentAPI - WebAPI

## Overview

`PaymentAPI` is a robust and secure RESTful API built using **ASP.NET Core**. This API provides essential endpoints for managing payment-related operations, featuring secure authentication through **JSON Web Tokens (JWT)**. It is designed to be easily extendable and integrates seamlessly with other microservices. The project also includes comprehensive **API documentation** using **Swagger** for easy exploration and testing.

## Features

- **JWT Authentication**: Utilizes JSON Web Tokens for secure and stateless authentication.
- **Swagger UI**: Fully integrated Swagger documentation for easy access to API endpoints, allowing for real-time testing.
- **Configurability**: Flexible configuration options for JWT authentication settings, including Issuer, Audience, and Key, all configurable via the `appsettings.json` file.
- **Scalable Architecture**: Designed with scalability in mind, allowing for easy addition of new features and endpoints.

## Getting Started

Follow the instructions below to set up and run the `PaymentAPI` locally.

### Prerequisites

Ensure that the following are installed before setting up the project:

- [.NET 5 or higher](https://dotnet.microsoft.com/download)
- Visual Studio or any IDE of your choice
- Postman (or any other API testing tool)

## Installation

### Clone the Repository

   Begin by cloning the repository to your local machine:

```bash
git clone https://github.com/ismailaydemirx/WebAPI.git
   cd WebAPI
```

### Configure JWT Authentication Settings

Open the `appsettings.json` file and update the JWT authentication settings with your specific values. These settings are critical for token validation:

```json
"JwtOptions": {
  "Issuer": "yourIssuer",
  "Audience": "yourAudience",
  "Key": "yourSecretKey"
}
```

Ensure that the `Issuer`, `Audience`, and `Key` fields are updated to meet the needs of your application.

### Restore Dependencies

Execute the following command to restore the necessary project dependencies:

```bash
dotnet restore
```

### Run the Application

Now, you can run the application using the following command:

```bash
dotnet run
```

By default, the application will be hosted at [http://localhost:5000](http://localhost:5000).

### API Documentation

Once the application is running, you can access the interactive API documentation through Swagger UI at:

[http://localhost:5000/swagger](http://localhost:5000/swagger)

Swagger UI provides an intuitive platform for exploring and testing all available API endpoints directly in your browser.

### Authentication

The API uses JWT authentication to secure access to protected routes. To interact with these routes, you must include a valid JWT token in the `Authorization` header of your HTTP requests.

Example header format:

```plaintext
Authorization: Bearer <your_token>
```

## How to Obtain a JWT Token

1. **Login:** Make a POST request to `/api/auth/login` with valid user credentials.
2. **Token:** The response will include a JWT token, which can be used in the `Authorization` header for subsequent requests.

### Project Structure

The PaymentAPI project is organized as follows:

```lua
/PaymentAPI
|-- /Controllers
|   |-- PaymentController.cs
|-- /Models
|   |-- PaymentModel.cs
|-- /Services
|   |-- PaymentService.cs
|-- Startup.cs
|-- appsettings.json
```

### Key Files

- **Startup.cs:** Configures JWT authentication, token validation parameters, Swagger integration, and sets up routing, authentication, and authorization middleware for the API.
- **appsettings.json:** Contains configurable settings for JWT authentication (Issuer, Audience, Key).

### Endpoints

Here are the key API endpoints available in the application:

- **POST /api/auth/login:** Authenticates a user and returns a JWT token.
- **GET /api/payments:** Retrieves a list of all payments (protected endpoint, requires JWT token).
- **POST /api/payments:** Creates a new payment record (protected endpoint, requires JWT token).

### Error Handling

The API includes comprehensive error handling. Common error responses are formatted as JSON objects, containing an error field with a descriptive message.

Example error response:

```json
{
  "error": "Invalid token"
}
```

### Contributing

We welcome contributions to enhance and extend the functionality of this project. If you would like to contribute, please follow these steps:

1. Fork the repository.
2. Clone your fork locally.
3. Create a new branch for your changes.
4. Commit your changes.
5. Push your changes to your fork.
6. Submit a pull request to the main branch of the original repository.

## Code of Conduct

Please adhere to the project's code of conduct and contribute respectfully.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.

## Acknowledgements

This API is built using ASP.NET Core and is designed to be lightweight and performant.  
Swagger UI has been integrated for easier API documentation and testing.  
A big thank you to the open-source community for providing essential libraries and tools.  
For further details, please refer to the official ASP.NET Core documentation.

---
