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
            // Get the path to the Firebase credentials from the environment variable
            string jsonFilePath = Environment.GetEnvironmentVariable("FIRESTORE_CREDENTIALS_PATH");
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                throw new InvalidOperationException("The environment variable FIRESTORE_CREDENTIALS_PATH is not set.");
            }

            // Initialize FirebaseApp only if it's not already initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(jsonFilePath)
                });
            }

            // Initialize FirestoreDb
            _firestoreDb = FirestoreDb.Create("pan-k-f6477");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> Get()
        {
            try
            {
                var reports = new List<Report>();
                var snapshot = await _firestoreDb.Collection("reports").GetSnapshotAsync();

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
            report.Id = documentReference.Id; // Set the ID from Firestore document

            return CreatedAtAction(nameof(Get), new { id = report.Id }, report); // Return 201 Created
        }
    }
}
