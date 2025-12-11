using Bunit;
using DashComponents.Monitoring;
using DashLib.Interfaces;
using DashLib.Monitoring;
using DashLib.Network;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class MonitorStatesHistoryTest : TestContext
{
    public (IRenderedComponent<MonitorStatesHistory>, Mock<IMonitoringAPI>) CreateStandardComponent(TestServiceProvider services, bool visible)
    {
        var monitoringApi = new Mock<IMonitoringAPI>();
        services.AddSingleton(monitoringApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ips = subnet.List;

        List<MonitorState> states = [];

        foreach (var ip in ips)
        {
            var stateList = CreateRandomListOfMonitorStates(ip);
            ip.MonitorStateList = stateList;
            states.AddRange(stateList);
        };

        var cut = RenderComponent<MonitorStatesHistory>(parameters => parameters
            .Add(p => p.MonitorStates, states)
            .Add(p => p.MonitorHistoryVisible, visible)
        );

        return (cut, monitoringApi);
    }

    public List<MonitorState> CreateRandomListOfMonitorStates(IP ip)
    {
        int count = new Random().Next(0, 20);
        var list = new List<MonitorState>();

        for (int i = 0; i < count; i++)
        {
            var random = new Random().Next(0, 100);

            var monitorState = new MonitorState
            {
                ID = i,
                IP_ID = ip.ID,
                SubmitTime = DateTime.Now.AddMinutes(-i * 5),
                PortState = new List<PortState>(),
                IP = ip
            };

            if (random >= 0 && random < 50)
            {
                monitorState.PingState = new PingState
                {
                    ID = i,
                    MonitorID = i,
                    MonitorState = monitorState,
                    Response = true
                };

                if (random >= 0 && random < 25)
                {
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i,
                        MonitorID = i,
                        Port = 80,
                        Status = true,
                        MonitorState = monitorState
                    });
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i + 1,
                        MonitorID = i,
                        Port = 443,
                        Status = true,
                        MonitorState = monitorState
                    });
                }
                else if (random >= 25 && random < 50)
                {
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i,
                        MonitorID = i,
                        Port = 80,
                        Status = false,
                        MonitorState = monitorState
                    });
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i + 1,
                        MonitorID = i,
                        Port = 443,
                        Status = false,
                        MonitorState = monitorState
                    });
                }
            }
            else if (random > 50 && random <= 100)
            {
                monitorState.PingState = new PingState
                {
                    ID = i,
                    MonitorID = i,
                    Response = false
                };
            }

            list.Add(monitorState);
        }

        ip.MonitorStateList = list;
        return list;
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
}