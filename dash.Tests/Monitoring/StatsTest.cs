using Bunit;
using Xunit;
using dash.Components.Monitoring;
using danklibrary.Network;
using System.Collections.Generic;

public class StatsTest : TestContext
{
    [Fact]
    public void RendersStats_WhenVisible()
    {
        var ips = new List<IP>();
        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, true)
        );

        Assert.Contains("stats_card", cut.Markup);
    }
}