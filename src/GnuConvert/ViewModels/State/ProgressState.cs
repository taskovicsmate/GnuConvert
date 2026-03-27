using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GnuConvert.ViewModels.State
{
    public sealed class ProgressState : INotifyPropertyChanged
    {
    public record ProgressInfo(
         double? Fraction,        // 0..1, null = indeterminate
        string? Message = null,
        int? Current = null,
         int? Total = null
);
        public event PropertyChangedEventHandler? PropertyChanged;
        void On([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        bool _isBusy;
        bool _isIndeterminate = false;
        double _value;
        string _message = "";

        public bool IsBusy { get => _isBusy; set { _isBusy = value; On(); } }
        public bool IsIndeterminate { get => _isIndeterminate; set { _isIndeterminate = value; On(); } }
        public double Value { get => _value; set { _value = value; On(); } }          // 0..100
        public string Message { get => _message; set { _message = value; On(); } }

        public void Reset()
        {
            IsIndeterminate = true;
            Value = 0;
            Message = "";
        }
    }
}
