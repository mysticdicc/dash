using Bunit;
using Xunit;
using dash.Components.Subnets;
using danklibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;

public class SubnetCardTest : TestContext
{
    [Fact]
    public void RendersSubnetCard_Basic()
    {
        var subnet = new Subnet("192.168.0.0/24");

        Services.AddSingleton(Mock.Of<ISubnetsAPI>());

        var cut = RenderComponent<SubnetCard>(parameters => parameters
            .Add(p => p.Subnet, subnet)
        );

        Assert.Contains("subnet_card", cut.Markup);
    }

    [Fact]
    public void ExpandButton_ShowsIpList()
    {
        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "iphost";

        Services.AddSingleton(Mock.Of<ISubnetsAPI>());

        var cut = RenderComponent<SubnetCard>(parameters => parameters
            .Add(p => p.Subnet, subnet)
        );

        cut.Find("#iplistbtn").Click();
        Assert.Contains("ip_layout", cut.Markup);
        Assert.Contains("iphost", cut.Markup);

        cut.Find("#iplistbtn").Click();
        Assert.DoesNotContain("ip_layout", cut.Markup);
        Assert.DoesNotContain("iphost", cut.Markup);

        Assert.Contains("subnet_card", cut.Markup);
    }
}