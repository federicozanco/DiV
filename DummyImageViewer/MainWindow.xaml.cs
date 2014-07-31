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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;

namespace DummyImageViewer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            MainWindowViewModel model = DataContext as MainWindowViewModel;

            model.Skip = (int)Properties.Settings.Default["Skip"];
            model.ImageWidth = (double)Properties.Settings.Default["ImageWidth"];
            model.ThumbImageWidth = (double)Properties.Settings.Default["ThumbImageWidth"];
            
            // Make sure we handle command line args:
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
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
                        (this.DataContext as MainWindowViewModel).ImageSource = new Uri(System.IO.Path.GetFullPath(filename));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            MainWindowViewModel model = DataContext as MainWindowViewModel;

            Properties.Settings.Default["Skip"] = model.Skip;
            Properties.Settings.Default["ImageWidth"] = model.ImageWidth;
            Properties.Settings.Default["ThumbImageWidth"] = model.ThumbImageWidth;

            Properties.Settings.Default.Save();
        }
    }
}
