using Bunit;
using Xunit;
using dash.Components.Subnets;
using danklibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class IpRowTest : TestContext
{
    [Fact]
    public void RendersIpRow_Basic()
    {
        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "host1";

        Services.AddSingleton(Mock.Of<ISubnetsAPI>());

        var cut = RenderComponent<IpRow>(parameters => parameters
            .Add(p => p.IpAddress, ip)
        );

        Assert.Contains("ip_row", cut.Markup);
        Assert.Contains("host1", cut.Markup);
    }

    [Fact]
    public void DeleteButton_InvokesDelete()
    {
        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];

        var apiMock = new Mock<ISubnetsAPI>();
        Services.AddSingleton(apiMock.Object);

        bool deleted = false;
        var cut = RenderComponent<IpRow>(parameters => parameters
            .Add(p => p.IpAddress, ip)
            .Add(p => p.OnDeleteIp, EventCallback.Factory.Create<bool>(this, _ => { deleted = true; return Task.CompletedTask; }))
        );

        cut.Find("button").Click();
        Assert.True(deleted);
    }
}