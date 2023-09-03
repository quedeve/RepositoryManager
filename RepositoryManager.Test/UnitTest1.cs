using System;
using System.Threading.Tasks;
using Xunit;

public class RepositoryManagerTests
{
    private IRepository repository;

    public RepositoryManagerTests()
    {
        try
        {
            // Initialize the IRepositoryManager instance here.
            repository = new Repository();
            repository.InitializeAsync().GetAwaiter().GetResult(); // Ensure the repository is initialized before each test.
        }
        catch (Exception ex)
        {
            Assert.True(false, $"Failed to initialize repository: {ex.Message}");
        }
    }

    [Fact]
    public async Task Register_ValidJSONItem_Success()
    {
        // Arrange
        string itemName = "TestItem";
        string jsonContent = "{\"key\": \"value\"}";
        int itemType = 1;

        // Act
        await repository.RegisterAsync(itemName, jsonContent, itemType);

        // Assert
        var registeredItem = await repository.RetrieveAsync(itemName);
        Assert.NotNull(registeredItem);
        Assert.Equal(jsonContent, registeredItem);
    }

    [Fact]
    public async Task Register_ItemWithSameNameExists_NoOverwrite()
    {
        // Arrange
        string itemName = "ExistingItem";
        string jsonContent = "{\"key\": \"value\"}";
        int itemType = 1;

        // Act
        await repository.RegisterAsync(itemName, jsonContent, itemType);

        // Assert
        var registeredItem = await repository.RetrieveAsync(itemName);
        Assert.NotNull(registeredItem);
        Assert.Equal(jsonContent, registeredItem);

        // Act again (trying to register the same item)
        await repository.RegisterAsync(itemName, "{\"newKey\": \"newValue\"}", itemType);

        // Assert that the content was not overwritten
        var updatedItem = await repository.RetrieveAsync(itemName);
        Assert.Equal(jsonContent, updatedItem);
    }

    [Fact]
    public async Task Register_InvalidItemType_ThrowsArgumentException()
    {
        // Act and Assert
        await repository.DeregisterAsync("TestItem");
        Assert.Throws<ArgumentException>(() =>
        {
            repository.RegisterAsync("TestItem", "{}", 3).GetAwaiter().GetResult(); // Invalid itemType
        });
    }

    [Fact]
    public async Task Retrieve_ExistingItem_ReturnsItemContent()
    {
        // Arrange
        string itemName = "TestItem";
        string jsonContent = "{\"key\": \"value\"}";
        int itemType = 1;
        await repository.DeregisterAsync(itemName);
        await repository.RegisterAsync(itemName, jsonContent, itemType);

        // Act
        var result = await repository.RetrieveAsync(itemName);

        // Assert
        Assert.Equal(jsonContent, result);
    }

    [Fact]
    public async Task Retrieve_NonExistentItem_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await repository.RetrieveAsync("NonExistentItem");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetType_ExistingItem_ReturnsItemType()
    {
        // Arrange
        string itemName = "TestItem";
        int itemType = 1;

        await repository.RegisterAsync(itemName, "{}", itemType);

        // Act
        var result = await repository.GetTypeAsync(itemName);

        // Assert
        Assert.Equal(itemType, result);
    }

    [Fact]
    public async Task GetType_NonExistentItem_ReturnsMinusOne()
    {
        // Arrange

        // Act
        var result = await repository.GetTypeAsync("NonExistentItem");

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task Deregister_ExistingItem_Success()
    {
        // Arrange
        string itemName = "TestItem";

        await repository.RegisterAsync(itemName, "{}", 1);

        // Act
        await repository.DeregisterAsync(itemName);

        // Assert
        // Verify that the item was removed
        var result = await repository.RetrieveAsync(itemName);
        Assert.Null(result);
    }

    [Fact]
    public async Task Deregister_NonExistentItem_NoAction()
    {
        // Arrange
        string nonExistentItemName = "NonExistentItem";

        // Act
        await repository.DeregisterAsync(nonExistentItemName);

        // Assert
        // Verify that no action was taken and the state remains unchanged
        var result = await repository.RetrieveAsync(nonExistentItemName);
        Assert.Null(result);
    }

    [Fact]
    public async Task Initialize_Always_ExecutesWithoutException()
    {
        // Act and Assert
        try
        {
            await repository.InitializeAsync();
        }
        catch (Exception ex)
        {
            Assert.True(false, $"Initialize method threw an exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task Register_ValidXMLItem_Success()
    {
        // Arrange
        string itemName = "TestItemXML";
        string xmlContent = "<root><element>value</element></root>";
        int itemType = 2;

        // Act
        await repository.RegisterAsync(itemName, xmlContent, itemType);

        // Assert
        var registeredItem = await repository.RetrieveAsync(itemName);
        Assert.NotNull(registeredItem);
        Assert.Equal(xmlContent, registeredItem);
    }

    [Fact]
    public async Task Register_ItemWithSameNameExists_NoOverwrite_XML()
    {
        // Arrange
        string itemName = "ExistingItemXML";
        string xmlContent = "<root><element>value</element></root>";
        int itemType = 2;

        // Act
        await repository.RegisterAsync(itemName, xmlContent, itemType);

        // Assert
        var registeredItem = await repository.RetrieveAsync(itemName);
        Assert.NotNull(registeredItem);
        Assert.Equal(xmlContent, registeredItem);

        // Act again (trying to register the same item)
        await repository.RegisterAsync(itemName, "<newRoot></newRoot>", itemType);

        // Assert that the content was not overwritten
        var updatedItem = await repository.RetrieveAsync(itemName);
        Assert.Equal(xmlContent, updatedItem);
    }

    [Fact]
    public async Task Register_InvalidXMLItem_ThrowsArgumentException()
    {
        // Act and Assert
        await repository.DeregisterAsync("TestItemXML");
        Assert.Throws<ArgumentException>(() =>
        {
            repository.RegisterAsync("TestItemXML", "invalid XML", 2).GetAwaiter().GetResult(); // Invalid XML
        });
    }

    [Fact]
    public async Task Retrieve_ExistingXMLItem_ReturnsItemContent()
    {
        // Arrange
        string itemName = "TestItemXML";
        string xmlContent = "<root><element>value</element></root>";
        int itemType = 2;
        await repository.DeregisterAsync(itemName);
        await repository.RegisterAsync(itemName, xmlContent, itemType);

        // Act
        var result = await repository.RetrieveAsync(itemName);

        // Assert
        Assert.Equal(xmlContent, result);
    }

    [Fact]
    public async Task GetType_ExistingXMLItem_ReturnsItemType()
    {
        // Arrange
        string itemName = "TestItemXML";
        int itemType = 2;

        await repository.RegisterAsync(itemName, "<root></root>", itemType);

        // Act
        var result = await repository.GetTypeAsync(itemName);

        // Assert
        Assert.Equal(itemType, result);
    }

    [Fact]
    public async Task Deregister_ExistingXMLItem_Success()
    {
        // Arrange
        string itemName = "TestItemXML";

        await repository.RegisterAsync(itemName, "<root></root>", 2);

        // Act
        await repository.DeregisterAsync(itemName);

        // Assert
        // Verify that the item was removed
        var result = await repository.RetrieveAsync(itemName);
        Assert.Null(result);
    }
}
