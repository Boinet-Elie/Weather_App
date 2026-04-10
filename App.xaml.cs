namespace Weather_App_2;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());

#if MACCATALYST
		// Configure la taille de la fenêtre pour simuler un téléphone sur Mac Catalyst
		// iPhone 14 Pro: 393 x 852 pixels (taille logique)
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