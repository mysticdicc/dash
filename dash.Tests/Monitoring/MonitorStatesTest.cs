using Bunit;
using Xunit;
using dash.Components.Monitoring;
using danklibrary.Network;
using System.Collections.Generic;

public class MonitorStatesTest : TestContext
{
    [Fact]
    public void RendersMonitorStates_WhenVisible()
    {
        var ips = new List<IP>();
        var cut = RenderComponent<MonitorStates>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, true)
        );

        Assert.Contains("monitor_states", cut.Markup);
    }
}