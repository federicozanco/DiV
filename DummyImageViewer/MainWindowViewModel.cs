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
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace DummyImageViewer
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constants
        public const String DEFAULT_WINDOW_TITLE = "DiV";

        public const Double DEFAULT_IMAGE_WIDTH = 640.0;
        public const Double DEFAULT_IMAGE_HEIGHT = 512.0;
        public const Double MIN_IMAGE_WIDTH = 20.0;
        public const Double MIN_IMAGE_HEIGHT = 16.0;
        public const Double MAX_IMAGE_WIDTH = 1280.0;
        public const Double MAX_IMAGE_HEIGHT = 1024.0;
        public const Double DEFAULT_THUMB_IMAGE_WIDTH = 160.0;
        public const Double DEFAULT_THUMB_IMAGE_HEIGHT = 128.0;

        public const int DEFAULT_SKIP = 32;

        public const String DEFAULT_IMAGE = "pack://application:,,,/notfound.png";
        #endregion

        #region Private
        public int[] Skips = { 8, 8, 16, 32 };
        public string[] ImageFormatExtensions = { ".jpg", ".png", ".gif", ".bmp" };

        private String windowTitle = DEFAULT_WINDOW_TITLE;
        
        private Uri imageSource = new Uri(DEFAULT_IMAGE);
        private Uri backward1ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri backward2ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri backward3ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri backward4ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri forward1ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri forward2ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri forward3ImageSource = new Uri(DEFAULT_IMAGE);
        private Uri forward4ImageSource = new Uri(DEFAULT_IMAGE);

        private Double imageWidth = DEFAULT_IMAGE_WIDTH;
        private Double imageHeight = DEFAULT_IMAGE_HEIGHT;
        private Double thumbImageWidth = DEFAULT_THUMB_IMAGE_WIDTH;
        private Double thumbImageHeight = DEFAULT_THUMB_IMAGE_HEIGHT;
        private List<string> imageFiles = null;
        private int index = -1;
        private int skip = DEFAULT_SKIP;

        private bool areThumbsVisible = true;
        private bool isHelpVisible = false;
        private bool areSettingsVisible = false;
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
            get { return windowTitle; }
            set
            {
                windowTitle = value;
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
            get { return imageSource; }
            set
            {
                imageSource = value;
                NotifyPropertyChanged("ImageSource");

                ReadImageFiles();

                if (imageFiles != null && index >= 0 && index < imageFiles.Count)
                    WindowTitle = DEFAULT_WINDOW_TITLE + " - " + Path.GetFileName(imageFiles[index]) + "   (" + (index + 1) + " / " + imageFiles.Count + ")";
                else
                    WindowTitle = DEFAULT_WINDOW_TITLE;

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
            get { return backward1ImageSource; }
            set
            {
                backward1ImageSource = value;
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
            get { return backward2ImageSource; }
            set
            {
                backward2ImageSource = value;
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
            get { return backward3ImageSource; }
            set
            {
                backward3ImageSource = value;
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
            get { return backward4ImageSource; }
            set
            {
                backward4ImageSource = value;
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
            get { return forward1ImageSource; }
            set
            {
                forward1ImageSource = value;
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
            get { return forward2ImageSource; }
            set
            {
                forward2ImageSource = value;
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
            get { return forward3ImageSource; }
            set
            {
                forward3ImageSource = value;
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
            get { return forward4ImageSource; }
            set
            {
                forward4ImageSource = value;
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
            get { return imageWidth; }
            set
            {
                double ratio = imageHeight / imageWidth;

                imageWidth = value;
                NotifyPropertyChanged("ImageWidth");

                ImageHeight = imageWidth * ratio;

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
            get { return imageHeight; }
            set
            {
                imageHeight = value;
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
            get { return thumbImageWidth; }
            set
            {
                double ratio = thumbImageHeight / thumbImageWidth;

                thumbImageWidth = value;
                NotifyPropertyChanged("ThumbImageWidth");

                ThumbImageHeight = thumbImageWidth * ratio;

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
            get { return thumbImageHeight; }
            set
            {
                thumbImageHeight = value;
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
            get { return skip; }
            set
            {
                skip = value;
                NotifyPropertyChanged("Skip");
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
            get { return areThumbsVisible; }
            set
            {
                areThumbsVisible = value;
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
            get { return isHelpVisible; }
            set
            {
                isHelpVisible = value;
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
            get { return areSettingsVisible; }
            set
            {
                areSettingsVisible = value;
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
        public ICommand UpCommand { get; set; }

        /// <summary>
        /// Gets or sets down command.
        /// </summary>
        /// <value>
        /// Down command.
        /// </value>
        public ICommand DownCommand { get; set; }

        /// <summary>
        /// Gets or sets the left command.
        /// </summary>
        /// <value>
        /// The left command.
        /// </value>
        public ICommand LeftCommand { get; set; }

        /// <summary>
        /// Gets or sets the right command.
        /// </summary>
        /// <value>
        /// The right command.
        /// </value>
        public ICommand RightCommand { get; set; }

        /// <summary>
        /// Gets or sets the home command.
        /// </summary>
        /// <value>
        /// The home command.
        /// </value>
        public ICommand HomeCommand { get; set; }

        /// <summary>
        /// Gets or sets the end command.
        /// </summary>
        /// <value>
        /// The end command.
        /// </value>
        public ICommand EndCommand { get; set; }

        /// <summary>
        /// Gets or sets the zoom command.
        /// </summary>
        /// <value>
        /// The zoom command.
        /// </value>
        public ICommand ZoomCommand { get; set; }

        /// <summary>
        /// Gets or sets the toggle thumbs command.
        /// </summary>
        /// <value>
        /// The toggle thumbs command.
        /// </value>
        public ICommand ToggleThumbsCommand { get; set; }

        /// <summary>
        /// Gets or sets the reload command.
        /// </summary>
        /// <value>
        /// The reload command.
        /// </value>
        public ICommand ReloadCommand { get; set; }

        /// <summary>
        /// Gets or sets the help command.
        /// </summary>
        /// <value>
        /// The help command.
        /// </value>
        public ICommand HelpCommand { get; set; }

        /// <summary>
        /// Gets or sets the options command.
        /// </summary>
        /// <value>
        /// The options command.
        /// </value>
        public ICommand SettingsCommand { get; set; }

        /// <summary>
        /// Gets or sets the copy command.
        /// </summary>
        /// <value>
        /// The copy command.
        /// </value>
        public ICommand CopyCommand { get; set; }
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
            ToggleThumbsCommand = new SimpleDelegateCommand(ToggleThumbsCommandExecute, ToggleThumbsCommandCanExecute);
            ReloadCommand = new SimpleDelegateCommand(ReloadCommandExecute, ReloadCommandCanExecute);
            HelpCommand = new SimpleDelegateCommand(HelpCommandExecute, HelpCommandCanExecute);
            SettingsCommand = new SimpleDelegateCommand(SettingsCommandExecute, SettingsCommandCanExecute);
            CopyCommand = new SimpleDelegateCommand(CopyCommandExecute, CopyCommandCanExecute);

            SkipValues = new List<int>();
            for (int i = 2; i <= 1024; i *= 2)
                SkipValues.Add(i);

            ImageWidthValues = new List<Double>();
            Double w = MIN_IMAGE_WIDTH;
            while (w <= MAX_IMAGE_WIDTH)
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
            if (!force && imageFiles != null && imageFiles.Count > 0)
                return;

            string filename = Path.GetFullPath(Uri.UnescapeDataString(imageSource.AbsolutePath));
            string[] files = Directory.GetFiles(Directory.GetParent(filename).FullName);
            imageFiles = new List<string>();

            int ifel = ImageFormatExtensions.Length;

            for (int i = 0; i < files.Length; i++)
            {
                string suffix = files[i].Substring(files[i].Length - 4).ToLower();

                for (int f = 0; f < ifel; f++)
                {
                    if (suffix == ImageFormatExtensions[f])
                    {
                        imageFiles.Add(files[i]);

                        if (Path.GetFullPath(files[i]) == filename)
                            index = imageFiles.Count - 1;

                        break;
                    }
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
            if (imageFiles == null || imageFiles.Count == 0)
                return;

            while (imageFiles.Count > 0 && index < imageFiles.Count)
            {
                try
                {
                    SetImageSource(-1, new Uri(imageFiles[index]));
                    break;
                }
                catch (NotSupportedException)
                {
                    // System.NotSupportedException:
                    // No imaging component suitable to complete this operation was found.
                    imageFiles.RemoveAt(index);
                }
            }

            if (!areThumbsVisible)
                return;

            int bIndex = index;

            for (int b = 0; b < 4; b++)
            {
                bIndex -= Skips[b];

                while (bIndex > 0 && imageFiles.Count > 0 && bIndex < imageFiles.Count)
                {
                    try
                    {
                        SetImageSource(b, new Uri(imageFiles[bIndex]));
                        break;
                    }
                    catch (NotSupportedException)
                    {
                        // System.NotSupportedException:
                        // No imaging component suitable to complete this operation was found.
                        imageFiles.RemoveAt(bIndex);
                        index--;
                    }
                }

                if (bIndex < 0)
                    SetImageSource(b, new Uri(DEFAULT_IMAGE));
            }


            int fIndex = index;

            for (int f = 0; f < 4; f++)
            {
                fIndex += Skips[f];

                while (imageFiles.Count > 0 && fIndex < imageFiles.Count)
                {
                    try
                    {
                        SetImageSource(f, new Uri(imageFiles[fIndex]), false);
                        break;
                    }
                    catch (NotSupportedException)
                    {
                        // System.NotSupportedException:
                        // No imaging component suitable to complete this operation was found.
                        imageFiles.RemoveAt(fIndex);
                    }
                }

                if (fIndex > imageFiles.Count)
                    SetImageSource(f, new Uri(DEFAULT_IMAGE), false);
            }
        }

        #region ICommand
        private void KeyCommandExecute(object parameter)
        {
            if (imageFiles == null || imageFiles.Count == 0)
                return;

            string key = parameter as string;

            if (key == "Up")
                index = Math.Min(index + Skip, imageFiles.Count - 1);
            else if (key == "Down")
                index = Math.Max(index - Skip, 0);
            else if (key == "Left")
                index = Math.Max(index - 1, 0);
            else if (key == "Right")
                index = Math.Min(index + 1, imageFiles.Count - 1);
            else if (key == "Home")
                index = 0;
            else if (key == "End")
                index = imageFiles.Count - 1;

            SetImageSources();
        }

        private bool UpCommandCanExecute(object parameter)
        {
            return imageFiles != null && index + Skip < imageFiles.Count;
        }

        private bool DownCommandCanExecute(object parameter)
        {
            return imageFiles != null && index - Skip >= 0;
        }

        private bool LeftCommandCanExecute(object parameter)
        {
            return imageSource != null && index > 0;
        }

        private bool RightCommandCanExecute(object parameter)
        {
            return imageFiles != null && index < imageFiles.Count - 1;
        }

        private bool HomeCommandCanExecute(object parameter)
        {
            return imageFiles != null && imageSource != null && imageFiles.Count > 0;
        }

        private bool EndCommandCanExecute(object parameter)
        {
            return imageFiles != null && imageSource != null && imageFiles.Count > 0;
        }

        private void ZoomCommandExecute(object parameter)
        {
            string direction = parameter as string;

            if (string.IsNullOrEmpty(direction))
                return;

            if (direction == "Up" && imageWidth * 2.0 <= MAX_IMAGE_WIDTH && imageHeight * 2.0 <= MAX_IMAGE_HEIGHT)
            {
                ImageWidth = imageWidth * 2.0;
            }

            if (direction == "Down" && imageWidth / 2.0 >= MIN_IMAGE_WIDTH && imageHeight / 2.0 >= MIN_IMAGE_HEIGHT)
            {
                ImageWidth = imageWidth / 2.0;
            }
        }

        private bool ZoomCommandCanExecute(object parameter)
        {
            return imageSource != null;
        }

        private void ToggleThumbsCommandExecute(object parameter)
        {
            AreThumbsVisible = !AreThumbsVisible;
            SetImageSources();
        }

        private bool ToggleThumbsCommandCanExecute(object parameter)
        {
            return true;
        }

        private void ReloadCommandExecute(object parameter)
        {
            ReadImageFiles(true);
        }

        private bool ReloadCommandCanExecute(object parameter)
        {
            return true;
        }

        private void HelpCommandExecute(object parameter)
        {
            IsHelpVisible = !IsHelpVisible;
        }

        private bool HelpCommandCanExecute(object parameter)
        {
            return true;
        }

        private void SettingsCommandExecute(object parameter)
        {
            AreSettingsVisible = !AreSettingsVisible;
        }

        private bool SettingsCommandCanExecute(object parameter)
        {
            return true;
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
            (UpCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (DownCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (LeftCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (RightCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (HomeCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (EndCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (ZoomCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (ToggleThumbsCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (ReloadCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (HelpCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (SettingsCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
            (CopyCommand as SimpleDelegateCommand).RaiseCanExecuteChanged();
        }
        #endregion
        #endregion
    }
}