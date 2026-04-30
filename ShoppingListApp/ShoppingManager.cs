// ShoppingManager.cs
using System.Text.Json;

namespace ShoppingListApp
{
    public class ShoppingManager
    {
        public List<ShoppingItem> Items { get; set; } = new List<ShoppingItem>();

        public void AddItem(int id, string title, int quantity, string category, bool isPurchased)
        {
            if (string.IsNullOrWhiteSpace(title))
                return;

            if (quantity <= 0)
                return;

            if (string.IsNullOrWhiteSpace(category))
                return;

            Items.Add(new ShoppingItem
            {
                Id = id,
                Title = title.Trim(),
                Quantity = quantity,
                Category = category.Trim(),
                IsPurchased = isPurchased
            });
        }

        public List<ShoppingItem> FilterByCategory(string category)
        {
            return Items
                .Where(item => string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task SaveItemsToFile(string filePath)
        {
            string json = JsonSerializer.Serialize(Items, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task LoadItemsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Items = new List<ShoppingItem>();
                return;
            }

            string json = await File.ReadAllTextAsync(filePath);

            var loadedItems = JsonSerializer.Deserialize<List<ShoppingItem>>(json) ?? new List<ShoppingItem>();
            Items = loadedItems;
        }
    }
}