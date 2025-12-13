using Bunit;
using DashLib.Interfaces;
using DashLib.Network;
using DashComponents.Subnets;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Web;

public class SubnetEditorTest : TestContext
{
    public (IRenderedComponent<SubnetEditor>, Mock<ISubnetsAPI>) CreateStandardComponent(TestServiceProvider services)
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

        var cut = RenderComponent<SubnetEditor>(parameters => parameters
            .Add(p => p.Subnet, subnet)
            .Add(p => p.EditorVisible, true)
        );

        return (cut, subnetApi);
    }

    [Fact]
    public void RendersSubnetEditor_WhenVisible()
    {
        var cut = CreateStandardComponent(this.Services);

        Assert.Contains("subnet_editor", cut.Item1.Markup);
    }

    [Fact]
    public void OnClickCancel_HidesEditor()
    {
        var (cut, _) = CreateStandardComponent(this.Services);

        Assert.Contains("subnet_editor", cut.Markup);

        cut.Find("button#cancelbutton").Click();

        Assert.DoesNotContain("subnet_editor", cut.Markup);
    }

    [Fact]
    public void OnKeyDownCidrInput_Enter_GeneratesSubnet()
    {
        var (cut, _) = CreateStandardComponent(this.Services);

        var cidrInput = cut.Find("input#cidrinput");
        cidrInput.Input("10.0.0.0/24");
        cidrInput.KeyDown(new KeyboardEventArgs { Code = "Enter" });

        var expandButton = cut.Find("button#iplist");
        expandButton.Click();

        Assert.Contains("10.0.0.0", cut.Markup);
        Assert.Contains("10.0.0.1", cut.Markup);
    }

    [Fact]
    public void OnClickCidrGenerate_UpdatesSubnet()
    {
        var (cut, _) = CreateStandardComponent(this.Services);

        var cidrInput = cut.Find("input#cidrinput");
        var generateButton = cut.Find("button#generatebutton");

        cidrInput.Input("172.16.0.0/16");
        generateButton.Click();

        var expandButton = cut.Find("button#iplist");
        expandButton.Click();

        Assert.Contains("172.16.0.0", cut.Markup);
        Assert.Contains("172.16.0.1", cut.Markup);
    }

    [Fact]
    public void OnClickIpListExpand_TogglesIpList()
    {
        var (cut, _) = CreateStandardComponent(this.Services);
        var expandButton = cut.Find("button#iplist");

        Assert.DoesNotContain("iphost", cut.Markup);
        expandButton.Click();
        Assert.Contains("iphost", cut.Markup);
    }

    [Fact]
    public void OnClickDeleteIp_ApiDeletesObject()
    {
        var (cut, apiMock) = CreateStandardComponent(this.Services);
        var subnet = apiMock.Object.GetSubnetByIdAsync(It.IsAny<int>()).Result;
        var ip = subnet.List[0];
        var expandButton = cut.Find("button#iplist");
        expandButton.Click();
        var deleteButton = cut.FindAll("button#deleteipbutton")[0];
        deleteButton.Click();

        apiMock.Verify(x => x.DeleteIpByObjectAsync(ip), Times.Once);
    }
}