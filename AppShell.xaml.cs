namespace Weather_App_2;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(View.ConfigurationPage), typeof(View.ConfigurationPage));
	}
}
