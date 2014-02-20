using System;
using Newtonsoft.Json;

namespace BuildMonitor.Domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IProject
    {
        Guid Id { get; }
        [JsonProperty] string Name { get; }
    }

    public class Project : IProject
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}