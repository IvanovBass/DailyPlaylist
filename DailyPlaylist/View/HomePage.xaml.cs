using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace DailyPlaylist.View
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            StartAnimations();
        }

        private async void StartAnimations()
        {
            while (true)
            {
                await AnimateLogoImage();
                await Task.WhenAll(AnimateButton(), AnimateSecondLabel());
                await Task.Delay(3000); // 3 seconds delay
            }
        }

        private async Task AnimateLogoImage()
        {
            await logoImage.ScaleTo(1.12, 800, Easing.SinIn);
            await logoImage.ScaleTo(1, 600, Easing.SinOut);
            for (int i = 0; i < 2; i++)
            {
                await logoImage.ScaleTo(1.10, 700, Easing.SinIn);
                await logoImage.ScaleTo(1, 500, Easing.SinOut);
            }
        }

        private async Task AnimateButton()
        {
            await PlaylistGenBtn.ScaleTo(1.25, 800, Easing.SinIn);
            await PlaylistGenBtn.ScaleTo(1, 600, Easing.SinOut);
            for (int i = 0; i < 2; i++)
            {
                await PlaylistGenBtn.ScaleTo(1.15, 700, Easing.SinIn);
                await PlaylistGenBtn.ScaleTo(1, 600, Easing.SinOut);
            }
        }

        private async Task AnimateSecondLabel()
        {
            
            await instructionLabel.ScaleTo(1.2, 1300, Easing.SinIn);
            await instructionLabel.ScaleTo(1, 1100, Easing.SinOut);
        }
    }
}
