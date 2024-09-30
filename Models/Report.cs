using Google.Cloud.Firestore;
using Google.Cloud.Location;

namespace ReportAPI.Models
{
    [FirestoreData]
    public class Report
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string description { get; set; }

        [FirestoreProperty]
        public GeoPoint location { get; set; }

        [FirestoreProperty]
        public string imageUrl { get; set; }

        [FirestoreProperty]
        public long timestamp { get; set; }

        [FirestoreProperty]
        public string title { get; set; }

        [FirestoreProperty]
        public string userId { get; set; }

        public Report()
        {
            Id = string.Empty;
            description = string.Empty;
            location = new GeoPoint(0, 0); // Initialize with default values
            imageUrl = string.Empty;
            timestamp = 0;
            title = string.Empty;
            userId = string.Empty;
        }
    }
}
