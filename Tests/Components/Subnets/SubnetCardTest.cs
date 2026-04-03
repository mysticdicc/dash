using DashComponents.Subnets;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using DashLib.Models.Network;
using Microsoft.JSInterop;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SubnetCardTest : BunitContext
{
    public (IRenderedComponent<SubnetCard>, Mock<IMonitorTargetAPI>) CreateStandardComponent(BunitServiceProvider services)
    {
        var subnetApi = new Mock<IMonitorTargetAPI>();

        var subnet = new SubnetContainer("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "iphost";

        subnetApi.Setup(x => x.RunDiscoveryTaskAsync(It.IsAny<SubnetContainer>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.AddSubnetByObjectAsync(It.IsAny<SubnetContainer>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.UpdateSubnetByObjectAsync(It.IsAny<SubnetContainer>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.GetAllAsync()).ReturnsAsync(() => new List<SubnetContainer>());
        subnetApi.Setup(x => x.DeleteSubnetByObjectAsync(It.IsAny<SubnetContainer>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.EditIpAsync(It.IsAny<IpMonitoringTarget>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.DeleteSubnetAsync(It.IsAny<int>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.DiscoveryUpdateAsync(It.IsAny<SubnetContainer>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.GetSubnetByIdAsync(It.IsAny<int>())).ReturnsAsync((SubnetContainer)null!);
        subnetApi.Setup(x => x.DeleteIpByObjectAsync(It.IsAny<IpMonitoringTarget>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SubnetContainer> { subnet });
        subnetApi.Setup(x => x.GetSubnetByIdAsync(It.IsAny<int>())).ReturnsAsync(subnet);

        services.AddSingleton(subnetApi.Object);

        var mockJs = new Mock<IJSRuntime>();
        services.AddSingleton(mockJs.Object);

        var cut = Render<SubnetCard>(parameters => parameters
            .Add(p => p.Subnet, subnet)
        );

        return (cut, subnetApi);
    }

    [Fact]
    public void RendersSubnetCard_Basic()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains("subnet_card", cut.Item1.Markup);
    }

    [Fact]
    public void ExpandButton_ShowsAndHidesIpList()
    {
        var cut = CreateStandardComponent(this.Services);

        cut.Item1.Find("button#iplistbtn").Click();
        Assert.Contains("ip_layout", cut.Item1.Markup);
        Assert.Contains("iphost", cut.Item1.Markup);

        cut.Item1.Find("button#iplistbtn").Click();
        Assert.DoesNotContain("ip_layout", cut.Item1.Markup);
        Assert.DoesNotContain("iphost", cut.Item1.Markup);
    }

    [Fact]
    public void DeleteButton_CallsDelete()
    {
        var cut = CreateStandardComponent(this.Services);

        cut.Item1.Find("button#deletebutton").Click();
        cut.Item2.Verify(x => x.DeleteSubnetByObjectAsync(It.IsAny<SubnetContainer>()), Times.Once);

    }

    [Fact]
    public void DiscoveryButton_StartsDiscovery()
    {
        var cut = CreateStandardComponent(this.Services);

        cut.Item1.Find("button#discoverbutton").Click();
        cut.Item2.Verify(x => x.RunDiscoveryTaskAsync(It.IsAny<SubnetContainer>()), Times.Once);
    }
}