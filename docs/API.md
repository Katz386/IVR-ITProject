# Api Reference

## Navigation

- [Diagram](#diagram)
- [Specification](#specification)
- [The services inside Docker](#the-services-inside-docker)
- [Data ownership](#data-ownership)
- [Asynchronous events](#asynchronous-events)
- [Requests and event flows](#requests-and-event-flows)


## Diagram

```mermaid
flowchart LR
    Browser[Web browser]
    Gateway[Nginx gateway]
    Auth[AuthService]
    Users[UserService]
    Articles[ArticlesService]
    Tasks[TasksService]
    Files[FileService]
    Platform[PlatformService]
    Broker[(RabbitMQ)]
    AuthDB[(Auth DB)]
    UserDB[(User DB)]
    ArticlesDB[(Articles DB)]
    TasksDB[(Tasks DB)]
    FilesDB[(Files DB)]
    PlatformDB[(Platform DB)]

    Browser -->|HTTPS / static assets| Gateway
    Gateway -->|/api/auth| Auth
    Gateway -->|/api/user| Users
    Gateway -->|/api/articles| Articles
    Gateway -->|/api/tasks| Tasks
    Gateway -->|/api/files| Files
    Gateway -->|/api/platform| Platform

    Auth --> AuthDB
    Users --> UserDB
    Articles --> ArticlesDB
    Tasks --> TasksDB
    Files --> FilesDB
    Platform --> PlatformDB

    Auth <--> Broker
    Users <--> Broker
    Articles <--> Broker
    Tasks <--> Broker
    Platform <--> Broker
```

# Specification

OpenApi documentation:
- [Articles service](docs/ArticlesService.WebApi.json)
- [Auth service](docs/ArticlesService.WebApi.json)
- [File service](docs/ArticlesService.WebApi.json)
- [Platform service](docs/ArticlesService.WebApi.json)
- [Articles service](docs/ArticlesService.WebApi.json)
- [Articles service](docs/ArticlesService.WebApi.json)

# The services inside Docker

| Public prefix | Internal service | Container port |
| --- | --- | ---: |
| `/api/auth/` | `authservice` | 8080 |
| `/api/articles/` | `articlesservice` | 8081 |
| `/api/tasks/` | `tasksservice` | 8082 |
| `/api/user/` | `userservice` | 8083 |
| `/api/files/` | `fileservice` | 8084 |
| `/api/platform/` | `platformservice` | 8085 |

# Data ownership

Each service has a dedicated PostgreSQL database container and named Docker volume:

| Service | Compose database container | Compose volume |
| --- | --- | --- |
| AuthService | `users-db` | `authService_data` |
| UserService | `userService-db` | `userService_data` |
| ArticlesService | `articles-db` | `articlesService_data` |
| TasksService | `tasks-db` | `tasksService_data` |
| FileService | `files-db` | `fileService_data` |
| PlatformService | `platform-db` | `platformService_data` |

>FileService also uses a mounted storage directory configured by `FileService_MountVolume` and `FileService_Storage`. The repository\`s `static_storage/` directory is mounted at `/app/static_storage` for static Markdown materials

# Asynchronous events

The shared message contracts are located in `src/backend/Shared/SharedContracts/`. MassTransit is used to publish and consume messages through RabbitMQ

| Event | Publisher | Consumer | effect |
| --- | --- | --- | --- |
| `AccountCreated` | AuthService | UserService | initializes a profile for the new account |
| `AccountCreatedStatistics` | AuthService | PlatformService | increments the user counter |
| `ArticleCreated` | ArticlesService | PlatformService | increments the article counter |
| `ArticleDeleted` | ArticlesService | PlatformService | decrements the article counter |
| `TaskCreated` | TasksService | PlatformService | increments the task counter |
| `TaskDeleted` | TasksService | PlatformService | decrements the task counter |
| `TaskSolved` | TasksService | UserService | updates the accounts solved-task progress |

# Requests and event flows

## Task submission

```mermaid
sequenceDiagram
    participant B as Browser
    participant N as Nginx
    participant T as TasksService
    participant R as RabbitMQ
    participant U as UserService

    B->>N: POST /api/tasks/{id}/solve + Bearer token
    N->>T: Forward request
    T->>T: Validate submitted flag
    alt Correct flag and authenticated user
        T->>R: Publish TaskSolved
        T-->>B: Return success response
        R-->>U: Deliver TaskSolved
        U->>U: Update profile progress
    else Incorrect flag or anonymous request
        T-->>B: Return validation result
    end
```

>Anonymous users can submit a flag for validation. A successful anonymous submission won`t affect the UserService

## User registration

```mermaid
sequenceDiagram
    participant B as Browser
    participant N as Nginx
    participant A as AuthService
    participant R as RabbitMQ
    participant U as UserService

    B->>N: POST /api/auth/sign-up
    N->>A: Forward request
    A->>A: Validate submitted creds
    alt Correct credentials
        A->>R: Publish AccountCreated
        A-->>B: Return success response
        R-->>U: Deliver AccountCreated
        U->>U: Create a new profile
    else Invalid creds or trying to create an existing user
        A-->>B: Return error
    end
```