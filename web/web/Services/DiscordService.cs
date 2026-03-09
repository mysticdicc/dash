using DashLib.DankAPI;
using DashLib.Interfaces;
using DashLib.Models.Settings;
using DashLib.Models.Settings.Monitoring;
using DashLib.Models.Network;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using DashLib.Models.Monitoring;

namespace web.Services {
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private TaskCompletionSource _readyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private AllSettings _currentSettings;
        private DiscordSettings _currentDiscordSettings;
        private readonly SettingsAPI _settingsAPI;
        private readonly MonitoringAPI _monitoringAPI;

        public DiscordService(SettingsAPI settingsAPI, MonitoringAPI monitoringAPI)
        {
            _settingsAPI = settingsAPI;
            _monitoringAPI = monitoringAPI;
            _currentSettings = new(true);
            _currentDiscordSettings = _currentSettings.MonitoringSettings.DiscordSettings;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds
            });

            _client.Ready += OnReadyAsync;
            _client.SlashCommandExecuted += OnSlashCommandExecutedAsync;

            _ = StartAsync();
        }

        private async Task OnReadyAsync()
        {
            try
            {
                await RegisterSlashCommandsAsync();
                _readyTcs.TrySetResult();
            }
            catch (Exception ex)
            {
                _readyTcs.TrySetException(ex);
                throw;
            }
        }

        private async Task StartAsync()
        {
            _currentSettings = await _settingsAPI.GetCurrentSettingsAsync();
            _currentDiscordSettings = _currentSettings.MonitoringSettings.DiscordSettings;
            await _client.LoginAsync(TokenType.Bot, _currentDiscordSettings.Token);
            await _client.StartAsync();
        }

        private async Task RegisterSlashCommandsAsync()
        {
            var summaryCommand = new SlashCommandBuilder()
                .WithName("summary")
                .WithDescription("Show current monitoring summary.");

            var onlineCommand = new SlashCommandBuilder()
                .WithName("online")
                .WithDescription("Check if an IP was online on last monitor state.")
                .AddOption("ip", ApplicationCommandOptionType.String, "IPv4 address", isRequired: true);

            var pingCommand = new SlashCommandBuilder()
                .WithName("ping")
                .WithDescription("Check if a device is online with ICMP ping.")
                .AddOption("ip", ApplicationCommandOptionType.String, "IPv4 address", isRequired: true);

            var listCommand = new SlashCommandBuilder()
                .WithName("list")
                .WithDescription("List all monitored devices.");

            await _client.CreateGlobalApplicationCommandAsync(summaryCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(onlineCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(pingCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(listCommand.Build());
        }

        private async Task OnSlashCommandExecutedAsync(SocketSlashCommand command)
        {
            try
            {
                switch (command.CommandName)
                {
                    case "summary":
                        await HandleSummaryAsync(command);
                        break;

                    case "online":
                        await HandleOnlineAsync(command);
                        break;

                    case "ping":
                        await HandlePingAsync(command);
                        break;

                    case "list":
                        await HandleListAsync(command);
                        break;

                    default:
                        await command.FollowupAsync("Unknown command.", ephemeral: true);
                        break;
                }
            }
            catch (Exception ex)
            {
                await command.FollowupAsync($"Command failed: {ex.Message}", ephemeral: true);
                throw;
            }
        }

        public async Task<bool> SendMessageAsync(string message)
        {
            await _readyTcs.Task;

            var channel = _client.GetChannel(_currentDiscordSettings.ChannelID) as IMessageChannel;
            if (channel is null)
                throw new InvalidOperationException($"Discord channel not found or not message-capable: {_currentDiscordSettings.ChannelID}");

            await channel.SendMessageAsync(message);
            return true;
        }

        private async Task HandleListAsync(SocketSlashCommand command)
        {
            await DeferIfNeededAsync(command, ephemeral: true);
            var list = await _monitoringAPI.GetMonitoredIpsAsync();

            if (list.Count == 0)
            {
                await command.FollowupAsync("No monitored devices");
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var ip in list)
                {
                    sb.AppendLine($"Address: {IP.ConvertToString(ip.Address)}");

                    if (null != ip.MonitorStateList)
                    {
                        var last = ip.MonitorStateList
                            .Where(x => x.PingState != null)
                            .OrderByDescending(x => x.SubmitTime)
                            .FirstOrDefault(); 

                        if (null != last)
                        {
                            string status = last.PingState?.Response == true ? "online" : "offline";
                            sb.AppendLine($"Last Online State: {status}");
                            sb.AppendLine($"Last Poll Time: {last.SubmitTime}");
                        }
                        else
                        {
                            sb.AppendLine("Couldnt get last monitor state for device.");
                        }
                    }
                    else
                    {
                        sb.AppendLine("No monitor states for device.");
                    }
                }

                await command.FollowupAsync(sb.ToString());
            }
        }

        private async Task HandlePingAsync(SocketSlashCommand command)
        {
            await DeferIfNeededAsync(command, ephemeral: true);
            var ipText = command.Data.Options.First(o => o.Name == "ip").Value?.ToString();

            if (ipText == null)
            {
                await command.FollowupAsync("IP address is required.", ephemeral: true);
                return;
            }

            try
            {
                var response = await _monitoringAPI.PingDeviceByStringIpAsync(ipText);

                if (null == response)
                {
                    await command.FollowupAsync($"No ping response for IP: {ipText}", ephemeral: true);
                    return;
                }
                else
                {
                    await command.FollowupAsync($"Ping response for IP {ipText}: {(response.IcmpResponse ? "Online" : "Offline")}" 
                        + (string.IsNullOrEmpty(response.Exception) ? "" : $" (Exception: {response.Exception})"), 
                        ephemeral: true);
                    return;
                }
            }
            catch (Exception ex)
            {
                await command.FollowupAsync($"Failed to retrieve monitor state for IP: {ipText}; {ex.Message}", ephemeral: true);
            }
        }

        private async Task HandleSummaryAsync(SocketSlashCommand command)
        {
            await DeferIfNeededAsync(command, ephemeral: true);

            var ips = await _monitoringAPI.GetAllPollsAsync();
            int totalOnline = 0;
            int totalOffline = 0;
            float totalUptime = 0;

            var sb = new StringBuilder();
            sb.AppendLine("Current Monitoring Settings:");
            sb.AppendLine($"Evaluation Period: {_currentSettings.MonitoringSettings.AlertTimePeriodInMinutes} minutes");
            sb.AppendLine($"Alert Threshold: {_currentSettings.MonitoringSettings.AlertIfDownForPercent}%");
            sb.AppendLine($"Alerts Enabled: {_currentSettings.MonitoringSettings.AlertsEnabled}");
            sb.AppendLine($"SMTP Alerts: {_currentSettings.MonitoringSettings.SmtpSettings.AlertsEnabled}");
            sb.AppendLine($"Discord Alerts: {_currentSettings.MonitoringSettings.DiscordSettings.AlertsEnabled}");
            sb.AppendLine();

            var timespan = TimeSpan.FromMinutes(_currentSettings.MonitoringSettings.AlertTimePeriodInMinutes);
            var oldDate = DateTime.UtcNow - timespan;

            foreach (var ip in ips)
            {
                sb.AppendLine($"IP: {IP.ConvertToString(ip.Address)}");
                
                try
                {
                    var list = new List<IP>() { ip };
                    var states = MonitorState.GetAllDevicePollsFromIps(list).Where(x => null != x.PingState).OrderByDescending(x => x.SubmitTime);
                    var lastState = states.FirstOrDefault();

                    if (null == lastState)
                    {
                        sb.AppendLine("Status: Offline | No Monitor State");
                        totalOffline++;
                    }
                    else
                    {
                        if (null == lastState.PingState)
                        {
                            sb.AppendLine("Status: Offline | No Ping State");
                            totalOffline++;
                        }
                        else
                        {
                            if (lastState.PingState.Response)
                            {
                                sb.AppendLine("Status: Online");
                                totalOnline++;
                            }
                            else
                            {
                                sb.AppendLine("Status: Offline");
                                totalOffline++;
                            }
                        }
                    }

                    var pingStates = states
                        .Where(x => x.PingState != null)
                        .Where(x => x.SubmitTime > oldDate)
                        .OrderBy(x => x.SubmitTime)
                        .ToList();

                    int totalCount = pingStates.Count();
                    int upCount = pingStates.Where(x => x.PingState!.Response == true).ToList().Count();

                    if (upCount <= 0)
                    {
                        upCount = 1;
                    }

                    if (totalCount <= 0)
                    {
                        totalCount = 1;
                    }

                    float uptimePercent = (upCount / totalCount) * 100;
                    totalUptime += uptimePercent;

                    sb.AppendLine($"Uptime Percentage: {(float)uptimePercent}%");
                }
                catch(Exception ex)
                {
                    sb.AppendLine("Status: Offline | No Monitor State");
                    sb.AppendLine($"Error fetching uptime percentage: {ex.Message}");
                }

                sb.AppendLine();
            }

            sb.AppendLine($"Total Online: {totalOnline}");
            sb.AppendLine($"Total Offline: {totalOffline}");
            sb.AppendLine($"Average Uptime: {(totalUptime / (ips.Count))}%");

            await command.FollowupAsync(sb.ToString(), ephemeral: true);
        }

        private async Task HandleOnlineAsync(SocketSlashCommand command)
        {
            await DeferIfNeededAsync(command, ephemeral: true);
            var ipText = command.Data.Options.First(o => o.Name == "ip").Value?.ToString();

            if (ipText == null)
            {
                await command.FollowupAsync("IP address is required.", ephemeral: true);
                return;
            }

            try
            {
                IP ip = await _monitoringAPI.GetDeviceAndMonitorStatesByStringIpAsync(ipText);

                if (ip.MonitorStateList == null || ip.MonitorStateList.Count == 0)
                {
                    await command.FollowupAsync("No monitor state found for the specified IP.", ephemeral: true);
                    return;
                }

                var lastState = ip.MonitorStateList.OrderByDescending(x => x.SubmitTime).FirstOrDefault();

                if (lastState == null)
                {
                    await command.FollowupAsync("No monitor state found for the specified IP.", ephemeral: true);
                    return;
                }

                string status = lastState.PingState?.Response == true ? "online" : "offline";
                await command.FollowupAsync($"The device with IP {ipText} was last seen as {status} at {lastState.SubmitTime}.", ephemeral: true);
                return;
            }
            catch(Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Failed to retrieve monitor state for IP: {ipText}");
                sb.Append(ex.Message);
                await command.FollowupAsync(sb.ToString(), ephemeral: true);
            }
        }

        private static async Task DeferIfNeededAsync(SocketSlashCommand command, bool ephemeral = true)
        {
            if (!command.HasResponded)
                await command.DeferAsync(ephemeral: ephemeral);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _client.StopAsync();
            }
            finally
            {
                _client.Dispose();
            }
        }
    }
}
