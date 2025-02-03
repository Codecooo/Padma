using System.Linq;
using ReactiveUI;
using Padma.Models;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;

namespace Padma.ViewModels;

public class HomeViewModel : ReactiveObject
{
    private readonly SaveHistory _history;
    private string _downloadStatus;
    private ObservableCollection<LiteDbHistory> _historyList = new();

    public HomeViewModel(SaveHistory history)
    {
        _history = history;
        _history.HistoryChangedSignal
            .Subscribe (_  => LoadRecentHistory());
        this.WhenAnyValue(h => h._history.DownloadStatusChange)
            .Subscribe(_ => AutoClearDownloadBar());
    }
    
    public string DownloadStatusNow
    {
        get => _downloadStatus;
        set => this.RaiseAndSetIfChanged(ref _downloadStatus, value);
    }
    
    private void LoadRecentHistory()
    {
        var recenthistory = _history.GetRecentHistoryList().ToList();
        HistoryList = new ObservableCollection<LiteDbHistory>(recenthistory);
    }
    
    public ObservableCollection<LiteDbHistory> HistoryList
    {
        get => _historyList;
        set => this.RaiseAndSetIfChanged(ref _historyList, value);
    }

    public void AutoClearDownloadBar()
    {
        DownloadStatusNow = _history.DownloadStatusChange;
        if (_history.DownloadStatusChange == "Finished")
            Task.Delay(TimeSpan.FromMinutes(1.6)).ContinueWith(_ => HistoryList.Clear());
    }
}