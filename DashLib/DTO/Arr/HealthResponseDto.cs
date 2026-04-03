using DashLib.Models.Monitoring.Arr;
using DashLib.Models.MonitoringTargets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Arr
{
    public class HealthResponseDto
    {
        //https://sonarr.tv/docs/api/#v3/tag/health/GET/api/v3/health
        public int Id { get; set; }
        public string? Source { get; set; }
        public HealthType Type { get; set; }
        public string? Message { get; set; }
        public WikiUrlDto WikiUrl { get; set; }

        public enum HealthType
        {
            Ok,
            Notice,
            Warning,
            Error
        }

        public HealthResponseDto()
        {
            Id = 0;
            Type = HealthType.Ok;
            WikiUrl = new WikiUrlDto();
        }

        public ArrHealthMonitorState ToMonitorState(ArrMonitoringTarget target)
        {
            var state = new ArrHealthMonitorState(target);
            state.Source = Source ?? string.Empty;
            state.Type = Type;
            state.Message = Message ?? string.Empty;
            state.FullUri = WikiUrl.FullUri ?? string.Empty;
            state.Scheme = WikiUrl.Scheme ?? string.Empty;
            state.Host = WikiUrl.Host ?? string.Empty;
            state.Port = WikiUrl.Port ?? string.Empty;
            state.Path = WikiUrl.Path ?? string.Empty;
            state.Query = WikiUrl.Query ?? string.Empty;
            state.Fragment = WikiUrl.Fragment ?? string.Empty;
            return state;
        }
    }

    public class WikiUrlDto
    {
        public string? FullUri { get; set; }
        public string? Scheme { get; set; }
        public string? Host { get; set; }
        public string? Port { get; set; }
        public string? Path { get; set; }
        public string? Query { get; set; }
        public string? Fragment { get; set; }
    }
}
