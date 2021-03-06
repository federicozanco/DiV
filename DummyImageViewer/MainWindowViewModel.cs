﻿/*
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;

namespace DummyImageViewer
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region ImageSourceEnum
        private enum ImageSourceEnum
        {
            None = -1,
            Backward1ImageSource = 0,
            Backward2ImageSource = 1,
            Backward3ImageSource = 2,
            Backward4ImageSource = 3,
            Forward1ImageSource = 4,
            Forward2ImageSource = 5,
            Forward3ImageSource = 6,
            Forward4ImageSource = 7,
            ImageSource = 8,
            Other = 9,
        }
        #endregion

        #region Constants
        private const string DefaultWindowTitle = "DiV";

        private const double DefaultImageWidth = 640.0;
        private const double DefaultImageHeight = 360.0;
        private const double DefaultThumbImageWidth = 160.0;
        private const double DefaultThumbImageHeight = 80.0;
        private const double MinImageWidth = 20.0;
        private const double MinImageHeight = 16.0;
        private const double MaxImageWidth = 1280.0;
        private const double MaxImageHeight = 1024.0;

        private const int DefaultSkip = 32;

        /// <summary>
        /// The default image
        /// </summary>
        public const string DefaultImage = "pack://application:,,,/notfound.png";
        private const string OpenFileDialogFilter = "Image Files (JPG, PNG, GIF, BMP)|*.jpg;*.png;*.gif;*.bmp";

        private const double DefaultReloadTimerInterval = 100.0;
        #endregion

        #region Private
        // NOTE: skips are half value of the current shift (+=/-=)
        private readonly int[] _skips = { 8, 8, 16, 32 };
        private string _windowTitle = DefaultWindowTitle;

        private Uri _imageSource = new Uri(DefaultImage);
        private Uri _backward1ImageSource = new Uri(DefaultImage);
        private Uri _backward2ImageSource = new Uri(DefaultImage);
        private Uri _backward3ImageSource = new Uri(DefaultImage);
        private Uri _backward4ImageSource = new Uri(DefaultImage);
        private Uri _forward1ImageSource = new Uri(DefaultImage);
        private Uri _forward2ImageSource = new Uri(DefaultImage);
        private Uri _forward3ImageSource = new Uri(DefaultImage);
        private Uri _forward4ImageSource = new Uri(DefaultImage);

        private double _imageWidth = DefaultImageWidth;
        private double _imageHeight = DefaultImageHeight;
        private double _thumbImageWidth = DefaultThumbImageWidth;
        private double _thumbImageHeight = DefaultThumbImageHeight;
        private List<string> _imageFiles;
        private int _index = -1;
        private int _skip = DefaultSkip;

        private bool _areThumbsVisible = false;
        private bool _isHelpVisible;
        private bool _areSettingsVisible;

        // create a new FileSystemWatcher and set its properties.
        // Watch for changes in LastAccess and LastWrite times, and
        // the renaming of files or directories.
        private FileSystemWatcher _watcher = new FileSystemWatcher
        {
            NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName,
            Filter = "*.*"
        };

        private readonly DispatcherTimer _reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(DefaultReloadTimerInterval) };
        private ImageSourceEnum _changed = ImageSourceEnum.None;
        private string _renamed = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>
        /// The window title.
        /// </value>
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyPropertyChanged("WindowTitle");
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public Uri ImageSource
        {
            get { return _imageSource; }
            set
            {
                _watcher.EnableRaisingEvents = false;

                _imageSource = value;
                NotifyPropertyChanged("ImageSource");

                ReadImageFiles();

                WindowTitle = DefaultWindowTitle
                    + (_imageFiles != null && _index >= 0 && _index < _imageFiles.Count
                    ? " - " + Path.GetFileName(_imageFiles[_index]) + "   (" + (_index + 1) + " / " + _imageFiles.Count + ")"
                    : string.Empty);

                _watcher.Path = Directory.GetParent(_imageFiles[_index]).FullName;
                _watcher.EnableRaisingEvents = true;

                SetCommandsExecutionStatus();
            }
        }

        /// <summary>
        /// Gets or sets the backward1 image source.
        /// </summary>
        /// <value>
        /// The backward1 image source.
        /// </value>
        public Uri Backward1ImageSource
        {
            get { return _backward1ImageSource; }
            set
            {
                _backward1ImageSource = value;
                NotifyPropertyChanged("Backward1ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the backward2 image source.
        /// </summary>
        /// <value>
        /// The backward2 image source.
        /// </value>
        public Uri Backward2ImageSource
        {
            get { return _backward2ImageSource; }
            set
            {
                _backward2ImageSource = value;
                NotifyPropertyChanged("Backward2ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the backward3 image source.
        /// </summary>
        /// <value>
        /// The backward3 image source.
        /// </value>
        public Uri Backward3ImageSource
        {
            get { return _backward3ImageSource; }
            set
            {
                _backward3ImageSource = value;
                NotifyPropertyChanged("Backward3ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the bacwardk4 image source.
        /// </summary>
        /// <value>
        /// The backward4 image source.
        /// </value>
        public Uri Backward4ImageSource
        {
            get { return _backward4ImageSource; }
            set
            {
                _backward4ImageSource = value;
                NotifyPropertyChanged("Backward4ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the forward1 image source.
        /// </summary>
        /// <value>
        /// The forward1 image source.
        /// </value>
        public Uri Forward1ImageSource
        {
            get { return _forward1ImageSource; }
            set
            {
                _forward1ImageSource = value;
                NotifyPropertyChanged("Forward1ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the forward2 image source.
        /// </summary>
        /// <value>
        /// The forward2 image source.
        /// </value>
        public Uri Forward2ImageSource
        {
            get { return _forward2ImageSource; }
            set
            {
                _forward2ImageSource = value;
                NotifyPropertyChanged("Forward2ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the forward3 image source.
        /// </summary>
        /// <value>
        /// The forward3 image source.
        /// </value>
        public Uri Forward3ImageSource
        {
            get { return _forward3ImageSource; }
            set
            {
                _forward3ImageSource = value;
                NotifyPropertyChanged("Forward3ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the forward4 image source.
        /// </summary>
        /// <value>
        /// The forward4 image source.
        /// </value>
        public Uri Forward4ImageSource
        {
            get { return _forward4ImageSource; }
            set
            {
                _forward4ImageSource = value;
                NotifyPropertyChanged("Forward4ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                var ratio = _imageHeight / _imageWidth;

                _imageWidth = value;
                NotifyPropertyChanged("ImageWidth");

                ImageHeight = _imageWidth * ratio;

                SetImageSources();
            }
        }

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                NotifyPropertyChanged("ImageHeight");
            }
        }

        /// <summary>
        /// Gets or sets the width of the thumb image.
        /// </summary>
        /// <value>
        /// The width of the thumb image.
        /// </value>
        public double ThumbImageWidth
        {
            get { return _thumbImageWidth; }
            set
            {
                var ratio = _thumbImageHeight / _thumbImageWidth;

                _thumbImageWidth = value;
                NotifyPropertyChanged("ThumbImageWidth");

                ThumbImageHeight = _thumbImageWidth * ratio;

                SetImageSources();
            }
        }

        /// <summary>
        /// Gets or sets the height of the thumb image.
        /// </summary>
        /// <value>
        /// The height of the thumb image.
        /// </value>
        public double ThumbImageHeight
        {
            get { return _thumbImageHeight; }
            set
            {
                _thumbImageHeight = value;
                NotifyPropertyChanged("ThumbImageHeight");
            }
        }

        /// <summary>
        /// Gets or sets the image width values.
        /// </summary>
        /// <value>
        /// The image width values.
        /// </value>
        public List<double> ImageWidthValues { get; set; }

        /// <summary>
        /// Gets or sets the skip.
        /// </summary>
        /// <value>
        /// The skip.
        /// </value>
        public int Skip
        {
            get { return _skip; }
            set
            {
                _skip = value;
                NotifyPropertyChanged("Skip");

                var s = _skip / 2;

                for (var i = 3; i > 0; i--)
                {
                    _skips[i] = s;
                    s /= 2;
                }

                _skips[0] = s * 2;
            }
        }

        /// <summary>
        /// Gets the skip values.
        /// </summary>
        /// <value>
        /// The skip values.
        /// </value>
        public List<int> SkipValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [are thumbs visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [are thumbs visible]; otherwise, <c>false</c>.
        /// </value>
        public bool AreThumbsVisible
        {
            get { return _areThumbsVisible; }
            set
            {
                _areThumbsVisible = value;
                NotifyPropertyChanged("AreThumbsVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is help visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is help visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsHelpVisible
        {
            get { return _isHelpVisible; }
            set
            {
                _isHelpVisible = value;
                NotifyPropertyChanged("IsHelpVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [are settings visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [are settings visible]; otherwise, <c>false</c>.
        /// </value>
        public bool AreSettingsVisible
        {
            get { return _areSettingsVisible; }
            set
            {
                _areSettingsVisible = value;
                NotifyPropertyChanged("AreSettingsVisible");
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Gets or sets up command.
        /// </summary>
        /// <value>
        /// Up command.
        /// </value>
        public SimpleDelegateCommand UpCommand { get; set; }

        /// <summary>
        /// Gets or sets down command.
        /// </summary>
        /// <value>
        /// Down command.
        /// </value>
        public SimpleDelegateCommand DownCommand { get; set; }

        /// <summary>
        /// Gets or sets the left command.
        /// </summary>
        /// <value>
        /// The left command.
        /// </value>
        public SimpleDelegateCommand LeftCommand { get; set; }

        /// <summary>
        /// Gets or sets the right command.
        /// </summary>
        /// <value>
        /// The right command.
        /// </value>
        public SimpleDelegateCommand RightCommand { get; set; }

        /// <summary>
        /// Gets or sets the home command.
        /// </summary>
        /// <value>
        /// The home command.
        /// </value>
        public SimpleDelegateCommand HomeCommand { get; set; }

        /// <summary>
        /// Gets or sets the end command.
        /// </summary>
        /// <value>
        /// The end command.
        /// </value>
        public SimpleDelegateCommand EndCommand { get; set; }

        /// <summary>
        /// Gets or sets the zoom command.
        /// </summary>
        /// <value>
        /// The zoom command.
        /// </value>
        public SimpleDelegateCommand ZoomCommand { get; set; }

        /// <summary>
        /// Gets or sets the toggle thumbs command.
        /// </summary>
        /// <value>
        /// The toggle thumbs command.
        /// </value>
        public SimpleDelegateCommand ToggleThumbsCommand { get; set; }

        /// <summary>
        /// Gets or sets the reload command.
        /// </summary>
        /// <value>
        /// The reload command.
        /// </value>
        public SimpleDelegateCommand ReloadCommand { get; set; }

        /// <summary>
        /// Gets or sets the help command.
        /// </summary>
        /// <value>
        /// The help command.
        /// </value>
        public SimpleDelegateCommand HelpCommand { get; set; }

        /// <summary>
        /// Gets or sets the options command.
        /// </summary>
        /// <value>
        /// The options command.
        /// </value>
        public SimpleDelegateCommand SettingsCommand { get; set; }

        /// <summary>
        /// Gets or sets the copy command.
        /// </summary>
        /// <value>
        /// The copy command.
        /// </value>
        public SimpleDelegateCommand CopyCommand { get; set; }

        /// <summary>
        /// Gets or sets the open file command.
        /// </summary>
        /// <value>
        /// The open file command.
        /// </value>
        public SimpleDelegateCommand OpenFileCommand { get; set; }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// Viene generato quando il valore di una proprietà cambia.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            UpCommand = new SimpleDelegateCommand(KeyCommandExecute, UpCommandCanExecute);
            DownCommand = new SimpleDelegateCommand(KeyCommandExecute, DownCommandCanExecute);
            LeftCommand = new SimpleDelegateCommand(KeyCommandExecute, LeftCommandCanExecute);
            RightCommand = new SimpleDelegateCommand(KeyCommandExecute, RightCommandCanExecute);
            HomeCommand = new SimpleDelegateCommand(KeyCommandExecute, HomeCommandCanExecute);
            EndCommand = new SimpleDelegateCommand(KeyCommandExecute, EndCommandCanExecute);
            ZoomCommand = new SimpleDelegateCommand(ZoomCommandExecute, ZoomCommandCanExecute);
            ToggleThumbsCommand = new SimpleDelegateCommand(ToggleThumbsCommandExecute, o => true);
            ReloadCommand = new SimpleDelegateCommand(ReloadCommandExecute, o => true);
            HelpCommand = new SimpleDelegateCommand(HelpCommandExecute, o => true);
            SettingsCommand = new SimpleDelegateCommand(SettingsCommandExecute, o => true);
            CopyCommand = new SimpleDelegateCommand(CopyCommandExecute, CopyCommandCanExecute);
            OpenFileCommand = new SimpleDelegateCommand(OpenFileCommandExecute, o => true);

            SkipValues = new List<int>();

            for (var i = 32; i <= 1024; i *= 2)
                SkipValues.Add(i);

            ImageWidthValues = new List<double>();

            var w = MinImageWidth;

            while (w <= MaxImageWidth)
            {
                ImageWidthValues.Add(w);
                w *= 2;
            }

            // add event handlers.
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnCreatedDeleted);
            _watcher.Deleted += new FileSystemEventHandler(OnCreatedDeleted);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);

            _reloadTimer.Tick += new EventHandler(ReloadTimer_Tick);
        }
        #endregion

        #region Private
        private void ReadImageFiles(bool force = false)
        {
            if (!force && _imageFiles != null && _imageFiles.Count > 0)
                return;

            var filename = Path.GetFullPath(Uri.UnescapeDataString(_imageSource.AbsolutePath));
            var files = Directory.GetFiles(Directory.GetParent(filename).FullName, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => OpenFileDialogFilter.Contains(Path.GetExtension(s).ToLower()));
            _imageFiles = new List<string>();

            foreach (var f in files)
            {
                _imageFiles.Add(f);

                if (Path.GetFullPath(f) == filename)
                    _index = _imageFiles.Count - 1;
            }

            SetImageSources();
        }

        private void SetImageSource(ImageSourceEnum source, Uri uri)
        {
            switch (source)
            {
                case ImageSourceEnum.Backward1ImageSource:
                    Backward1ImageSource = uri;
                    break;

                case ImageSourceEnum.Backward2ImageSource:
                    Backward2ImageSource = uri;
                    break;

                case ImageSourceEnum.Backward3ImageSource:
                    Backward3ImageSource = uri;
                    break;

                case ImageSourceEnum.Backward4ImageSource:
                    Backward4ImageSource = uri;
                    break;

                case ImageSourceEnum.Forward1ImageSource:
                    Forward1ImageSource = uri;
                    break;

                case ImageSourceEnum.Forward2ImageSource:
                    Forward2ImageSource = uri;
                    break;

                case ImageSourceEnum.Forward3ImageSource:
                    Forward3ImageSource = uri;
                    break;

                case ImageSourceEnum.Forward4ImageSource:
                    Forward4ImageSource = uri;
                    break;

                default:
                    ImageSource = uri;
                    break;
            }
        }

        private void SetImageSources()
        {
            if (_imageFiles == null || _imageFiles.Count == 0)
                return;

            while (_imageFiles.Count > 0 && _index < _imageFiles.Count)
            {
                try
                {
                    SetImageSource(ImageSourceEnum.ImageSource, new Uri(_imageFiles[_index]));
                    break;
                }
                catch (NotSupportedException)
                {
                    // System.NotSupportedException:
                    // No imaging component suitable to complete this operation was found.
                    _imageFiles.RemoveAt(_index);
                }
            }

            if (!_areThumbsVisible)
                return;

            var bIndex = _index;

            for (var b = 0; b < 4; b++)
            {
                bIndex -= _skips[b];

                while (bIndex >= 0 && _imageFiles.Count > 0 && bIndex < _imageFiles.Count)
                {
                    try
                    {
                        SetImageSource((ImageSourceEnum)b, new Uri(_imageFiles[bIndex]));
                        break;
                    }
                    catch (NotSupportedException)
                    {
                        // System.NotSupportedException:
                        // No imaging component suitable to complete this operation was found.
                        _imageFiles.RemoveAt(bIndex);
                        _index--;
                    }
                }

                if (bIndex < 0)
                    SetImageSource((ImageSourceEnum)b, new Uri(DefaultImage));
            }

            var fIndex = _index;

            for (var f = 0; f < 4; f++)
            {
                fIndex += _skips[f] * 3 / 4;

                while (_imageFiles.Count > 0 && fIndex < _imageFiles.Count)
                {
                    try
                    {
                        SetImageSource((ImageSourceEnum)(f + 4), new Uri(_imageFiles[fIndex]));
                        break;
                    }
                    catch (NotSupportedException)
                    {
                        // System.NotSupportedException:
                        // No imaging component suitable to complete this operation was found.
                        _imageFiles.RemoveAt(fIndex);
                    }
                }

                if (fIndex > _imageFiles.Count)
                    SetImageSource((ImageSourceEnum)(f + 4), new Uri(DefaultImage));
            }
        }

        #region FileSystemWatcher
        private void SetChanged(string fullPath)
        {
            // NOTE: the best way sould be to use new (Uri(e.FullPath)).AbsolutePath
            if (_imageSource != null && fullPath == _imageSource.LocalPath)
                _changed = ImageSourceEnum.ImageSource;
            else if (_backward1ImageSource != null && fullPath == _backward1ImageSource.LocalPath)
                _changed = ImageSourceEnum.Backward1ImageSource;
            else if (_backward2ImageSource != null && fullPath == _backward2ImageSource.LocalPath)
                _changed = ImageSourceEnum.Backward2ImageSource;
            else if (_backward3ImageSource != null && fullPath == _backward3ImageSource.LocalPath)
                _changed = ImageSourceEnum.Backward3ImageSource;
            else if (_backward4ImageSource != null && fullPath == _backward4ImageSource.LocalPath)
                _changed = ImageSourceEnum.Backward4ImageSource;
            else if (_forward1ImageSource != null && fullPath == _forward1ImageSource.LocalPath)
                _changed = ImageSourceEnum.Forward1ImageSource;
            else if (_forward2ImageSource != null && fullPath == _forward2ImageSource.LocalPath)
                _changed = ImageSourceEnum.Forward2ImageSource;
            else if (_forward3ImageSource != null && fullPath == _forward3ImageSource.LocalPath)
                _changed = ImageSourceEnum.Forward3ImageSource;
            else if (_forward4ImageSource != null && fullPath == _forward4ImageSource.LocalPath)
                _changed = ImageSourceEnum.Forward4ImageSource;
            else
                _changed = ImageSourceEnum.Other;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            SetChanged(e.FullPath);

            _reloadTimer.Start();
        }

        private void OnCreatedDeleted(object source, FileSystemEventArgs e)
        {
            _changed = ImageSourceEnum.Other;

            _reloadTimer.Start();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            SetChanged(e.FullPath);

            if (_changed == ImageSourceEnum.ImageSource)
                _renamed = e.FullPath;

            _reloadTimer.Start();
        }

        private void ReloadTimer_Tick(object sender, EventArgs e)
        {
            _reloadTimer.Stop();

            if (!string.IsNullOrEmpty(_renamed))
            {
                ReadImageFiles(true);

                if (_imageFiles.Any(f => f == _renamed))
                    _index = _imageFiles.IndexOf(_imageFiles.First(s => s == _renamed));

                _renamed = null;
            }
            else if (_changed == ImageSourceEnum.Other)
                ReadImageFiles(true);
            else
                NotifyPropertyChanged(_changed.ToString());

            _changed = ImageSourceEnum.None;
        }
        #endregion

        #region ICommand
        private void KeyCommandExecute(object parameter)
        {
            if (_imageFiles == null || _imageFiles.Count == 0)
                return;

            var key = parameter as string;

            if (key == "Up")
                _index = Math.Min(_index + Skip, _imageFiles.Count - 1);
            else if (key == "Down")
                _index = Math.Max(_index - Skip, 0);
            else if (key == "Left")
                _index = Math.Max(_index - 1, 0);
            else if (key == "Right")
                _index = Math.Min(_index + 1, _imageFiles.Count - 1);
            else if (key == "Home")
                _index = 0;
            else if (key == "End")
                _index = _imageFiles.Count - 1;

            SetImageSources();
        }

        private bool UpCommandCanExecute(object parameter)
        {
            return _imageFiles != null && _index + Skip < _imageFiles.Count;
        }

        private bool DownCommandCanExecute(object parameter)
        {
            return _imageFiles != null && _index - Skip >= 0;
        }

        private bool LeftCommandCanExecute(object parameter)
        {
            return _imageSource != null && _index > 0;
        }

        private bool RightCommandCanExecute(object parameter)
        {
            return _imageFiles != null && _index < _imageFiles.Count - 1;
        }

        private bool HomeCommandCanExecute(object parameter)
        {
            return _imageFiles != null && _imageSource != null && _imageFiles.Count > 0;
        }

        private bool EndCommandCanExecute(object parameter)
        {
            return _imageFiles != null && _imageSource != null && _imageFiles.Count > 0;
        }

        private void ZoomCommandExecute(object parameter)
        {
            string direction = parameter as string;

            if (string.IsNullOrEmpty(direction))
                return;

            if (direction == "Up" && _imageWidth * 2.0 <= MaxImageWidth && _imageHeight * 2.0 <= MaxImageHeight)
                ImageWidth = _imageWidth * 2.0;

            if (direction == "Down" && _imageWidth / 2.0 >= MinImageWidth && _imageHeight / 2.0 >= MinImageHeight)
                ImageWidth = _imageWidth / 2.0;
        }

        private bool ZoomCommandCanExecute(object parameter)
        {
            return _imageSource != null;
        }

        private void ToggleThumbsCommandExecute(object parameter)
        {
            AreThumbsVisible = !AreThumbsVisible;
            SetImageSources();
        }

        private void ReloadCommandExecute(object parameter)
        {
            ReadImageFiles(true);
        }

        private void HelpCommandExecute(object parameter)
        {
            IsHelpVisible = !IsHelpVisible;
        }

        private void SettingsCommandExecute(object parameter)
        {
            AreSettingsVisible = !AreSettingsVisible;
        }

        private void CopyCommandExecute(object parameter)
        {
            Clipboard.SetDataObject(
                new DataObject(
                    DataFormats.FileDrop,
                    new string[] { Uri.UnescapeDataString(ImageSource.AbsolutePath) },
                    true), true);
        }

        private bool CopyCommandCanExecute(object parameter)
        {
            return ImageSource != null;
        }

        private void OpenFileCommandExecute(object parameter)
        {
            var openFileDialog = new OpenFileDialog() { Filter = OpenFileDialogFilter };

            if (openFileDialog.ShowDialog() != true)
                return;

            ImageSource = new Uri(Path.GetFullPath(openFileDialog.FileName));
            ReadImageFiles(true);
        }

        private void SetCommandsExecutionStatus()
        {
            UpCommand.RaiseCanExecuteChanged();
            DownCommand.RaiseCanExecuteChanged();
            LeftCommand.RaiseCanExecuteChanged();
            RightCommand.RaiseCanExecuteChanged();
            HomeCommand.RaiseCanExecuteChanged();
            EndCommand.RaiseCanExecuteChanged();
            ZoomCommand.RaiseCanExecuteChanged();
            ToggleThumbsCommand.RaiseCanExecuteChanged();
            ReloadCommand.RaiseCanExecuteChanged();
            HelpCommand.RaiseCanExecuteChanged();
            SettingsCommand.RaiseCanExecuteChanged();
            CopyCommand.RaiseCanExecuteChanged();
            OpenFileCommand.RaiseCanExecuteChanged();
        }
        #endregion
        #endregion
    }
}