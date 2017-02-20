/*
 * Dummy Image Viewer (DiV) - I need it to scan ladle tracking snapshots... :D
 * Copyright (C) 2014  Federico Zanco [federico.zanco (at) gmail.com]
 * 
 * This file is part of Dummy Image Viewer.
 * 
 * Dummy Image Viewer is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Dummy Image Viewer; if not, If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DummyImageViewer.Properties;

namespace DummyImageViewer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var model = DataContext as MainWindowViewModel;

            if (model != null)
            {
                model.ImageWidth = (double)Settings.Default["ImageWidth"];
                model.Skip = (int)Settings.Default["Skip"];
                model.ThumbImageWidth = (double)Settings.Default["ThumbImageWidth"];
            }

            // odd workaround... see: http://stackoverflow.com/questions/911904/commandbinding-in-window-doesnt-catch-execution-of-command-from-contextmenu
            Focus();

            // Make sure we handle command line args:
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Application.Current.Properties["Arg0"] != null)
                {
                    string filename = Application.Current.Properties["Arg0"].ToString();

                    // Act on the file...
                    if (File.Exists(filename))
                    {
                        var model = DataContext as MainWindowViewModel;

                        if (model != null)
                            model.ImageSource = new Uri(Path.GetFullPath(filename));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            var model = DataContext as MainWindowViewModel;
            if (model == null)
                return;

            Settings.Default["Skip"] = model.Skip;
            Settings.Default["ImageWidth"] = model.ImageWidth;
            Settings.Default["ThumbImageWidth"] = model.ThumbImageWidth;

            Settings.Default.Save();
        }
    }
}
