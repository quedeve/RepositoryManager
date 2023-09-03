using System.Threading.Tasks;

public interface IRepository
{
    // Store an item to the repository asynchronously.
    // Parameter item Type is used to differentiate JSON or XML.
    // 1 = itemContent is a JSON string.
    // 2 = itemContent is an XML string.
    Task RegisterAsync(string itemName, string itemContent, int itemType);

    // Retrieve an item from the repository asynchronously.
    Task<string> RetrieveAsync(string itemName);

    // Retrieve the type of the item (JSON or XML) asynchronously.
    Task<int> GetTypeAsync(string itemName);

    // Remove an item from the repository asynchronously.
    Task DeregisterAsync(string itemName);

    // Initialize the repository for use asynchronously, if needed.
    // You could leave it empty if you have your own way to make the repository ready for use
    // (e.g., using the constructor).
    Task InitializeAsync();
}
