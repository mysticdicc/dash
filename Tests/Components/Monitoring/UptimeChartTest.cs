using ApexCharts;
using Bunit;
using DashComponents.Monitoring;
using DashLib.Interfaces;
using DashLib.Monitoring;
using DashLib.Network;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class UptimeChartTest : TestContext
{
    public (IRenderedComponent<UptimeChart>, Mock<IMonitoringAPI>) CreateStandardComponent(TestServiceProvider services, bool visible)
    {
        var monitoringApi = new Mock<IMonitoringAPI>();
        services.AddSingleton(monitoringApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ips = subnet.List;

        foreach (var ip in ips)
        {
            ip.MonitorStateList = CreateRandomListOfMonitorStates();
        };

        var cut = RenderComponent<UptimeChart>(parameters => parameters
            .Add(p => p.IPAddresses, ips)
            .Add(p => p.Visible, visible)
        );

        return (cut, monitoringApi);
    }

    public List<MonitorState> CreateRandomListOfMonitorStates()
    {
        int count = new Random().Next(0, 20);
        var list = new List<MonitorState>();

        for (int i = 0; i < count; i++)
        {
            var random = new Random().Next(0, 100);

            var monitorState = new MonitorState
            {
                ID = i,
                IP_ID = i,
                SubmitTime = DateTime.Now.AddMinutes(-i * 5),
                PortState = new List<PortState>()
            };

            if (random >= 0 && random < 50)
            {
                monitorState.PingState = new PingState
                {
                    ID = i,
                    MonitorID = i,
                    Response = true
                };

                if (random >= 0 && random < 25)
                {
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i,
                        MonitorID = i,
                        Port = 80,
                        Status = true
                    });
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i + 1,
                        MonitorID = i,
                        Port = 443,
                        Status = true
                    });
                }
                else if (random >= 25 && random < 50)
                {
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i,
                        MonitorID = i,
                        Port = 80,
                        Status = false
                    });
                    monitorState.PortState.Add(new PortState
                    {
                        ID = i + 1,
                        MonitorID = i,
                        Port = 443,
                        Status = false
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

        return list;
    }

    [Fact]
    public void RendersUptimeChart_WhenVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, true);

        Assert.Contains("uptime_chart", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderUptimeChart_WhenNotVisible()
    {
        var (cut, api) = CreateStandardComponent(Services, false);
        Assert.DoesNotContain("uptime_chart", cut.Markup);
    }
}