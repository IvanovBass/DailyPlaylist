namespace DailyPlaylist.ViewModel
{
    public static class AnimationHelper
    {
        public static async Task AnimatePressedImageButton(ImageButton button)
        {
            await button.ScaleTo(0.8, 130, Easing.Linear);
            await button.ScaleTo(1, 70, Easing.Linear);
        }
    }
}
