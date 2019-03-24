using CraftingCalculator.Utilities;
using MahApps.Metro;
using System;
using System.ComponentModel;
using System.Windows;

namespace CraftingCalculator.ViewModel
{
    public class CraftingCalculatorMainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandRunner TopMostCommand { get; set; }
        public CommandRunner ChangeThemeCommand { get; set; }
        public CommandRunner ChangeAccentCommand { get; set; }
        public CommandRunner ResetSettingsCommand { get; private set; }

        private bool _isTopMost;
        public bool IsTopMost
        {
            get => _isTopMost;
            set
            {
                _isTopMost = value;
                RaisePropertyChanged(nameof(IsTopMost));
            }
        }

        public CraftingCalculatorMainViewModel()
        {
            TopMostCommand = new CommandRunner(ToggleTopMost);
            ChangeThemeCommand = new CommandRunner(ChangeTheme);
            ChangeAccentCommand = new CommandRunner(ChangeAccent);
            ResetSettingsCommand = new CommandRunner(ResetSettings);

            ChangeTheme(Properties.Settings.Default["Theme"]);
        }

        /// <summary>
        /// Resets Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ResetSettings(object obj)
        {
            Properties.Settings.Default.Reset();
        }

        /// <summary>
        /// Changes application theme while preserving current accent, updates theme in Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeTheme(object obj)
        {
            string accent = Properties.Settings.Default["Accent"].ToString();

            ThemeManager.ChangeAppStyle(Application.Current,
                       ThemeManager.GetAccent(accent),
                       ThemeManager.GetAppTheme($"Base{obj}"));

            Properties.Settings.Default["Theme"] = obj;
        }

        /// <summary>
        /// Changes application accent while preserving current theme, updates accent in Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeAccent(object obj)
        {
            string theme = $"Base{Properties.Settings.Default["Theme"]}";

            ThemeManager.ChangeAppStyle(Application.Current,
                       ThemeManager.GetAccent(obj as string),
                       ThemeManager.GetAppTheme(theme));

            Properties.Settings.Default["Accent"] = obj;
        }

        /// <summary>
        /// Inverts IsTopMost bool
        /// </summary>
        /// <param name="obj"></param>
        private void ToggleTopMost(object obj)
        {
            IsTopMost = !IsTopMost;
        }

        /// <summary>
        /// Is called when window is closed, provides confirmation dialog and saves Properties.Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            //var result = MessageBox.Show("Are you sure you want to quit?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //if (result == MessageBoxResult.No)
            //{
            //    e.Cancel = true;
            //}

            Properties.Settings.Default.Save();
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
