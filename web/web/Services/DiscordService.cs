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
using DashLib.Models;

namespace web.Services {
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private TaskCompletionSource _readyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly SettingsService _settings;
        private readonly MonitorStateAPI _monitoringAPI;
        private readonly LoggingService _logger;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.DiscordService;

        public DiscordService(SettingsService settingsService, MonitorStateAPI monitoringAPI, LoggingService logger)
        {
            _settings = settingsService;
            _monitoringAPI = monitoringAPI;
            _logger = logger;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds
            });

            _client.Ready += OnReadyAsync;
            _client.SlashCommandExecuted += OnSlashCommandExecutedAsync;

            _logger.LogInfo("Sync startup completed, starting async tasks.", _logSource);
            _ = StartAsync();
        }

        private async Task OnReadyAsync()
        {
            _logger.LogInfo("Initiating post startup tasks.", _logSource);

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
            await _client.LoginAsync(TokenType.Bot, _settings.Discord.Token);
            await _client.StartAsync();

            _logger.LogInfo("Async start completed.", _logSource);
        }

        private async Task RegisterSlashCommandsAsync()
        {
            _logger.LogInfo("Registering discord bot commands.", _logSource);

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
            _logger.LogInfo($"Executing slash command: {command.CommandName}", _logSource);

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
                _logger.LogError("Command execution failed: " + ex.Message, _logSource);
                await command.FollowupAsync($"Command failed: {ex.Message}", ephemeral: true);
                throw;
            }
        }

        public async Task<bool> SendMessageAsync(string message)
        {
            try
            {
                _logger.LogInfo("Sending discord message.", _logSource);
                await _readyTcs.Task;

                var channel = _client.GetChannel(_settings.Discord.ChannelID) as IMessageChannel;
                if (channel is null)
                    throw new InvalidOperationException($"Discord channel not found or not message-capable: {_settings.Discord.ChannelID}");

                await channel.SendMessageAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Send discord message failed: " + ex.Message, _logSource);
                return false;
            }

        }

        private async Task HandleListAsync(SocketSlashCommand command)
        {
            _logger.LogInfo("Entered handle list method.", _logSource);

            try
            {
                _logger.LogInfo("Fetching monitored devices from API.", _logSource);
                await DeferIfNeededAsync(command, ephemeral: true);
                var list = await _monitoringAPI.GetMonitoredIpsAsync();

                if (list.Count == 0)
                {
                    _logger.LogInfo("API returned 0 monitored devices", _logSource);
                    await command.FollowupAsync("No monitored devices");
                }
                else
                {
                    _logger.LogInfo($"API returned {list.Count} monitored devices.", _logSource);
                    var sb = new StringBuilder();
                    foreach (var ip in list)
                    {
                        sb.AppendLine($"Address: {IpMonitoringTarget.ConvertToString(ip.Address)}");

                        if (null != ip.IcmpMonitorStates)
                        {
                            var last = PingState.GetMostRecentStateFromMonitoringTarget(ip).First();

                            if (null != last)
                            {
                                string status = last.Response == true ? "online" : "offline";
                                sb.AppendLine($"Last Online State: {status}");
                                sb.AppendLine($"Last Poll Time: {last.Timestamp}");
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

                    _logger.LogInfo("Finished creating list sending to discord.", _logSource);
                    await command.FollowupAsync(sb.ToString());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Handle list execution failed: " + ex.Message, _logSource);
            }
        }

        private async Task HandlePingAsync(SocketSlashCommand command)
        {
            _logger.LogInfo("Entered handle ping method.", _logSource);

            await DeferIfNeededAsync(command, ephemeral: true);
            var ipText = command.Data.Options.First(o => o.Name == "ip").Value?.ToString();

            if (ipText == null)
            {
                _logger.LogWarning("IP text from discord was null.", _logSource);
                await command.FollowupAsync("IP address is required.", ephemeral: true);
                return;
            }

            try
            {
                _logger.LogInfo("Polling dash API for ping response.", _logSource);
                var response = await _monitoringAPI.PingDeviceByStringIpAsync(ipText);

                if (null == response)
                {
                    _logger.LogWarning("HTTP response from ping request null.", _logSource);
                    await command.FollowupAsync($"No ping response for IP: {ipText}", ephemeral: true);
                    return;
                }
                else
                {
                    _logger.LogInfo("Ping response received sending to discord.", _logSource);
                    await command.FollowupAsync($"Ping response for IP {ipText}: {(response.IcmpResponse ? "Online" : "Offline")}" 
                        + (string.IsNullOrEmpty(response.Exception) ? "" : $" (Exception: {response.Exception})"), 
                        ephemeral: true);
                    return;
                }
            }
            catch (Exception ex)
            {
                await command.FollowupAsync($"Failed to retrieve monitor state for IP: {ipText}; {ex.Message}", ephemeral: true);
                _logger.LogError("API Request for ping response failed: " + ex.Message, _logSource);
            }
        }

        private async Task HandleSummaryAsync(SocketSlashCommand command)
        {
            _logger.LogInfo("Entered handle summary method.", _logSource);

            try
            {
                await DeferIfNeededAsync(command, ephemeral: true);

                //var ips = await _monitoringAPI.GetAllPingStatesAsync();
                //var report = MonitorState.GetMonitorStateSummaryFromIps(ips, _settings.All);
                throw new NotImplementedException();

                //await command.FollowupAsync(report, ephemeral: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Summary handle failed: " + ex.Message, _logSource);
            }
        }

        private async Task HandleOnlineAsync(SocketSlashCommand command)
        {
            _logger.LogInfo("Entered handle online method.", _logSource);

            await DeferIfNeededAsync(command, ephemeral: true);
            var ipText = command.Data.Options.First(o => o.Name == "ip").Value?.ToString();

            if (ipText == null)
            {
                await command.FollowupAsync("IP address is required.", ephemeral: true);
                return;
            }

            try
            {
                IpMonitoringTarget ip = await _monitoringAPI.GetDeviceAndMonitorStatesByStringIpAsync(ipText);
                var pings = PingState.GetMostRecentStateFromMonitoringTarget(ip);
                var ports = PortState.GetMostRecentStateFromMonitoringTarget(ip);

                if (pings.Count + ports.Count == 0)
                {
                    await command.FollowupAsync("No monitor state found for the specified IP.", ephemeral: true);
                    return;
                }

                var ping = pings.First();
                if (ping == null)
                {
                    await command.FollowupAsync("Last ping state returned null for the specified IP.", ephemeral: true);
                }
                else
                {
                    string status = ping.Response == true ? "online" : "offline";
                    _logger.LogInfo("Online status parsed, sending to discord.", _logSource);
                    await command.FollowupAsync($"The device with IP {ipText} was last seen as {status} at {ping.Timestamp}.", ephemeral: true);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ipText} : Failed to retrieve monitor states: " + ex.Message, _logSource);
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

        public async Task Restart()
        {
            _logger.LogInfo("Service restart initiated.", _logSource);

            await _client.StopAsync();
            _client.Dispose();
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds
            });
            _client.Ready += OnReadyAsync;
            _client.SlashCommandExecuted += OnSlashCommandExecutedAsync;
            await StartAsync();

            _logger.LogInfo("Service restart complete.", _logSource);
        }
    }
}
