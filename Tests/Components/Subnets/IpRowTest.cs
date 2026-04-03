using Bunit;
using DashComponents.Subnets;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using DashLib.Models.Network;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class IpRowTest : BunitContext
{
    public (IRenderedComponent<IpRow>, Mock<IMonitorTargetAPI>) CreateStandardComponent(BunitServiceProvider services)
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

        var cut = Render<IpRow>(parameters => parameters
            .Add(p => p.IpAddress, subnet.List[0])
        );

        return (cut, subnetApi);
    }

    [Fact]
    public void RendersIpRow_Basic()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains("ip_row", cut.Item1.Markup);
    }

    [Fact]
    public void RendersIpInfo()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains($"<td>{cut.Item1.Instance.IpAddress.Hostname}</td>", cut.Item1.Markup);
        Assert.Contains($"<td>{IpMonitoringTarget.ConvertToString(cut.Item1.Instance.IpAddress.Address)}</td>", cut.Item1.Markup);
    }

    [Fact]
    public void DeleteButton_CallsDelete()
    {
        var cut = CreateStandardComponent(this.Services);

        cut.Item1.Find("button#deletebutton").Click();
        cut.Item2.Verify(x => x.DeleteIpByObjectAsync(It.IsAny<IpMonitoringTarget>()), Times.Once);

    }
}