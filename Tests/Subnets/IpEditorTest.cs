using Bunit;
using DashLib.Interfaces;
using DashLib.Network;
using DashComponents.Subnets;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class IpEditorTest : TestContext
{
    public (IRenderedComponent<IpEditor>, Mock<ISubnetsAPI>) CreateStandardComponent(TestServiceProvider services)
    {
        var subnetApi = new Mock<ISubnetsAPI>();
        subnetApi.Setup(x => x.DeleteSubnetByObjectAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        subnetApi.Setup(x => x.RunDiscoveryTaskAsync(It.IsAny<Subnet>())).ReturnsAsync(true);
        services.AddSingleton(subnetApi.Object);

        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "iphost";

        var cut = RenderComponent<IpEditor>(parameters => parameters
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
}