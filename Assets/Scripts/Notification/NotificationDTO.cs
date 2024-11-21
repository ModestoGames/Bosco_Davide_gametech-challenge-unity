using com.modesto.notificationhandler;
using UnityEngine;

public class NotificationDTO
{
    public int Id { get;set; }
    public string WorkUUID { get; set; }
    public NotificationStatus Status { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public long CreationTime { get; set; }
    public long SchedulationTime { get; set; }

    public int DurationInSeconds => (int)Mathf.Abs(SchedulationTime - CreationTime) / 1000;

    public string GetPrettyDurationString()
    {
        int minutes = DurationInSeconds / 60;
        int remainingSeconds = DurationInSeconds % 60;

        return $"{minutes:D1}:{remainingSeconds:D2}";
    }

    public NotificationDTO(int id)
    {
        Id = id;
    }

    public NotificationDTO(int id, AndroidJavaObject androidDto) : this(id)
    {
        WorkUUID = androidDto.Call<string>("getUUIDToString");
        Status = (NotificationStatus)androidDto.Call<int>("getStatus");
        Title = androidDto.Call<string>("getTitle");
        Text = androidDto.Call<string>("getText");
        CreationTime = androidDto.Call<long>("getCreationTime");
        SchedulationTime = androidDto.Call<long>("getSchedulationTime");
    }

    public override string ToString()
    {
        return $"NotificationDTO:\n" +
       $"- Id: {Id}\n" +
       $"- WorkUUID: {WorkUUID}\n" +
       $"- Status: {Status}\n" +
       $"- Title: {Title}\n" +
       $"- Text: {Text}\n" +
       $"- CreationTime: {CreationTime} (ms since epoch)\n" +
       $"- SchedulationTime: {SchedulationTime} (ms since epoch)\n" +
       $"- Duration: {GetPrettyDurationString()} (mm:ss)";
    }
}
