using System.Collections.ObjectModel;
using BikeShare.Desktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BikeShare.Desktop.ViewModels;

public partial class BikeHistoryViewModel : ObservableObject
{
    public Bike Bike { get; set; }
    
    [ObservableProperty]
    private ObservableCollection<StatusHistory> _history = [];

    public BikeHistoryViewModel(Bike bike)
    {
        Bike = bike;
        _ = LoadHistory();
    }

    private async Task LoadHistory()
    {
        var history = await ApiService.GetAsync<List<StatusHistory>>($"bikes/{Bike.Id}/status");
        Console.WriteLine(history);
        if (history != null)
        {
            History = new ObservableCollection<StatusHistory>(history);
        }
    }
}