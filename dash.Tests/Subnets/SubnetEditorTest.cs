using Bunit;
using Xunit;
using dash.Components.Subnets;
using danklibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;

public class SubnetEditorTest : TestContext
{
    [Fact]
    public void RendersSubnetEditor_WhenVisible()
    {
        var subnet = new Subnet("192.168.0.0/24");
        Services.AddSingleton(Mock.Of<ISubnetsAPI>());

        var cut = RenderComponent<SubnetEditor>(parameters => parameters
            .Add(p => p.EditorVisible, true)
            .Add(p => p.IsNewSubnet, true)
            .Add(p => p.Subnet, subnet)
        );

        Assert.Contains("subnet_editor", cut.Markup);
    }
}