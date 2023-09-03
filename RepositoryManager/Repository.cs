using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryManager.Model;

public class Repository : IRepository
{
    private readonly DataContext dbContext;
    private bool isInitialized;

    public Repository()
    {
        var connectionString = "Data Source=RepositoryManager.db"; // Adjust the connection string as needed
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connectionString)
            .Options;

        dbContext = new DataContext(options);
        isInitialized = false;
    }

    public async Task InitializeAsync()
    {
        if (!isInitialized)
        {
            if (!await TableExistsAsync("Items"))
            {
                await CreateItemTableAsync();
            }
            isInitialized = true;
        }
    }

    public async Task RegisterAsync(string itemName, string itemContent, int itemType)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }

        // Check if an item with the same name already exists in the database
        var existingItem = await dbContext.Items.FirstOrDefaultAsync(i => i.Name == itemName);
        if (existingItem != null)
        {
            // Item with the same name exists, do not overwrite
            return;
        }

        // Perform validation based on itemType
        if (itemType != 1 && itemType != 2)
        {
            // Invalid item type
            throw new ArgumentException("Invalid item type. itemType should be 1 (JSON) or 2 (XML).");
        }

        // Additional validation for JSON
        if (itemType == 1)
        {
            try
            {
                // Attempt to parse the JSON to ensure it's valid
                Newtonsoft.Json.JsonConvert.DeserializeObject(itemContent);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid JSON: {ex.Message}");
            }
        }
        // Additional validation for XML (itemType 2)
        if (itemType == 2)
        {
            try
            {
                // Attempt to parse the XML to ensure it's valid
                var xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.LoadXml(itemContent);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid XML: {ex.Message}");
            }
        }

        // Insert the item into the database
        var newItem = new Item
        {
            Name = itemName,
            Content = itemContent,
            Type = itemType
        };

        dbContext.Items.Add(newItem);
        await dbContext.SaveChangesAsync();
    }

    public async Task<string> RetrieveAsync(string itemName)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }

        var item = await dbContext.Items.FirstOrDefaultAsync(i => i.Name == itemName);
        return item?.Content;
    }

    public async Task<int> GetTypeAsync(string itemName)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }

        var item = await dbContext.Items.FirstOrDefaultAsync(i => i.Name == itemName);
        return item?.Type ?? -1; // Return -1 if item not found.
    }

    public async Task DeregisterAsync(string itemName)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }

        var item = await dbContext.Items.FirstOrDefaultAsync(i => i.Name == itemName);
        if (item != null)
        {
            dbContext.Items.Remove(item);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        using (var command = dbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
            command.Parameters.Add(
                new SqliteParameter("@tableName", DbType.String) { Value = tableName }
            );
            await dbContext.Database.OpenConnectionAsync();

            using (var result = await command.ExecuteReaderAsync())
            {
                return result.HasRows;
            }
        }
    }

    private async Task CreateItemTableAsync()
    {
        using (var command = dbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText =
                @"
            CREATE TABLE Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Content TEXT,
                Type INTEGER NOT NULL
            )";
            await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
