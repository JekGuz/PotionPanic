using System.Collections.ObjectModel;
using System.Windows.Input;
using PotionPanic.Models;
using PotionPanic.Services;

namespace PotionPanic.ViewModels;

public class ResultsViewModel
{
    private readonly IResultsRepository _repo;

    public ObservableCollection<GameResult> Items { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand BackToMenuCommand { get; }

    public ResultsViewModel()
    {
        _repo = ServiceHelper.Get<IResultsRepository>();

        RefreshCommand = new Command(async () => await LoadAsync());
        ClearCommand = new Command(async () => await ClearAllAsync());
        BackToMenuCommand = new Command(() => Shell.Current.GoToAsync("//menu"));

        // первичная загрузка
        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        var list = await _repo.GetAllAsync(); // уже отсортировано в репозитории
        Items.Clear();
        foreach (var r in list)
            Items.Add(r);
    }

    public async Task ClearAllAsync()
    {
        await _repo.DeleteAllAsync();
        Items.Clear();
    }
}
