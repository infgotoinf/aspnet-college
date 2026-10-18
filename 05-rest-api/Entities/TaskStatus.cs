using System.Text.Json.Serialization;

namespace TaskFlowApi.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    ToDo,
    InProgress,
    Done
}
