using Bunit;
using DashComponents.Subnets;
using DashLib.Interfaces;
using DashLib.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Xunit;

public class IpViewTest : TestContext
{
    public (IRenderedComponent<IpView>, Mock<ISubnetsAPI>) CreateStandardComponent(TestServiceProvider services)
    {
        var subnetApi = new Mock<ISubnetsAPI>();

        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "iphost";

        subnetApi.Setup(x => x.RunDiscoveryTaskAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.AddSubnetByObjectAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.UpdateSubnetByObjectAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.GetAllAsync()).ReturnsAsync(() => new List<Subnet>());
        subnetApi.Setup(x => x.DeleteSubnetByObjectAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.EditIpAsync(It.IsAny<IP>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.DeleteSubnetAsync(It.IsAny<int>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.DiscoveryUpdateAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.GetSubnetByIdAsync(It.IsAny<int>())).ReturnsAsync((Subnet)null!);
        subnetApi.Setup(x => x.DeleteIpByObjectAsync(It.IsAny<IP>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Subnet> { subnet });
        subnetApi.Setup(x => x.GetSubnetByIdAsync(It.IsAny<int>())).ReturnsAsync(subnet);

        services.AddSingleton(subnetApi.Object);

        var cut = RenderComponent<IpView>(parameters => parameters
            .Add(p => p.IpAddress, ip)
            .Add(p => p.IpViewVisible, true)
        );

        return (cut, subnetApi);
    }

    [Fact]
    public void RendersIpView_WhenVisible()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains("ip_view", cut.Item1.Markup);
        Assert.Contains("iphost", cut.Item1.Markup);
    }

    [Fact]
    public void ClickClose_HidesIpView()
    {
        var (cut, _) = CreateStandardComponent(this.Services);
        Assert.Contains("ip_view", cut.Markup);
        cut.Find("button#closebutton").Click();
        Assert.DoesNotContain("ip_view", cut.Markup);
    }

    [Fact]
    public void ClickEdit_HidesIpView()
    {
        var (cut, _) = CreateStandardComponent(this.Services);
        Assert.Contains("ip_view", cut.Markup);
        cut.Find("button#editbutton").Click();
        Assert.DoesNotContain("ip_view", cut.Markup);
    }
}