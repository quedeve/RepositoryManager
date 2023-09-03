# Repository Manager Documentation

## Introduction

The Repository Manager is a .NET application designed to store and manage items with different content types (JSON and XML). It provides an interface to register, retrieve, get the type of, and deregister items in a persistent SQLite database.

## Table of Contents

1. [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Initialization](#initialization)

2. [Usage](#usage)
   - [Register an Item](#register-an-item)
   - [Retrieve an Item](#retrieve-an-item)
   - [Get the Type of an Item](#get-the-type-of-an-item)
   - [Deregister an Item](#deregister-an-item)

3. [Unit Tests](#unit-tests)
   - [Running Unit Tests](#running-unit-tests)

## Getting Started

### Prerequisites

Before using the Repository Manager, make sure you have the following prerequisites installed:

- .NET 5.0 SDK or later
- SQLite database engine

### Initialization

To use the Repository Manager, you need to initialize it by creating an instance of the `Repository` class. This initializes the SQLite database and creates the necessary `Items` table.

```csharp
try
{
    IRepository repository = new Repository();
    await repository.InitializeAsync();
}
catch (Exception ex)
{
    // Handle initialization error
    Console.WriteLine($"Failed to initialize repository: {ex.Message}");
}
```

## Usage

### Register an Item

You can register an item in the repository using the `RegisterAsync` method. You need to provide the item's name, content, and type (1 for JSON, 2 for XML).

```csharp
string itemName = "ItemName";
string itemContent = "{\"key\": \"value\"}"; // JSON content
int itemType = 1; // 1 for JSON, 2 for XML

await repository.RegisterAsync(itemName, itemContent, itemType);
```

### Retrieve an Item

Retrieve an item from the repository using the `RetrieveAsync` method by providing the item's name. It returns the content of the item.

```csharp
string itemName = "ItemName";
string itemContent = await repository.RetrieveAsync(itemName);

if (itemContent != null)
{
    // Item found, process itemContent
}
else
{
    // Item not found
}
```

### Get the Type of an Item

You can get the type (1 for JSON, 2 for XML) of an item using the `GetTypeAsync` method by providing the item's name.

```csharp
string itemName = "ItemName";
int itemType = await repository.GetTypeAsync(itemName);

if (itemType != -1)
{
    // Process itemType (1 for JSON, 2 for XML)
}
else
{
    // Item not found
}
```

### Deregister an Item

To remove an item from the repository, use the `DeregisterAsync` method by providing the item's name.

```csharp
string itemName = "ItemName";
await repository.DeregisterAsync(itemName);
```

## Unit Tests

The Repository Manager includes unit tests to ensure the functionality of the repository operations.

### Running Unit Tests

You can run the unit tests to verify the correctness of the Repository Manager's behavior. Use a testing framework like xUnit to execute the tests.

```bash
dotnet test RepositoryManagerTests.csproj
```

Make sure to configure your testing environment and set up the database connection accordingly for successful testing.

This concludes the documentation for the Repository Manager, which allows you to manage items with different content types in a SQLite database.
