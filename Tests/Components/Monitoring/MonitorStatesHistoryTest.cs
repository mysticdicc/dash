using Bunit;
using DashComponents.Monitoring;
using DashLib.Interfaces;
using DashLib.Monitoring;
using DashLib.Network;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Linq;
using Xunit;

public class MonitorStatesHistoryTest : BunitContext
{
    public (IRenderedComponent<MonitorStatesHistory>, Mock<IMonitoringAPI>) CreateStandardComponent(BunitServiceProvider services, bool visible)
    {
        var monitoringApi = new Mock<IMonitoringAPI>();
        services.AddSingleton(monitoringApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ips = subnet.List;

        List<MonitorState> states = [];

        foreach (var ip in ips)
        {
            var stateList = MonitorState.CreateRandomListOfMonitorStates(ip);
            ip.MonitorStateList = stateList;
            states.AddRange(stateList);
        };

        var cut = Render<MonitorStatesHistory>(parameters => parameters
            .Add(p => p.MonitorStates, states)
            .Add(p => p.MonitorHistoryVisible, visible)
        );

        return (cut, monitoringApi);
    }

    [Fact]
    public void RendersMonitorStatesHistory_WhenVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.Contains("monitor_history", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderMonitorStatesHistory_WhenNotVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, false);
        Assert.DoesNotContain("monitor_history", cut.Markup);
    }

    [Fact]
    public void RendersCorrectNumberOfMonitorStates()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        var pings = cut.FindAll("tr#pingstate").Count;
        var expectedPings = cut.Instance.MonitorStates.Where(x => null != x.PingState).Count();

        var ports = cut.FindAll("tr#portstate").Count;
        var expectedPorts = cut.Instance.MonitorStates.Sum(x => x.PortState?.Count ?? 0);

        Assert.Equal(ports, expectedPorts);
        Assert.Equal(pings, expectedPings);
    }

    [Fact]
    public void OnClickViewIp_EventCallbackCalled()
    {
        var (cut, api) = CreateStandardComponent(Services, true);
        bool invoked = false;

        cut.Render(parameters => parameters.Add(p => p.ViewClicked, EventCallback.Factory.Create<IP>(this, ip =>
        {
            invoked = true;
        })));

        cut.Find("button#viewbutton").Click();
        Assert.True(invoked);
    }

    [Fact]
    public void OnClickViewIp_HistoryHidden()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.True(cut.Instance.MonitorHistoryVisible);
        cut.Find("button#viewbutton").Click();
        Assert.False(cut.Instance.MonitorHistoryVisible);
    }
}