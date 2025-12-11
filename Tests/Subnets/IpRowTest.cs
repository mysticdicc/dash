using Bunit;
using Xunit;
using DashComponents.Subnets;
using DashLib.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using DashLib.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class IpRowTest : TestContext
{
    public (IRenderedComponent<IpRow>, Mock<ISubnetsAPI>) CreateStandardComponent(TestServiceProvider services)
    {
        var subnetApi = new Mock<ISubnetsAPI>();
        subnetApi.Setup(x => x.DeleteSubnetByObjectAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.RunDiscoveryTaskAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        services.AddSingleton(subnetApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "iphost";

        var cut = RenderComponent<IpRow>(parameters => parameters
            .Add(p => p.IpAddress, subnet.List[0])
        );

        return (cut, subnetApi);
    }

    [Fact]
    public void RendersIpRow_Basic()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains("ip_row", cut.Item1.Markup);
        Assert.Contains("iphost", cut.Item1.Markup);
    }
}