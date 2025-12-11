using Bunit;
using Xunit;
using DashComponents.Monitoring;
using DashLib.Network;
using DashLib.Monitoring;
using System.Collections.Generic;

public class MonitorStatesHistoryTest : TestContext
{
    [Fact]
    public void RendersMonitorStatesHistory_WhenVisible()
    {
        var states = new List<MonitorState> { new MonitorState { SubmitTime = System.DateTime.Now } };
        var cut = RenderComponent<MonitorStatesHistory>(parameters => parameters
            .Add(p => p.MonitorStates, states)
            .Add(p => p.MonitorHistoryVisible, true)
        );

        Assert.Contains("monitor_history", cut.Markup);
    }
}