Weather App (CI/CD Pipeline)


    A modern, containerized .NET 10 Web API that integrates with third-party weather services (Open-Meteo) to provide real-time forecasting.
    This repository serves as a blueprint for implementing secure configuration patterns, Docker containerization, and fully automated
    Continuous Integration and Continuous Deployment (CI/CD) pipelines.


    FeaturesModern Stack: 
        Built entirely on the latest .NET 10 framework.
    Separation of Concerns: 
        Zero hardcoded configuration. API keys, base endpoints, and environment variables are cleanly separated from 
        application controllers using .NET Configuration (IConfiguration).Containerized Infrastructure: Fully Dockerized architecture utilizing
        multi-stage builds for optimized production images.

    Automated CI/CD: 
        Integrated GitHub Actions workflow that handles automatic building, dependency caching, testing, and continuous deployment tracking.
    
    Production Ready: 
        Configured to dynamically bind to assigned production
        ports (e.g., Render Web Services) with environment isolation.
    
    Architecture & Security
        To prevent sensitive keys and environment-specific
        endpoints from leaking into source control, the application uses decoupled environment configurations:
            Development Environment: Leverages .NET User Secrets and .env files locally to keep credentials off GitHub.
            Production Environment: Leverages Linux environment variables and cloud-side application secrets, mapped cleanly via the 
                                    double-underscore convention (__) to support .NET's hierarchical JSON configurations.


    Prerequisites
    Before running or deploying the application, ensure you have the following installed on your machine:
    .NET 10 SDK
    Docker Desktop
    Git

    Local Development Setup

        1.Clone the Repository
            git clone https://github.com/Skifta40/WeatherApp-CI-CD-Pipiline-Skifter-Bytyqi-2026.git
            cd WeatherApp-CI-CD-Pipiline-Skifter-Bytyqi-2026

        2. Configure Local SecretsInitialize user secrets inside your local project directory to load credentials outside of your Git root:
            dotnet user-secrets init
            dotnet user-secrets set "GeoCodeAPI:ApiKey" "64758a99a1db4f3e85d5c4e7eb8f4143"
            dotnet user-secrets set "OpenMeteo:BaseUrl" "https://api.open-meteo.com/v1/"
            dotnet user-secrets set "OpenCage:BaseUrl" "https://api.opencagedata.com/geocode/v1" 
        3. Run the Application Localy
            cd WeatherApp
            dotnet restore
            dotnet run 
    
        The API will spin up and be accessible locally at http://localhost:5000 or https://localhost:5001.

    Docker Containerization
    The application includes a highly optimized, multi-stage Dockerfile designed to isolate the SDK build tools from the lightweight 
    production runtime.
    Build and Run with Docker Locally:
        Build the image:
            docker build -t weather-app:latest .
        Run the container:
            Pass your local configurations directly into the container engine using environment flags: 
                docker run -d -p 10000:10000 \
                -e GeoCodeAPI__ApiKey="your_production_api_key" \
                -e OpenMeteo__BaseUrl="https://api.open-meteo.com/v1/" \
                -e OpenCage:BaseUrl="https://api.opencagedata.com/geocode/v1" \
                weather-app:latest


    CI/CD Pipeline (GitHub Actions)
    The repository includes a GitHub Actions workflow located in .github/workflows/. On every push to the main branch, the workflow:
    Checks out the source code safely.Configures a virtual .NET environment.Automatically restores NuGet packages.
    Compiles the binaries and executes all automated unit/integration tests with production-ready variable masks.
    Production Environment Mapping TableWhen deploying to cloud native hosts or orchestration layers (like Render, Docker Swarm, or Kubernetes), 
    register your configurations directly via the host dashboard using these exact keys:
    "GeoCodeAPI:ApiKey" "64758a99a1db4f3e85d5c4e7eb8f4143"
    "OpenMeteo:BaseUrl" "https://api.open-meteo.com/v1/"
    "OpenCage:BaseUrl" "https://api.opencagedata.com/geocode/v1" 
