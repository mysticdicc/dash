using Bunit;
using DashComponents.Monitoring;
using DashLib.Interfaces;
using DashLib.Monitoring;
using DashLib.Network;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Components;

public class StatsTest : BunitContext
{
    public (IRenderedComponent<Stats>, Mock<IMonitoringAPI>) CreateStandardComponent(BunitServiceProvider services, bool visible)
    {
        var monitoringApi = new Mock<IMonitoringAPI>();
        services.AddSingleton(monitoringApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ips = subnet.List;

        foreach (var ip in ips)
        {
            ip.MonitorStateList = MonitorState.CreateRandomListOfMonitorStates(ip);
        };

        var cut = Render<Stats>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, visible)
        );

        return (cut, monitoringApi);
    }

    [Fact]
    public void RendersStats_WhenVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.Contains("stats_card", cut.Markup);
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
    public void StatsNotVisible_HidesStats()
    {
        var (cut, api) = CreateStandardComponent(Services, false);

        Assert.DoesNotContain("stats_card", cut.Markup);
    }

    [Fact]
    public void ClickExpanded_CallsEventCallback()
    {
        var (cut, api) = CreateStandardComponent(Services, true);
        bool invoked = false;

        cut.Render(parameters => parameters.Add(p => p.ExpandChanged, EventCallback.Factory.Create<string>(this, ip =>
        {
            invoked = true;
        })));

        cut.Find("button#expand").Click();

        Assert.True(invoked);
    }
}