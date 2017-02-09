﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorPickerWPF.Code;
using UserControl = System.Windows.Controls.UserControl;

namespace ColorPickerWPF
{
    /// <summary>
    /// Interaction logic for ColorPickerControl.xaml
    /// </summary>
    public partial class ColorPickerControl : UserControl
    {
        public Color Color = Colors.White;

        public delegate void ColorPickerChangeHandler(Color color);

        public event ColorPickerChangeHandler OnPickColor;

        public List<ColorSwatchItem> ColorSwatch1 = new List<ColorSwatchItem>();
        public List<ColorSwatchItem> ColorSwatch2 = new List<ColorSwatchItem>();

        public bool IsSettingValues = false;

        protected const int NumColorsFirstSwatch = 39;
        protected const int NumColorsSecondSwatch = 112;

        public static ColorPalette ColorPalette;


        public ColorPickerControl()
        {
            InitializeComponent();

            if (ColorPalette == null)
            {
                // Load from file if possible

                ColorPalette = new ColorPalette();
                ColorPalette.InitializeDefaults();
            }


            ColorSwatch1.AddRange(ColorPalette.BuiltInColors.Take(NumColorsFirstSwatch).ToArray());

            ColorSwatch2.AddRange(ColorPalette.BuiltInColors.Skip(NumColorsFirstSwatch).Take(NumColorsSecondSwatch).ToArray());

            var megaColors = Util.GetWebColors();
            //ColorSwatch2.AddRange(megaColors);


            Swatch1.SwatchListBox.ItemsSource = ColorSwatch1;
            Swatch2.SwatchListBox.ItemsSource = ColorSwatch2;

            CustomColorSwatch.SwatchListBox.ItemsSource = ColorPalette.CustomColors;


            RSlider.Slider.Maximum = 255;
            GSlider.Slider.Maximum = 255;
            BSlider.Slider.Maximum = 255;
            ASlider.Slider.Maximum = 255;
            HSlider.Slider.Maximum = 360;
            SSlider.Slider.Maximum = 1;
            LSlider.Slider.Maximum = 1;


            RSlider.Label.Content = "R";
            RSlider.Slider.TickFrequency = 1;
            RSlider.Slider.IsSnapToTickEnabled = true;
            GSlider.Label.Content = "G";
            GSlider.Slider.TickFrequency = 1;
            GSlider.Slider.IsSnapToTickEnabled = true;
            BSlider.Label.Content = "B";
            BSlider.Slider.TickFrequency = 1;
            BSlider.Slider.IsSnapToTickEnabled = true;

            ASlider.Label.Content = "A";
            ASlider.Slider.TickFrequency = 1;
            ASlider.Slider.IsSnapToTickEnabled = true;

            HSlider.Label.Content = "H";
            HSlider.Slider.TickFrequency = 1;
            HSlider.Slider.IsSnapToTickEnabled = true;
            SSlider.Label.Content = "S";
            //SSlider.Slider.TickFrequency = 1;
            //SSlider.Slider.IsSnapToTickEnabled = true;
            LSlider.Label.Content = "V";
            //LSlider.Slider.TickFrequency = 1;
            //LSlider.Slider.IsSnapToTickEnabled = true;


            SetColor(Color);

        }


        public void SetColor(Color color)
        {
            Color = color;

            CustomColorSwatch.CurrentColor = color;

            IsSettingValues = true;

            RSlider.Slider.Value = Color.R;
            GSlider.Slider.Value = Color.G;
            BSlider.Slider.Value = Color.B;
            ASlider.Slider.Value = Color.A;

            SSlider.Slider.Value = Color.GetSaturation();
            LSlider.Slider.Value = Color.GetBrightness();
            HSlider.Slider.Value = Color.GetHue();

            ColorDisplayBorder.Background = new SolidColorBrush(Color);

            IsSettingValues = false;

        }


        protected void SampleImageClick(BitmapSource img, Point pos)
        {
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/82a5731e-e201-4aaf-8d4b-062b138338fe/getting-pixel-information-from-a-bitmapimage?forum=wpf

            int stride = (int) img.Width*4;
            int size = (int) img.Height*stride;
            byte[] pixels = new byte[(int) size];

            img.CopyPixels(pixels, stride, 0);


            // Get pixel
            var x = (int) pos.X;
            var y = (int) pos.Y;

            int index = y*stride + 4*x;

            byte red = pixels[index];
            byte green = pixels[index + 1];
            byte blue = pixels[index + 2];
            byte alpha = pixels[index + 3];

            var color = Color.FromArgb(alpha, blue, green, red);
            SetColor(color);
        }


        private void SampleImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(SampleImage);
            var img = SampleImage.Source as BitmapSource;
            SampleImageClick(img, pos);
        }

        private void SampleImage2_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(SampleImage2);
            var img = SampleImage2.Source as BitmapSource;
            SampleImageClick(img, pos);
        }

        private void Swatch_OnOnPickColor(Color color)
        {
            SetColor(color);
            OnPickColor?.Invoke(color);
        }

        private void HSlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var s = Color.GetSaturation();
                var l = Color.GetBrightness();
                var h = (float) value;
                var a = (int) ASlider.Slider.Value;
                Color = Util.FromAhsb(a, h, s, l);

                SetColor(Color);
            }
        }




        private void RSlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var val = (byte) value;
                Color.R = val;
                SetColor(Color);
            }
        }

        private void GSlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var val = (byte) value;
                Color.G = val;
                SetColor(Color);
            }
        }

        private void BSlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var val = (byte) value;
                Color.B = val;
                SetColor(Color);
            }
        }

        private void ASlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var val = (byte)value;
                Color.A = val;
                SetColor(Color);
            }
        }

        private void SSlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var s = (float) value;
                var l = Color.GetBrightness();
                var h = Color.GetHue();
                var a = (int) ASlider.Slider.Value;
                Color = Util.FromAhsb(a, h, s, l);

                SetColor(Color);
            }

        }

        private void PickerHueSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateImageForHSV();
        }


        private void UpdateImageForHSV()
        {

            //var hueChange = (int)((PickerHueSlider.Value / 360.0) * 240);
            var sliderHue = (float) PickerHueSlider.Value;


            //var colorPickerImage = System.IO.Path.Combine(Environment.CurrentDirectory, @"/Resources/colorpicker1.png");
            var img =
                new BitmapImage(new Uri("pack://application:,,,/ColorPickerWPF;component/Resources/colorpicker2.png",
                    UriKind.Absolute));

            var writableImage = BitmapFactory.ConvertToPbgra32Format(img);

            if (sliderHue <= 0f || sliderHue >= 360f)
            {
                // No hue change just return
                SampleImage2.Source = img;
                return;
            }

            using (var context = writableImage.GetBitmapContext())
            {
                long numPixels = img.PixelWidth*img.PixelHeight;

                for (int x = 0; x < img.PixelWidth; x++)
                {
                    for (int y = 0; y < img.PixelHeight; y++)
                    {
                        var pixel = writableImage.GetPixel(x, y);

                        var newHue = (float) (sliderHue + pixel.GetHue());
                        if (newHue >= 360)
                            newHue -= 360;

                        var color = Util.FromAhsb((int) 255,
                            newHue, pixel.GetSaturation(), pixel.GetBrightness());

                        writableImage.SetPixel(x, y, color);
                    }
                }
            }



            SampleImage2.Source = writableImage;
        }

        private void LSlider_OnOnValueChanged(double value)
        {
            if (!IsSettingValues)
            {
                var s = Color.GetSaturation();
                var l = (float) value;
                var h = Color.GetHue();
                var a = (int) ASlider.Slider.Value;
                Color = Util.FromAhsb(a, h, s, l);

                SetColor(Color);
            }
        }


        public void SaveCustomPalette(string filename)
        {
            var colors = CustomColorSwatch.GetColors();
            ColorPalette.CustomColors = colors;

            try
            {
                ColorPalette.SaveToXml(filename);
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

        public void LoadCustomPalette(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    ColorPalette = ColorPalette.LoadFromXml(filename);

                    CustomColorSwatch.SwatchListBox.ItemsSource = ColorPalette.CustomColors.ToList();
                }
                catch (Exception ex)
                {
                    ex = ex;
                }

            }
        }

    }
}
