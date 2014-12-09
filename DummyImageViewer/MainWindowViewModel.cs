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
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DummyImageViewer
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constants
        public const String DefaultWindowTitle = "DiV";

        public const Double DefaultImageWidth = 640.0;
        public const Double DefaultImageHeight = 512.0;
        public const Double MinImageWidth = 20.0;
        public const Double MinImageHeight = 16.0;
        public const Double MaxImageWidth = 1280.0;
        public const Double MaxImageHeight = 1024.0;
        public const Double DefaultThumbImageWidth = 160.0;
        public const Double DefaultThumbImageHeight = 128.0;

        public const int DefaultSkip = 32;

        public const String DefaultImage = "pack://application:,,,/notfound.png";
        #endregion

        #region Private
        // NOTE: skips are half value of the current shift (+=/-=)
        private readonly int[] _skips = { 8, 8, 16, 32 };
        private readonly string[] _imageFormatExtensions = { ".jpg", ".png", ".gif", ".bmp" };

        private String _windowTitle = DefaultWindowTitle;
        
        private Uri _imageSource = new Uri(DefaultImage);
        private Uri _backward1ImageSource = new Uri(DefaultImage);
        private Uri _backward2ImageSource = new Uri(DefaultImage);
        private Uri _backward3ImageSource = new Uri(DefaultImage);
        private Uri _backward4ImageSource = new Uri(DefaultImage);
        private Uri _forward1ImageSource = new Uri(DefaultImage);
        private Uri _forward2ImageSource = new Uri(DefaultImage);
        private Uri _forward3ImageSource = new Uri(DefaultImage);
        private Uri _forward4ImageSource = new Uri(DefaultImage);

        private Double _imageWidth = DefaultImageWidth;
        private Double _imageHeight = DefaultImageHeight;
        private Double _thumbImageWidth = DefaultThumbImageWidth;
        private Double _thumbImageHeight = DefaultThumbImageHeight;
        private List<string> _imageFiles;
        private int _index = -1;
        private int _skip = DefaultSkip;

        private bool _areThumbsVisible = true;
        private bool _isHelpVisible;
        private bool _areSettingsVisible;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>
        /// The window title.
        /// </value>
        public String WindowTitle
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
                _imageSource = value;
                NotifyPropertyChanged("ImageSource");

                ReadImageFiles();

                if (_imageFiles != null && _index >= 0 && _index < _imageFiles.Count)
                    WindowTitle = DefaultWindowTitle + " - " + Path.GetFileName(_imageFiles[_index]) + "   (" + (_index + 1) + " / " + _imageFiles.Count + ")";
                else
                    WindowTitle = DefaultWindowTitle;

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
        public Double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                double ratio = _imageHeight / _imageWidth;

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
        public Double ImageHeight
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
        public Double ThumbImageWidth
        {
            get { return _thumbImageWidth; }
            set
            {
                double ratio = _thumbImageHeight / _thumbImageWidth;

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
        public Double ThumbImageHeight
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
        public List<Double> ImageWidthValues { get; set; }

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

                int s = _skip / 2;
                for (int i = 3; i > 0; i--)
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
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// Viene generato quando il valore di una proprietà cambia.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
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

            SkipValues = new List<int>();
            for (int i = 16; i <= 1024; i *= 2)
                SkipValues.Add(i);

            ImageWidthValues = new List<Double>();
            Double w = MinImageWidth;
            while (w <= MaxImageWidth)
            {
                ImageWidthValues.Add(w);
                w *= 2;
            }
        }
        #endregion

        #region Public methods
        #endregion

        #region Private
        private void ReadImageFiles(bool force = false)
        {
            if (!force && _imageFiles != null && _imageFiles.Count > 0)
                return;

            string filename = Path.GetFullPath(Uri.UnescapeDataString(_imageSource.AbsolutePath));
            string[] files = Directory.GetFiles(Directory.GetParent(filename).FullName);
            _imageFiles = new List<string>();

            int ifel = _imageFormatExtensions.Length;

            foreach (string t in files)
            {
                string suffix = t.Substring(t.Length - 4).ToLower();

                for (int f = 0; f < ifel; f++)
                {
                    if (suffix != _imageFormatExtensions[f])
                        continue;

                    _imageFiles.Add(t);

                    if (Path.GetFullPath(t) == filename)
                        _index = _imageFiles.Count - 1;

                    break;
                }
            }

            SetImageSources();
        }

        private void SetImageSource(int source, Uri uri, bool isBackward = true)
        {
            switch (source)
            {
                case 0:
                    if (isBackward)
                        Backward1ImageSource = uri;
                    else
                        Forward1ImageSource = uri;
                    break;

                case 1:
                    if (isBackward)
                        Backward2ImageSource = uri;
                    else
                        Forward2ImageSource = uri;
                    break;

                case 2:
                    if (isBackward)
                        Backward3ImageSource = uri;
                    else
                        Forward3ImageSource = uri;
                    break;

                case 3:
                    if (isBackward)
                        Backward4ImageSource = uri;
                    else
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
                    SetImageSource(-1, new Uri(_imageFiles[_index]));
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

            int bIndex = _index;

            for (int b = 0; b < 4; b++)
            {
                bIndex -= _skips[b];

                while (bIndex >= 0 && _imageFiles.Count > 0 && bIndex < _imageFiles.Count)
                {
                    try
                    {
                        SetImageSource(b, new Uri(_imageFiles[bIndex]));
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
                    SetImageSource(b, new Uri(DefaultImage));
            }

            int fIndex = _index + _skips[0] / 2;

            for (int f = 0; f < 4; f++)
            {
                fIndex += _skips[f];

                while (_imageFiles.Count > 0 && fIndex < _imageFiles.Count)
                {
                    try
                    {
                        SetImageSource(f, new Uri(_imageFiles[fIndex]), false);
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
                    SetImageSource(f, new Uri(DefaultImage), false);
            }
        }

        #region ICommand
        private void KeyCommandExecute(object parameter)
        {
            if (_imageFiles == null || _imageFiles.Count == 0)
                return;

            string key = parameter as string;

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
            {
                ImageWidth = _imageWidth * 2.0;
            }

            if (direction == "Down" && _imageWidth / 2.0 >= MinImageWidth && _imageHeight / 2.0 >= MinImageHeight)
            {
                ImageWidth = _imageWidth / 2.0;
            }
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
            DataObject objData = new DataObject();
            string[] filename = new string[1];
            filename[0] = Uri.UnescapeDataString(ImageSource.AbsolutePath);
            objData.SetData(DataFormats.FileDrop, filename, true);
            Clipboard.SetDataObject(objData, true);
        }

        private bool CopyCommandCanExecute(object parameter)
        {
            return ImageSource != null;
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
        }
        #endregion
        #endregion
    }
}