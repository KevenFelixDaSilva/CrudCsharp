public record ProductRequest(
    string Code, string name, string Description, int CategoryId, List<string> Tags
    );