# CleanDDDArchitecture Template

The CleanDDDArchitecture Template is a comprehensive template project that utilises the [Aviant Library](https://github.com/panosru/Aviant) features to help you quickly get started with building clean, scalable, and maintainable applications following the principles of Vertical Sliced Clean Code Architecture. This template includes pre-configured Docker services for various dependencies and demonstrates best practices for DDD (Domain-Driven Design) and Event Sourcing.

## Features

- **Vertical Sliced Clean Code Architecture**: The template showcases a well-structured architecture that separates concerns vertically, allowing you to build maintainable applications.

- **Docker Services**: Easily spin up required services like Postgres, Kowl, Loki, EventStore, Kafka, and Redis using Docker containers, providing a ready-to-use development environment.

- **Multiple Example Applications**:
    - **WebApp with Razor Pages**: An example web application demonstrating how to create a user interface using Razor Pages.
    - **REST API**: A RESTful API example showcasing API design and best practices.

- **Four Domains**:
    - **Account**: Demonstrates the use of Event Sourcing and Identity, showing how to manage user accounts and identities.
    - **Todo**: Illustrates the concept of subdomains, providing a structured approach to managing tasks or to-do lists.
    - **Weather**: A simple domain, demonstrating the fundamental principles of DDD in a straightforward application context.
    - **Shared**: Provides cross-cutting concerns and utilities that can be shared across various domains.

## Getting Started

1. **Clone the Repository**: Clone this repository to your local development environment.

2. **Start Docker Services**: Run Docker to start the required services using the provided Docker Compose file.

    ```bash
    docker-compose up
    ```

3. **Explore Example Applications**: Dive into the example applications within the template to understand how to leverage the Aviant Library and implement Clean Code Architecture.

4. **Customize and Extend**: Use this template as a starting point for your project. Customize and extend the domains and applications to fit your specific requirements.

## Contribution

I welcome contributions from the community to enhance and improve the CleanDDDArchitecture Template. If you have ideas, bug fixes, or new features to add, please submit a pull request. Contributions related to documentation updates, code refactoring, and unit tests are highly encouraged.

## Support and Contact

For questions, feedback, or collaboration inquiries related to the CleanDDDArchitecture Template or the Aviant Library, please reach out via:

- [GitHub Issues](https://github.com/panosru/CleanDDDArchitecture/issues)

## License

This template is open-source and released under the MIT License. Please check the LICENSE file for more details.