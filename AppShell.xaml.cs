namespace Weather_App_2;

// Shell principal gérant la navigation de l'application
public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// Enregistre la route vers la page de configuration
		Routing.RegisterRoute(nameof(View.ConfigurationPage), typeof(View.ConfigurationPage));
	}
}
