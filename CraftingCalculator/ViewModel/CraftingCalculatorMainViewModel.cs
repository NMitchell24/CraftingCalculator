﻿using CraftingCalculator.Utilities;
using MahApps.Metro;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.Reflection;
using System.Text;
using System;

namespace CraftingCalculator.ViewModel
{
    public class CraftingCalculatorMainViewModel : INotifyPropertyChanged
    {
        private IDialogCoordinator dialogCoordinator;
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandRunner TopMostCommand { get; set; }
        public CommandRunner ChangeThemeCommand { get; set; }
        public CommandRunner ChangeAccentCommand { get; set; }
        public CommandRunner ResetSettingsCommand { get; private set; }
        public CommandRunner AboutCommand { get; set; }

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
            
            dialogCoordinator = instance;

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

        /// <summary>
        /// Shows dialogue with Application Information.
        /// </summary>
        /// <param name="obj"></param>
        private async void ShowAboutDialog(object obj)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
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

            //Show the dialog
            await dialogCoordinator.ShowMessageAsync(this, "About Crafting Calculator", sb.ToString());
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
