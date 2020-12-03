﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using ImageConverterLibrary;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;

namespace ImageProcessingApp
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        private string imagePath;
        private ImageSource imageResult;
        private string time;
        #endregion

        #region Properties
        public string ImagePath
        {
            get => imagePath;
            set { imagePath = value; OnPropertyChanged(); }
        }
        public ImageSource ImageResult
        {
            get { return imageResult; }
            set { imageResult = value; OnPropertyChanged(); }
        }
        public string Time
        {
            get => time;
            set { time = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand LoadImageCommand { get; set; }
        public ICommand LoadDefaultImageCommand { get; set; }
        public ICommand SaveImageCommand { get; set; }
        public ICommand ConvertRGBCommand { get; set; }
        public ICommand ConvertRGBAsyncCommand { get; set; }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            LoadDefaultImageCommand = new RelayCommand(LoadDefaultImage);
            SaveImageCommand = new RelayCommand(SaveImage);
            ConvertRGBCommand = new RelayCommand(ConvertRGB);
            ConvertRGBAsyncCommand = new RelayCommand(ConvertRGBAsync);
        }
        #endregion

        #region Methods

        private void LoadImage()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = (".png");
            open.Filter = "Pictures (*.jpg;*.gif;*.png)|*.jpg;*.gif;*.png";

            if (open.ShowDialog() == true)
                ImagePath = open.FileName;
        }

        private void LoadDefaultImage()
        {
            ImagePath = Path.Combine(Environment.CurrentDirectory, "../../Images/default.jpg");
        }

        private void SaveImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pictures (*.jpg;*.gif;*.png)|*.jpg;*.gif;*.png";
            string tempPath = "";

            try
            {
                if (saveFileDialog.ShowDialog() ?? false)
                    tempPath = Path.GetFullPath(saveFileDialog.FileName);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)ImageResult));

                File.Copy(imagePath, tempPath);

                using (var fileStream = new System.IO.FileStream(tempPath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occured: \n" + e.Message);
            }
        }
        private void ConvertRGB()
        {
            Bitmap bitmap = null;
            ConvertRGB ConvertRGB = new ConvertRGB();

            var watch = Stopwatch.StartNew();        
            bitmap = ConvertRGB.ConvertRGBValue(imagePath);
            watch.Stop();

            ImageResult = ConvertRGB.BitmapToImageSource(bitmap);
            Time = "Convert working time: " + watch.ElapsedMilliseconds + " ms";
        }
        private async void ConvertRGBAsync()
        {
            if (ImagePath == null)
            {
                MessageBox.Show("There is nothing to convert!");
                return;
            }

            Bitmap bitmap = null;
            ConvertRGB ConvertRGB = new ConvertRGB();

            var watch = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                bitmap = ConvertRGB.ConvertRGBValue(imagePath);
            });
            watch.Stop();

            ImageResult = ConvertRGB.BitmapToImageSource(bitmap);
            Time = "Convert working time: " + watch.ElapsedMilliseconds + " ms";
        }
        #endregion
    }
}
