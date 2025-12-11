using Bunit;
using DashComponents.Monitoring;
using DashLib.Interfaces;
using DashLib.Monitoring;
using DashLib.Network;
using Microsoft.AspNetCore.Components;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

public class StatsChartTest : TestContext
{
    public (IRenderedComponent<StatsChart>, Mock<IMonitoringAPI>) CreateStandardComponent(TestServiceProvider services, bool visible)
    {
        var monitoringApi = new Mock<IMonitoringAPI>();
        services.AddSingleton(monitoringApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ips = subnet.List;

        foreach (var ip in ips)
        {
            ip.MonitorStateList = MonitorState.CreateRandomListOfMonitorStates(ip);
        };

        var cut = RenderComponent<StatsChart>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, visible)
        );

        return (cut, monitoringApi);
    }

    [Fact]
    public void RendersStatsChart_WhenVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.Contains("apex_chart", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderStatsChart_WhenNotVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, false);
        Assert.DoesNotContain("apex_chart", cut.Markup);
    }

    [Fact]
    public void ClickExpanded_TogglesExpanded()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.False(cut.Instance._expanded);
        cut.Find("button#expand").Click();
        Assert.True(cut.Instance._expanded);
    }

    [Fact]
    public void ClickExpanded_CallsEventCallback()
    {
        var (cut, api) = CreateStandardComponent(Services, true);
        bool invoked = false;

        cut.SetParametersAndRender(parameters => parameters.Add(p => p.ExpandChanged, EventCallback.Factory.Create<string>(this, ip =>
        {
            invoked = true;
        })));

        cut.Find("button#expand").Click();

        Assert.True(invoked);
    }

    [Fact]
    public void ClickOnline_TogglesOnlineVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.False(cut.Instance._onlineHidden);
        cut.Find("button#onlinetoggle").Click();
        Assert.True(cut.Instance._onlineHidden);
    }

    [Fact]
    public void ClickOffline_TogglesOfflineVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.False(cut.Instance._offlineHidden);
        cut.Find("button#offlinetoggle").Click();
        Assert.True(cut.Instance._offlineHidden);
    }

    [Fact]
    public void ClickMonitored_TogglesMonitoredVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.False(cut.Instance._monitoredHidden);
        cut.Find("button#monitoredtoggle").Click();
        Assert.True(cut.Instance._monitoredHidden);
    }
}