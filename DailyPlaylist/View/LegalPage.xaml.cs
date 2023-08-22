using System.IO;
using System.Reflection;

namespace DailyPlaylist.View;

public partial class LegalPage : ContentPage
{
    public LegalPage()
    {
        InitializeComponent();
        LanguagePicker.SelectedIndex = 0;
    }

    private async void OnLanguageChanged(object sender, EventArgs e)
    {
        var language = LanguagePicker.SelectedItem.ToString();
        TermsOfUseLabel.Text = await ReadTextFilesAsync(language);
    }

    private async Task<string> ReadTextFilesAsync(string language)
    {
        string fileName = language == "English" ? "TermsOfUseEN.txt" : "TermsOfUseFR.txt";
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream($"DailyPlaylist.Resources.Raw.{fileName}");
        using (var reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        TermsOfUseLabel.Text = await ReadTextFilesAsync("Français");
    }

}
