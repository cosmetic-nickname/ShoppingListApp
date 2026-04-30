// MainPage.xaml.cs
using System.Collections.ObjectModel;
using System.Linq;

namespace ShoppingListApp;

public partial class MainPage : ContentPage
{
    private readonly ShoppingManager _manager = new();
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "shopping.json");

    public ObservableCollection<ShoppingItem> Items { get; set; } = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadItemsAsync();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Ошибка", "Название покупки не может быть пустым", "OK");
            return;
        }

        if (!int.TryParse(QuantityEntry.Text, out int quantity) || quantity <= 0)
        {
            await DisplayAlert("Ошибка", "Введите корректное количество", "OK");
            return;
        }

        if (CategoryPicker.SelectedItem == null)
        {
            await DisplayAlert("Ошибка", "Выберите категорию", "OK");
            return;
        }

        int nextId = _manager.Items.Count == 0 ? 1 : _manager.Items.Max(i => i.Id) + 1;

        _manager.AddItem(
            nextId,
            TitleEntry.Text,
            quantity,
            CategoryPicker.SelectedItem.ToString()!,
            PurchasedCheckBox.IsChecked);

        RefreshItems();

        TitleEntry.Text = string.Empty;
        QuantityEntry.Text = string.Empty;
        CategoryPicker.SelectedIndex = -1;
        PurchasedCheckBox.IsChecked = false;

        await SaveItemsAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await SaveItemsAsync();
        await DisplayAlert("Готово", "Данные сохранены", "OK");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingItem item)
        {
            bool confirm = await DisplayAlert("Удаление", "Удалить покупку?", "Да", "Нет");

            if (confirm)
            {
                _manager.Items.Remove(item);
                RefreshItems();
                await SaveItemsAsync();
            }
        }
    }

    private async void OnPurchasedCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is ShoppingItem item)
        {
            item.IsPurchased = e.Value;
            await SaveItemsAsync();
        }
    }

    private async Task SaveItemsAsync()
    {
        try
        {
            await _manager.SaveItemsToFile(_filePath);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось сохранить данные: {ex.Message}", "OK");
        }
    }

    private async void LoadItemsAsync()
    {
        try
        {
            await _manager.LoadItemsFromFile(_filePath);
            RefreshItems();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
        }
    }

    private void RefreshItems()
    {
        Items.Clear();

        foreach (var item in _manager.Items)
        {
            Items.Add(item);
        }
    }
}