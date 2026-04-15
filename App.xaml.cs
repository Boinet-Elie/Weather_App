namespace Weather_App_2;

// Point d'entrée principal de l'application
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	// Crée et configure la fenêtre principale
	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());

#if MACCATALYST
		// Taille de fenêtre simulant un téléphone sur Mac
		window.Width = 400;
		window.Height = 850;
		window.MinimumHeight = 700;
		window.MinimumWidth = 350;
		window.MaximumHeight = 900;
		window.MaximumWidth = 450;
#endif

		return window;
	}
}