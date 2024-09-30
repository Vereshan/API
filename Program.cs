using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using static Google.Rpc.Context.AttributeContext.Types;

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase using environment variable
var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
if (string.IsNullOrEmpty(credentialsPath))
{
    throw new InvalidOperationException("Google credentials not found in environment variables.");
}

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(credentialsPath),
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

