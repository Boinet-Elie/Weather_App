namespace Weather_App_2.View;

public partial class ConfigurationPage : ContentPage
{
    public ConfigurationPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    // Charge les paramètres sauvegardés
    private void LoadSettings()
    {
        LastNameEntry.Text = Preferences.Get("lastName", "");
        FirstNameEntry.Text = Preferences.Get("firstName", "");
        CityEntry.Text = Preferences.Get("city", "");
    }

    // Sauvegarde et retourne à la page précédente
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        Preferences.Set("lastName", LastNameEntry.Text);
        Preferences.Set("firstName", FirstNameEntry.Text);
        Preferences.Set("city", CityEntry.Text);

        await DisplayAlert("Succès", "Paramètres enregistrés", "OK");
        await Shell.Current.GoToAsync("..");
    }
}