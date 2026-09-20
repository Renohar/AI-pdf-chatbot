using PdfChatbot.backend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable SKEXP0070

var ollamaHttpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(5)
};

builder.Services.AddOllamaChatCompletion(
    modelId: "llama3.2:1b",
    httpClient: ollamaHttpClient
);

// builder.Services.AddOllamaChatCompletion(
//     modelId: "llama3.2",
//     endpoint: new Uri("http://localhost:11434")
// );


builder.Services.AddOllamaTextEmbeddingGeneration(
    modelId: "nomic-embed-text",
    endpoint: new Uri("http://localhost:11434")
);


#pragma warning restore SKEXP0070

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TextChunker>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<QdrantService>();
builder.Services.AddSingleton<ChatService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();