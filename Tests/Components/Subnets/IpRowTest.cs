using Bunit;
using DashComponents.Subnets;
using DashLib.Interfaces;
using DashLib.Network;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class IpRowTest : BunitContext
{
    public (IRenderedComponent<IpRow>, Mock<ISubnetsAPI>) CreateStandardComponent(BunitServiceProvider services)
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
        Assert.Contains($"<td>{IP.ConvertToString(cut.Item1.Instance.IpAddress.Address)}</td>", cut.Item1.Markup);
    }

    [Fact]
    public void DeleteButton_CallsDelete()
    {
        var cut = CreateStandardComponent(this.Services);

        cut.Item1.Find("button#deletebutton").Click();
        cut.Item2.Verify(x => x.DeleteIpByObjectAsync(It.IsAny<IP>()), Times.Once);

    }
}