using Bunit;
using Xunit;
using dash.Components.Subnets;
using danklibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;

public class IpEditorTest : TestContext
{
    [Fact]
    public void RendersEditor_WhenVisible()
    {
        var subnet = new Subnet("192.168.0.0/24");
        var ip = subnet.List[0];
        ip.Hostname = "host1";  
        Services.AddSingleton(Mock.Of<ISubnetsAPI>());

        var cut = RenderComponent<IpEditor>(parameters => parameters
            .Add(p => p.IpAddress, ip)  
            .Add(p => p.EditorVisible, true)
        );

        Assert.Contains("ip_editor", cut.Markup);
        Assert.Contains("host1", cut.Markup);
    }
}