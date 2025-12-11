using Bunit;
using Xunit;
using DashComponents.Monitoring;
using DashLib.Network;
using System.Collections.Generic;

public class UptimeChartTest : TestContext
{
    [Fact]
    public void RendersUptimeChart_WhenVisible()
    {
        var ips = new List<IP>();
        var cut = RenderComponent<UptimeChart>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, true)
        );

        Assert.Contains("uptime_chart", cut.Markup);
    }
}