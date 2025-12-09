using Bunit;
using Xunit;
using dash.Components.Dashboard;
using danklibrary.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using danklibrary.Interfaces;
using System;

public class DashboardFolderViewTest : TestContext
{
    [Fact]
    public void RendersFolderView_WhenNotHidden()
    {
        var directory = new DirectoryItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Folder",
            Description = "Folder Description",
            Children = [new ShortcutItem
            {
                Id = Guid.NewGuid(),
                DisplayName = "Child Shortcut",
                Description = "Child Description",
                Url = "https://child.com"
            }]
        };

        Services.AddSingleton(Mock.Of<IDashAPI>());

        var cut = RenderComponent<DashboardFolderView>(parameters => parameters
            .Add(p => p.Item, directory)
            .Add(p => p.Hidden, false)
        );

        Assert.Contains("dashboard_folder_view", cut.Markup);
        Assert.Contains("Child Shortcut", cut.Markup);
    }
}