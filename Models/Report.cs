using Google.Cloud.Firestore;
using Google.Cloud.Location;

namespace ReportAPI.Models
{
    [FirestoreData] //Suggested by CHatgpt
    public class Report
    {
        [FirestoreProperty] // Suggested by ChatGPT to fix serialization issues with Firestore
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
        }
        //References:
        //OpenAI. (2024). ChatGPT Conversation on API Development. Available at: https://www.openai.com (Accessed: 25 September 2024).
    }
  
  

}
