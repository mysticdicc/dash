using Bunit;
using DashComponents.Subnets;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using DashLib.Models.Network;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Xunit;

public class IpEditorTest : BunitContext
{
    public (IRenderedComponent<IpEditor>, Mock<IMonitorTargetAPI>) CreateStandardComponent(BunitServiceProvider services)
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

        var cut = Render<IpEditor>(parameters => parameters
            .Add(p => p.IpAddress, ip)
            .Add(p => p.EditorVisible, true)
        );

        return (cut, subnetApi);
    }

    [Fact]
    public void RendersEditor_WhenVisible()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains("ip_editor", cut.Item1.Markup);
        Assert.Contains("iphost", cut.Item1.Markup);
    }

    [Fact]
    public void OnClickCancel_HidesEditor()
    {
        var (cut, _) = CreateStandardComponent(this.Services);

        Assert.Contains("ip_editor", cut.Markup);

        cut.Find("button#cancelbutton").Click();

        Assert.DoesNotContain("ip_editor", cut.Markup);
    }

    [Fact]
    public void EditForm_UpdatesIpAddressHostname()
    {
        var cut = CreateStandardComponent(this.Services);
        var input = cut.Item1.Find("input#hostnameinput");
        input.Change("newhostname");
        Assert.Equal("newhostname", cut.Item1.Instance.IpAddress.Hostname);
    }

    [Fact]
    public void SaveButton_CallsSave()
    {
        var cut = CreateStandardComponent(this.Services);

        cut.Item1.Find("button#savebutton").Click();
        cut.Item2.Verify(x => x.EditIpAsync(It.IsAny<IpMonitoringTarget>()), Times.Once);

    }
}