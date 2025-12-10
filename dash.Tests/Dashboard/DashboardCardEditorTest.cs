using Bunit;
using Xunit;
using dash.Components.Dashboard;
using danklibrary.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;
using System;

public class DashboardCardEditorTest : TestContext
{
    [Fact]
    public void RendersEditor_WhenNotHidden()
    {
        var shortcut = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut",
            Description = "Shortcut Description",
            Url = "https://example.com"
        };

        Services.AddSingleton(Mock.Of<IDashAPI>());

        var cut = RenderComponent<DashboardCardEditor>(parameters => parameters
            .Add(p => p.Hidden, false)
            .Add(p => p.Item, shortcut)
        );

        Assert.Contains("dashboard_card_editor", cut.Markup);
        Assert.Contains(@$"value=""{shortcut.Url}""", cut.Markup);
        Assert.Contains(@$"value=""{shortcut.DisplayName}""", cut.Markup);
        Assert.Contains(shortcut.Id.ToString(), cut.Markup);

    }

    [Fact]
    public void SaveButton_CallsSave()
    {
        var shortcut = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut",
            Description = "Shortcut Description",
            Url = "https://example.com"
        };

        var apiMock = new Mock<IDashAPI>();
        apiMock.Setup(x => x.SaveItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        Services.AddSingleton(apiMock.Object);

        var cut = RenderComponent<DashboardCardEditor>(parameters => parameters
            .Add(p => p.Hidden, false)
            .Add(p => p.Item, shortcut)
            .Add(p => p.IsNewItem, true)
        );

        cut.Find("button").Click();
        apiMock.Verify(x => x.SaveItemAsync(It.IsAny<DashboardItemBase>()), Times.Once);
    }
}