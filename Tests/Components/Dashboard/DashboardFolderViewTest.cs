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

public class DashboardFolderViewTest : BunitContext
{
    public (IRenderedComponent<DashboardFolderView>, Mock<IDashAPI>, Mock<IJSRuntime>) CreateFolderComponent(BunitServiceProvider services)
    {
        var shortcut1 = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut 1",
            Description = "Shortcut Description 1",
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

        var cut = Render<DashboardFolderView>(parameters => parameters
            .Add(p => p.Item, folder)
            .Add(p => p.Hidden, false)
        );

        return (cut, dashApi, jsRuntimeMock);
    }

    [Fact]
    public void RendersFolderView_WhenNotHidden()
    {
        var (cut, api, js) = CreateFolderComponent(this.Services);

        Assert.Contains("dashboard_folder_view", cut.Markup);
        Assert.Contains("Test Shortcut 1", cut.Markup);
        Assert.Contains("Test Shortcut 2", cut.Markup);
    }

    [Fact]
    public void ClickDelete_DeletesItemAndChildren()
    {
        var (cut, api, js) = CreateFolderComponent(this.Services);

        var item = cut.Instance.Item as DirectoryItem;

        cut.Find("button#deletebutton").Click();
        api.Verify(a => a.DeleteItemAsync(item), Times.Once);

        foreach (var child in item.Children)
        {
            api.Verify(a => a.DeleteItemAsync(child), Times.Once);
        }
    }

    [Fact]
    public void ClickCancel_HidesFolderView()
    {
        var (cut, api, js) = CreateFolderComponent(this.Services);

        string test = "dashboard_folder_view";

        Assert.Contains(test, cut.Markup);
        cut.Find("button#cancelbutton").Click();
        Assert.DoesNotContain(test, cut.Markup);
    }

    [Fact]
    public void ClickChildCard_InvokesJsRuntime()
    {
        var (cut, api, js) = CreateFolderComponent(this.Services);
        var item = cut.Instance.Item as DirectoryItem;
        var child = item.Children[0] as ShortcutItem;
        cut.FindAll("div#dashcard")[0].Click();
        js.Verify(j => j.InvokeAsync<object?>("open", It.Is<object?[]>(o => o[0]!.ToString() == child.Url)), Times.Once);
    }
}