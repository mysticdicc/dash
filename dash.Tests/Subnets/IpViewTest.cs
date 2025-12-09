using Bunit;
using Xunit;
using dash.Components.Subnets;
using danklibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;

public class IpViewTest : TestContext
{
    [Fact]
    public void RendersIpView_WhenVisible()
    {
        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "host1";

        var apiMock = new Mock<ISubnetsAPI>();
        apiMock.Setup(x => x.GetSubnetByIdAsync(It.IsAny<int>())).ReturnsAsync(subnet);
        Services.AddSingleton(apiMock.Object);

        var cut = RenderComponent<IpView>(parameters => parameters
            .Add(p => p.IpAddress, ip)
            .Add(p => p.IpViewVisible, true)
        );

        Assert.Contains("ip_view", cut.Markup);
        Assert.Contains("host1", cut.Markup);
    }
}