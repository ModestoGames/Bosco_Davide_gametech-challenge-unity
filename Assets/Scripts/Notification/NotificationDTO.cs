using UnityEngine;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Data container to match java layer NotificationDTO and sync unity layer with java
    /// </summary>
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string WorkUUID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public long CreationTime { get; set; }
        public long SchedulationTime { get; set; }

        public int DurationInSeconds => (int)Mathf.Abs(SchedulationTime - CreationTime) / 1000;

        //use to update UI clock
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
           $"- Title: {Title}\n" +
           $"- Text: {Text}\n" +
           $"- CreationTime: {CreationTime} (ms since epoch)\n" +
           $"- SchedulationTime: {SchedulationTime} (ms since epoch)\n" +
           $"- Duration: {GetPrettyDurationString()} (mm:ss)";
        }
    }
}