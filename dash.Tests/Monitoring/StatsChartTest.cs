using Bunit;
using Xunit;
using dash.Components.Monitoring;
using danklibrary.Network;
using System.Collections.Generic;

public class StatsChartTest : TestContext
{
    [Fact]
    public void RendersStatsChart_WhenVisible()
    {
        var ips = new List<IP>();
        var cut = RenderComponent<StatsChart>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, true)
        );

        Assert.Contains("apex_chart", cut.Markup);
    }
}