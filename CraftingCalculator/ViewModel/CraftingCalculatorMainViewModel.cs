using CraftingCalculator.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CraftingCalculator.ViewModel
{
    public class CraftingCalculatorMainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandRunner TopMostCommand { get; set; }

        private bool _isTopMost;
        public bool IsTopMost
        {
            get => _isTopMost;
            set
            {
                _isTopMost = value;
                RaisePropertyChanged("IsTopMost");
            }
        }

        public CraftingCalculatorMainViewModel()
        {
            TopMostCommand = new CommandRunner(ToggleTopMost);
        }

        private void ToggleTopMost()
        {
            IsTopMost = !IsTopMost;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to quit?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }

            Properties.Settings.Default.Save();
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
