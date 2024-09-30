using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using ReportAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;

        public ReportController()
        {
            // Get the Google credentials from the environment variable
            var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            if (string.IsNullOrEmpty(credentialsPath))
            {
                throw new InvalidOperationException("Google credentials not found in environment variables.");
            }

            // Initialize FirebaseApp if it's not already initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }

            // Initialize FirestoreDb
            _firestoreDb = FirestoreDb.Create("pan-k-f6477");
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Report>>> Get(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID cannot be null or empty.");
                }

                var reports = new List<Report>();
                var snapshot = await _firestoreDb.Collection("reports")
                                                .WhereEqualTo("userId", userId)
                                                .GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    var report = document.ConvertTo<Report>();
                    report.Id = document.Id;
                    reports.Add(report);
                }

                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    


        [HttpPost]
        public async Task<ActionResult<Report>> Post([FromBody] Report report)
        {
            if (report == null)
            {
                return BadRequest("Report cannot be null.");
            }

            var documentReference = await _firestoreDb.Collection("reports").AddAsync(report);
            report.Id = documentReference.Id;

            return CreatedAtAction(nameof(Get), new { id = report.Id }, report);
        }
    }
}
