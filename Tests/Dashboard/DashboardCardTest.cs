using Bunit;
using Xunit;
using DashComponents.Dashboard;
using DashLib.Dashboard;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class DashboardCardTest : TestContext
{
    [Fact]
    public void RendersShortcutItem_Correctly()
    {
        var shortcut = new ShortcutItem
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Shortcut",
            Description = "Shortcut Description",
            Url = "https://example.com",
            Icon = null
        };

        var cut = RenderComponent<DashboardCard>(parameters => parameters
            .Add(p => p.Item, shortcut)
        );

        var expected = @$"<div class=""dashboard_card"" >
                              <article>
                                <header>
                                  <span class=""material-icons"">link</span>
                                  <span>
                                    <strong>{shortcut.DisplayName}</strong>
                                  </span>
                                  <button  >
                                    <span class=""material-icons"">edit</span>
                                  </button>
                                </header>
                                <content>
                                  <table>
                                    <tbody>
                                      <tr>
                                        <td>
                                          <span>{shortcut.Url}</span>
                                        </td>
                                      </tr>
                                      <tr>
                                        <td>{shortcut.Description}</td>
                                      </tr>
                                    </tbody>
                                  </table>
                                </content>
                              </article>
                            </div>";

        cut.MarkupMatches(expected);
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
}