using System.ComponentModel;

namespace CraftingCalculator.ViewModel
{
    /// <summary>
    /// Abstract implementation of property changed notification
    /// to reduce duplication.
    /// </summary>
    public abstract class AbstractPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
