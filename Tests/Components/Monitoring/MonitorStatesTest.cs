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

public class MonitorStatesTest : TestContext
{
    public (IRenderedComponent<MonitorStates>, Mock<IMonitoringAPI>) CreateStandardComponent(TestServiceProvider services, bool visible)
    {
        var monitoringApi = new Mock<IMonitoringAPI>();
        services.AddSingleton(monitoringApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ips = subnet.List;

        foreach (var ip in ips)
        {
            var states = MonitorState.CreateRandomListOfMonitorStates(ip);
            ip.MonitorStateList = states;

            if (states != null && states.Count > 0)
            {
                ip.IsMonitoredICMP = true;
            }
        };

        var cut = RenderComponent<MonitorStates>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, visible)
        );

        return (cut, monitoringApi);
    }

    [Fact]
    public void RendersMonitorStates_WhenVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.Contains("monitor_states", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderMonitorStates_WhenNotVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, false);
        Assert.DoesNotContain("monitor_states", cut.Markup);
    }

    [Fact]
    public void RendersCorrectNumberOfRows()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        var count = cut.FindAll("tr#ip").Count;
        var expectedCount = cut.Instance._lastPolls.Count;

        Assert.Equal(count, expectedCount);
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
    public void ClickView_CallsEventCallback()
    {
        var (cut, api) = CreateStandardComponent(Services, true);
        var callbackInvoked = false;
        IP? callbackIp = null;

        cut.SetParametersAndRender(parameters => parameters.Add(p => p.ViewClicked, EventCallback.Factory.Create<IP>(this, ip =>
        {
            callbackInvoked = true;
            callbackIp = ip;
        })));

        var expectedIp = cut.Instance._lastPolls[0].IP;

        cut.FindAll("tr#ip")[0].Click();

        Assert.True(callbackInvoked);
        Assert.Equal(expectedIp, callbackIp);
    }

    [Fact]
    public void ClickEdit_CallsEventCallback()
    {
        var (cut, api) = CreateStandardComponent(Services, true);
        var callbackInvoked = false;
        IP? callbackIp = null;

        cut.SetParametersAndRender(parameters => parameters.Add(p => p.EditClicked, EventCallback.Factory.Create<IP>(this, ip =>
        {
            callbackInvoked = true;
            callbackIp = ip;
        })));

        var expectedIp = cut.Instance._lastPolls[0].IP;
        cut.FindAll("button#editbutton")[0].Click();

        Assert.True(callbackInvoked);
        Assert.Equal(expectedIp, callbackIp);
    }

    [Fact]
    public void ClickHistory_CallsEventCallback()
    {
        var (cut, api) = CreateStandardComponent(Services, true);
        var callbackInvoked = false;
        IP? callbackIp = null;

        cut.SetParametersAndRender(parameters => parameters.Add(p => p.HistoryClicked, EventCallback.Factory.Create<IP>(this, ip =>
        {
            callbackInvoked = true;
            callbackIp = ip;
        })));

        var expectedIp = cut.Instance._lastPolls[0].IP;
        cut.FindAll("button#history")[0].Click();

        Assert.True(callbackInvoked);
        Assert.Equal(expectedIp, callbackIp);
    }   
}