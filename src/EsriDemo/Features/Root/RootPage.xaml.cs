namespace EsriDemo.Features.Root;

public partial class RootPage
{
    public RootPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        
        BuildTabs(serviceProvider);
    }

    private void BuildTabs(IServiceProvider serviceProvider)
    {
        // Children.Add(serviceProvider.GetRequiredService<Feature1TabPage>());
        // Children.Add(serviceProvider.GetRequiredService<Feature2TabPage>());
        // Children.Add(serviceProvider.GetRequiredService<Feature3TabPage>());
    }
}