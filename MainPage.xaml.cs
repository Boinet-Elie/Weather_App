namespace Weather_App_2;

using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Reflection;

// Page principale affichant les prévisions météo
public partial class MainPage : ContentPage
{
    private readonly HttpClient _client;
    private string _apiUrl = "https://www.prevision-meteo.ch/services/json/";

    public MainPage()
    {
        InitializeComponent();
        _client = new HttpClient();
        LoadUserInfo();
        _ = LoadWeatherAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInfo();
        _ = LoadWeatherAsync();
    }

    // Charge le nom de l'utilisateur
    private void LoadUserInfo()
    {
        var firstName = Preferences.Get("firstName", "");
        GreetingLabel.Text = string.IsNullOrEmpty(firstName) ? "Bonjour" : $"Bonjour, {firstName}";
    }

    // Récupère les données horaires via réflexion
    private object? GetHourlyData(HourlyData? hourlyData, int hour)
    {
        if (hourlyData == null) return null;

        var propertyName = hour < 10 ? $"_{hour:D1}H00" : $"_{hour}H00";
        var property = hourlyData.GetType().GetProperty(propertyName);

        if (property == null)
        {
            propertyName = $"_{hour:D2}H00";
            property = hourlyData.GetType().GetProperty(propertyName);
        }

        return property?.GetValue(hourlyData);
    }

    // Charge les données météo depuis l'API
    private async Task LoadWeatherAsync()
    {
        var city = Preferences.Get("city", "Annecy");
        _apiUrl = $"https://www.prevision-meteo.ch/services/json/{city}";

        try
        {
            var response = await _client.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<Root>(content);

                if (weather != null && weather.city_info != null)
                {
                    // Affiche les infos de la ville
                    CityLabel.Text = weather.city_info.name;
                    TodayCondition.Text = weather.fcst_day_0.condition;
                    TodayIcon.Source = weather.fcst_day_0.icon_big;

                    // Récupère la température actuelle
                    var currentHour = DateTime.Now.Hour;
                    var hourData = GetHourlyData(weather.fcst_day_0.hourly_data, currentHour);

                    if (hourData != null)
                    {
                        var tempProperty = hourData.GetType().GetProperty("TMP2m");
                        if (tempProperty != null)
                        {
                            var temp = tempProperty.GetValue(hourData);
                            TodayTemps.Text = $"{Math.Round(Convert.ToDouble(temp))}°";
                        }
                        else
                        {
                            TodayTemps.Text = $"{weather.fcst_day_0.tmax}°";
                        }
                    }
                    else
                    {
                        TodayTemps.Text = $"{weather.fcst_day_0.tmax}°";
                    }

                    // Affiche les prévisions sur 5 jours
                    ForecastContainer.Children.Clear();
                    AddDayCard("Aujourd'hui", weather.fcst_day_0.icon, weather.fcst_day_0.tmax, weather.fcst_day_0.tmin, weather.fcst_day_0.condition);
                    AddDayCard("Demain", weather.fcst_day_1.icon, weather.fcst_day_1.tmax, weather.fcst_day_1.tmin, weather.fcst_day_1.condition);
                    AddDayCard(weather.fcst_day_2.day_short, weather.fcst_day_2.icon, weather.fcst_day_2.tmax, weather.fcst_day_2.tmin, weather.fcst_day_2.condition);
                    AddDayCard(weather.fcst_day_3.day_short, weather.fcst_day_3.icon, weather.fcst_day_3.tmax, weather.fcst_day_3.tmin, weather.fcst_day_3.condition);
                    AddDayCard(weather.fcst_day_4.day_short, weather.fcst_day_4.icon, weather.fcst_day_4.tmax, weather.fcst_day_4.tmin, weather.fcst_day_4.condition);
                }
                else
                {
                    CityLabel.Text = "Erreur: données nulles";
                }
            }
            else
            {
                CityLabel.Text = $"Erreur HTTP: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            CityLabel.Text = $"Erreur: {ex.Message}";
        }
    }

    // Crée une carte de prévision
    private void AddDayCard(string day, string icon, int tmax, int tmin, string condition)
    {
        var card = new Border
        {
            BackgroundColor = Color.FromArgb("#1E293B"),
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            StrokeThickness = 1,
            Stroke = Color.FromArgb("#334155"),
            Padding = new Thickness(16, 18),
            Shadow = new Shadow
            {
                Brush = Color.FromArgb("#000000"),
                Offset = new Point(0, 4),
                Radius = 12,
                Opacity = 0.15f
            },
            Content = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(80) },
                    new ColumnDefinition { Width = new GridLength(60) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 8,
                VerticalOptions = LayoutOptions.Center
            }
        };

        var grid = card.Content as Grid;

        // Jour
        var dayLabel = new Label
        {
            Text = day,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(dayLabel, 0);
        grid.Children.Add(dayLabel);

        // Icône
        var weatherIcon = new Image
        {
            Source = icon,
            WidthRequest = 44,
            HeightRequest = 44,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(weatherIcon, 1);
        grid.Children.Add(weatherIcon);

        // Condition
        var conditionLabel = new Label
        {
            Text = condition,
            FontSize = 14,
            TextColor = Color.FromArgb("#94A3B8"),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        Grid.SetColumn(conditionLabel, 2);
        grid.Children.Add(conditionLabel);

        // Températures
        var tempsContainer = new VerticalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Label
                {
                    Text = $"{tmax}°",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#F97316"),
                    HorizontalOptions = LayoutOptions.End
                },
                new Label
                {
                    Text = $"{tmin}°",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#22D3EE"),
                    HorizontalOptions = LayoutOptions.End
                }
            }
        };
        Grid.SetColumn(tempsContainer, 3);
        grid.Children.Add(tempsContainer);

        ForecastContainer.Children.Add(card);
    }

    // Ouvre la page de configuration
    private async void OnConfigurationClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(View.ConfigurationPage));
    }
}