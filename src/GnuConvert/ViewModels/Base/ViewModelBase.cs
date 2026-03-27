using GnuConvert.ExceptionHandling;
using GnuConvert.ViewModels.State;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ViewModelBase : INotifyPropertyChanged
{
    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

 

    public ProgressState Progress { get; } = new();
    CancellationTokenSource? _cts;
    public void Cancel() => _cts?.Cancel();
    public void HandleAppException(AppException ex)
    {
        Log(ex);

        switch (ex)
        {
            case DomainException:
                ErrorMessage = ex.Message;
                break;

            case PersistenceException:
                ErrorMessage = "File operation failed. Please check permissions or file integrity.";
                break;

            case ConfigurationException:
                ErrorMessage = "Application configuration is invalid.";
                break;

            case ConversionException:
                ErrorMessage = "Invoice conversion failed.";
                break;

            default:
                ErrorMessage = "Unexpected error occurred.";
                break;
        }
    }
    public  void HandleUnknownException(Exception ex)
    {
        Log(ex);
        ErrorMessage = "An unexpected error occurred. Please try again.";
    }
    private void Log(Exception ex)
    {
        Console.Error.WriteLine(ex.Message.ToString());
    }
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
        catch (AppException ex)
        {
            HandleAppException(ex);
        }
        catch (Exception ex)
        {
            HandleUnknownException(ex);
        }
        finally
        {
            Progress.IsBusy = false;
            _cts.Dispose();
            _cts = null;
        }
    }
}
