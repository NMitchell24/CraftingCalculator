using CraftingCalculator.Utilities;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.Reflection;
using System.Text;
using System;
using ControlzEx.Theming;

namespace CraftingCalculator.ViewModel
{
    public class CraftingCalculatorMainViewModel : AbstractPropertyChanged
    {
        private IDialogCoordinator dialogCoordinator;
        public string? ConfirmOnCloseHeader { get; set; }
        public CommandRunner TopMostCommand { get; set; }
        public CommandRunner ChangeThemeCommand { get; set; }
        public CommandRunner ChangeAccentCommand { get; set; }
        public CommandRunner ResetSettingsCommand { get; private set; }
        public CommandRunner AboutCommand { get; set; }
        public CommandRunner ExitCommand { get; set; }
        public CommandRunner OpenRecipeConfiguratorCommand { get; set; }
        public CommandRunner OpenRecipesViewCommand { get; set; }
        public CommandRunner EnableDisableConfirmOnCloseCommand { get; set; }


        private bool _doClose;
        private int _currentView;

        public int CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value; this.RaisePropertyChanged("CurrentView");
            }
        }

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

        public CraftingCalculatorMainViewModel(IDialogCoordinator instance)
        {
            TopMostCommand = new CommandRunner(ToggleTopMost);
            ChangeThemeCommand = new CommandRunner(ChangeTheme);
            ChangeAccentCommand = new CommandRunner(ChangeAccent);
            ResetSettingsCommand = new CommandRunner(ResetSettings);
            AboutCommand = new CommandRunner(ShowAboutDialog);
            ExitCommand = new CommandRunner(Close);
            OpenRecipeConfiguratorCommand = new CommandRunner(OpenRecipeConfigurator);
            OpenRecipesViewCommand = new CommandRunner(OpenRecipesView);
            EnableDisableConfirmOnCloseCommand = new CommandRunner(EnableDisableConfirmOnClose);
            CurrentView = 0;
            
            dialogCoordinator = instance;

            ChangeTheme(Properties.Settings.Default["Theme"]);
            ChangeAccent(Properties.Settings.Default["Accent"]);
            UpdateConfirmOnCloseHeader();

        }

        /// <summary>
        /// Dynamically sets the Menu Item text based upon current Settings for ConfirmOnClose
        /// </summary>
        private void UpdateConfirmOnCloseHeader()
        {
            bool confirmOnClose = (bool)Properties.Settings.Default["ConfirmOnClose"];
            if(confirmOnClose)
            {
                ConfirmOnCloseHeader = "Disable Confirmation on Close (F3)";
            }
            else
            {
                ConfirmOnCloseHeader = "Enable Confirmation on Close (F3)";
            }
            RaisePropertyChanged(nameof(ConfirmOnCloseHeader));
        }

        /// <summary>
        /// Enables or disables the Confirmation on Closing of the application.
        /// </summary>
        /// <param name="obj"></param>
        private void EnableDisableConfirmOnClose(object obj)
        {
            bool confirmOnClose = (bool)Properties.Settings.Default["ConfirmOnClose"];
            Properties.Settings.Default["ConfirmOnClose"] = !confirmOnClose;
            UpdateConfirmOnCloseHeader();
        }

        /// <summary>
        /// Resets Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ResetSettings(object obj)
        {
            Properties.Settings.Default.Reset();
            ChangeTheme(Properties.Settings.Default["Theme"]);
            ChangeAccent(Properties.Settings.Default["Accent"]);
            UpdateConfirmOnCloseHeader();
        }

        /// <summary>
        /// Changes application theme while preserving current accent, updates theme in Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeTheme(object obj)
        {
            if (obj is string s)
            {
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, s);

                Properties.Settings.Default["Theme"] = obj;
            }
        }

        /// <summary>
        /// Changes application accent while preserving current theme, updates accent in Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeAccent(object obj)
        {
            if (obj is string s)
            {
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, s);

                Properties.Settings.Default["Accent"] = obj;
            }
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
        public async void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            bool confirmOnClose = (bool)Properties.Settings.Default["ConfirmOnClose"];

            if(!confirmOnClose)
            {
                Application.Current.Shutdown();
            }

            e.Cancel = !_doClose;
            if (_doClose) return;

            var _settings = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" };
            var _result = await dialogCoordinator.ShowMessageAsync(this, "Confirm", 
                "Are you sure you want to exit?", 
                MessageDialogStyle.AffirmativeAndNegative, 
                _settings);

            _doClose = _result == MessageDialogResult.Affirmative;

            if (_doClose)
            {
                Application.Current.Shutdown();
            }
        }

        private void Close(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Shows dialogue with Application Information.
        /// </summary>
        /// <param name="obj"></param>
        private async void ShowAboutDialog(object obj)
        {
            string? version = Assembly.GetExecutingAssembly().GetName()?.Version?.ToString();
            DateTime now = DateTime.Now;
            StringBuilder sb = new StringBuilder();

            //Build the text for the dialog message
            sb.AppendLine("Crafting Calculator");
            sb.AppendLine("Copyright © " + now.Year + " Nathan Mitchell");
            sb.AppendLine("Version: " + version);
            sb.Append(Environment.NewLine);
            sb.AppendLine("Licensed under GPL 2.0");
            sb.Append(Environment.NewLine);
            sb.AppendLine("This program is free software; you can redistribute it and/or modify " +
                "it under the terms of the GNU General Public License as published by " +
                "the Free Software Foundation; either version 2 of the License, or " +
                "any later version.");
            sb.Append(Environment.NewLine);
            sb.AppendLine("This program is distributed in the hope that it will be useful, " +
                "but WITHOUT ANY WARRANTY; without even the implied warranty of " +
                "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the " +
                "GNU General Public License for more details.");
            sb.Append(Environment.NewLine);
            sb.AppendLine("Crafting Calculator was brought to you by the color clear, " +
                "the number 0 and was made possible with contributions from BlendedAptness");

            //Show the dialog
            await dialogCoordinator.ShowMessageAsync(this, "About", sb.ToString());
        }

        private void OpenRecipeConfigurator(object obj)
        {
            CurrentView = 1;
        }

        private void OpenRecipesView(object obj)
        {
            CurrentView = 0;
        }
    }
}
