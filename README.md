# RazorHx

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

This small helper library seamlessly combines HTMX with the latest Razor Components in the ASP.NET Core world. With this library, you can effortlessly return HTML through Minimal API endpoints, which is then rendered by Razor Components. Streamline development by harnessing the power of HTMX alongside the flexibility of Razor Components and Minimal APIs.

## Features

- Seamless integration of HTMX with Razor Components.
- Return HTML rendered by Razor Components via Minimal API endpoints.
- Accelerate the development of dynamic web applications in ASP.NET Core.

## Installation

To use RazorHx in your ASP.NET Core project, you only need to install the NuGet package. Open your project in the Package Manager Console and run the following command:
```bash
Install-Package RazorHx
```

Alternatively, you can use the .NET CLI with the following command:
```bash
dotnet add package RazorHx
```

## Usage

Here's a quick example of how to use the library:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorHxComponents(options => {
    options.RootComponent = typeof(Index);
});

var app = builder.Build();

app.UseRazorHxComponents();

app.MapGet("/", () => new RazorHxComponentResult<Hello>());

app.Run();
```

## Roadmap

Our development roadmap outlines the upcoming features and improvements planned for the RazorHx for ASP.NET Core.

### 1. Partial HTML Result based on HTMX Requests

- **Description:** Implement the ability to return partial HTML results based on specific htmx requests, enabling more granular updates to the user interface.
- **Status:** In Progress
- **Target Release:** v1.0

### 2. Layout Support

- **Description:** Enhance the library to provide improved support for layouts, allowing for more versatile and dynamic page structures.
- **Status:** In Progress
- **Target Release:** v1.0

### 3. Form Handling

- **Description:** Integrate robust form handling capabilities, making it easier to work with forms and dynamic form submissions.
- **Status:** Planned
- **Target Release:** v1.0

## Contributing

We welcome contributions and feedback on these planned features. If you have specific features you'd like to see or would like to contribute to the development, please check our [contribution guidelines](CONTRIBUTING.md)

## License

This project is licensed under the MIT License.

Note: This library might still be in the development phase. We welcome your contributions and feedback!
