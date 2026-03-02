using GnuConvert.Services.Conversion;
using GnuConvert.ViewModels.State;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static GnuConvert.ViewModels.ConvertViewModel;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

 

    public ProgressState Progress { get; } = new();
    CancellationTokenSource? _cts;
    public void Cancel() => _cts?.Cancel();

    public async Task RunAsync(Func<IProgress<ProgressState.ProgressInfo>, CancellationToken, Task> work)
    {
        if (Progress.IsBusy) return;

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        Progress.IsBusy = true;
        Progress.Reset();

        var progress = new Progress<ProgressState.ProgressInfo>(info =>
        {
            if (info.Fraction is null)
            {
                Progress.IsIndeterminate = true;
            }
            else
            {
                Progress.IsIndeterminate = false;
                var f = info.Fraction.Value;
                if (f < 0) f = 0;
                if (f > 1) f = 1;
                Progress.Value = f * 100.0;
            }

            if (!string.IsNullOrWhiteSpace(info.Message))
                Progress.Message = info.Message!;
        });

        try { await work(progress, ct); }
        finally
        {
            Progress.IsBusy = false;
            _cts.Dispose();
            _cts = null;
        }
    }
}
