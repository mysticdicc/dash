using Bunit;
using DashComponents.Dashboard;
using DashLib.Dashboard;
using DashLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class DashboardCardEditorTest : TestContext
{
    public (IRenderedComponent<DashboardCardEditor>, Mock<IDashAPI>, Mock<IJSRuntime>) CreateStandardComponent(TestServiceProvider services, bool isNew)
    {
        var shortcut1 = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut 1",
            Description = "Shortcut Description 1",
            Icon = "testiconpath",
            Url = "https://example.com"
        };

        var shortcut2 = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut 2",
            Description = "Shortcut Description 2",
            Url = "https://example.com"
        };

        var folder = new DirectoryItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Folder",
            Description = "Folder Description",
            Children = [shortcut1, shortcut2]
        };

        shortcut1.Parent = folder;
        shortcut2.Parent = folder;

        var dashApi = new Mock<IDashAPI>();
        dashApi.Setup(api => api.GetAllItemsAsync()).ReturnsAsync(new List<DashboardItemBase> { folder });
        dashApi.Setup(api => api.SaveItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        dashApi.Setup(api => api.DeleteItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        dashApi.Setup(api => api.EditItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        services.AddSingleton(dashApi.Object);

        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock
            .Setup(js => js.InvokeAsync<object?>("open", It.IsAny<object?[]>()))
            .Returns(new ValueTask<object?>(result: null));
        services.AddSingleton<IJSRuntime>(jsRuntimeMock.Object);

        var list = new List<DirectoryItem> { folder };

        var cut = RenderComponent<DashboardCardEditor>(parameters => parameters
            .Add(p => p.Hidden, false)
            .Add(p => p.Item, shortcut1)
            .Add(p => p.IsNewItem, isNew)
        );

        return (cut, dashApi, jsRuntimeMock);
    }

    [Fact]
    public void RendersEditor_WhenNotHidden()
    {
        var (cut, api, js) = CreateStandardComponent(this.Services, true);

        var item = cut.Instance.Item as ShortcutItem;

        Assert.Contains("dashboard_card_editor", cut.Markup);
        Assert.Contains(@$"value=""{item!.Url}""", cut.Markup);
        Assert.Contains(@$"value=""{item!.DisplayName}""", cut.Markup);
        Assert.Contains(item!.Id.ToString(), cut.Markup);

    }

    [Fact]
    public void SaveButton_CallsSave()
    {
        var (cut, api, js) = CreateStandardComponent(this.Services, true);

        cut.Find("button#savebutton").Click();
        api.Verify(x => x.SaveItemAsync(It.IsAny<DashboardItemBase>()), Times.Once);
    }

    [Fact]
    public void ClickDeleteIcon_DeletesIcon()
    {
        var (cut, api, js) = CreateStandardComponent(this.Services, true);
        string test = "testiconpath";

        Assert.Contains(test, cut.Markup);
        cut.Find("button#deleteiconbutton").Click();
        Assert.DoesNotContain(test, cut.Markup);
    }

    [Fact]
    public void ClickCancel_HidesEditor()
    {
        var (cut, api, js) = CreateStandardComponent(this.Services, true);
        string test = "dashboard_card_editor";

        Assert.Contains(test, cut.Markup);
        cut.Find("button#cancelbutton").Click();
        Assert.DoesNotContain(test, cut.Markup);
    }

    [Fact]
    public void DeleteItem_CallsDelete()
    {
        var (cut, api, js) = CreateStandardComponent(this.Services, false);

        cut.Find("button#deleteitembutton").Click();
        api.Verify(x => x.DeleteItemAsync(It.IsAny<DashboardItemBase>()), Times.Once);
    }
}