using Bunit;
using DashComponents.Subnets;
using DashLib.Interfaces;
using DashLib.Network;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Xunit;

public class IpEditorTest : BunitContext
{
    public (IRenderedComponent<IpEditor>, Mock<ISubnetsAPI>) CreateStandardComponent(BunitServiceProvider services)
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
        cut.Item2.Verify(x => x.EditIpAsync(It.IsAny<IP>()), Times.Once);

    }
}