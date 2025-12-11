using Bunit;
using Xunit;
using DashComponents.Dashboard;
using DashLib.Dashboard;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Moq;
using DashLib.Interfaces;
using System.Collections.Generic;

public class DashboardCardTest : TestContext
{
    public (IRenderedComponent<DashboardCard>, Mock<IDashAPI>, Mock<IJSRuntime>) CreateStandardComponent(TestServiceProvider services)
    {
        var shortcut = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut",
            Description = "Shortcut Description",
            Url = "https://example.com"
        };

        var shortcut1 = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut 1",
            Description = "Shortcut Description 1",
            Url = "https://example.com/1"
        };

        var folder = new DirectoryItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Folder",
            Description = "Folder Description",
            Children = [shortcut1]
        };

        var dashApi = new Mock<IDashAPI>();
        dashApi.Setup(api => api.GetAllItemsAsync()).ReturnsAsync(new List<DashboardItemBase> { shortcut, folder });
        dashApi.Setup(api => api.SaveItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        dashApi.Setup(api => api.DeleteItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        dashApi.Setup(api => api.EditItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        services.AddSingleton(dashApi.Object);

        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock
            .Setup(js => js.InvokeAsync<object?>("open", It.IsAny<object?[]>()))
            .Returns(new ValueTask<object?>(result: null));
        services.AddSingleton<IJSRuntime>(jsRuntimeMock.Object);

        var cut = RenderComponent<DashboardCard>(parameters => parameters
            .Add(p => p.Item, shortcut)
        );

        return (cut, dashApi, jsRuntimeMock);

    }

    public (IRenderedComponent<DashboardCard>, Mock<IDashAPI>) CreateFolderComponent(TestServiceProvider services)
    {
        var shortcut = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut",
            Description = "Shortcut Description",
            Url = "https://example.com"
        };

        var folder = new DirectoryItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Folder",
            Description = "Folder Description",
            Children = [shortcut]
        };

        var dashApi = new Mock<IDashAPI>();
        dashApi.Setup(api => api.GetAllItemsAsync()).ReturnsAsync(new List<DashboardItemBase> { folder });
        dashApi.Setup(api => api.SaveItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        dashApi.Setup(api => api.DeleteItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        dashApi.Setup(api => api.EditItemAsync(It.IsAny<DashboardItemBase>())).ReturnsAsync(true);
        services.AddSingleton(dashApi.Object);

        var cut = RenderComponent<DashboardCard>(parameters => parameters
            .Add(p => p.Item, folder)
        );

        return (cut, dashApi);
    }

    [Fact]
    public void RendersShortcutItem_Correctly()
    {
        var (cut, api, js) = CreateStandardComponent(Services);
        var item = cut.Instance.Item as ShortcutItem;

        Assert.Contains("dashboard_card", cut.Markup);
        Assert.Contains($"<strong>{item!.DisplayName}</strong>", cut.Markup);
        Assert.Contains($"<span>{item!.Url}</span>", cut.Markup);
        Assert.Contains($"<td>{item!.Description}</td>", cut.Markup);
    }

    [Fact]
    public void RendersFolderItem_Correctly()
    {
        var (cut, api) = CreateFolderComponent(Services);
        var item = cut.Instance.Item as DirectoryItem;

        Assert.Contains("dashboard_card", cut.Markup);
        Assert.Contains($"<strong>{item!.DisplayName}</strong>", cut.Markup);
        Assert.Contains($"<td>{item!.Description}</td>", cut.Markup);
    }

    [Fact]
    public void ClickCard_OpensFolderView()
    {
        var (cut, api) = CreateFolderComponent(Services);
        string folderSelector = "div#dashfolderview";

        var item = cut.Instance.Item as DirectoryItem;

        Assert.Empty(cut.FindAll(folderSelector));
        cut.Find("div#dashcard").Click();
        Assert.Single(cut.FindAll(folderSelector));
        Assert.Contains(item!.Children[0].DisplayName, cut.Markup);
    }

    [Fact]
    public void EditButton_InvokesCallback()
    {
        var shortcut = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut",
            Description = "Shortcut Description",
            Url = "https://example.com"
        };

        bool called = false;
        var cut = RenderComponent<DashboardCard>(parameters => parameters
            .Add(p => p.OnEditClick, EventCallback.Factory.Create<DashboardItemBase>(this, (DashboardItemBase _) => { called = true; return Task.CompletedTask; }))
            .Add(p => p.Item, shortcut)
        );

        cut.Find("button").Click();
        Assert.True(called);
    }

    [Fact]
    public void ClickShortcut_InvokesJsRuntime()
    {
        var (cut, api, jsRuntime) = CreateStandardComponent(Services);
        cut.Find("div#dashcard").Click();

        var item = cut.Instance.Item as ShortcutItem;

        jsRuntime.Verify(
            js => js.InvokeAsync<object?>(
                "open",
                It.Is<object?[]>(args =>
                    args.Length == 2 &&
                    Equals(args[0], item!.Url) &&
                    Equals(args[1], "_blank"))),
            Times.Once);
    }
}