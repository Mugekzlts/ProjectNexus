using System.Collections.Immutable;

var builder = WebApplication.CreateBuilder(args);

// --- SERVİS KAYITLARI ---
// .NET'e ToolService'i tanıtıyoruz. Uygulama boyunca tek bir depo (Singleton) olacak.
builder.Services.AddSingleton<ToolService>();

var app = builder.Build();

// --- ROTALAR (ENDPOINTS) ---

app.MapGet("/", () => "DevOps Toolbox API'sine Hos Geldiniz!");

// Tüm listeyi getir
app.MapGet("/tools", (ToolService service) => service.GetAll());

// ID ile tek bir araç getir
app.MapGet("/tools/{id}", IResult (int id, ToolService service) =>
{
    var tool = service.GetById(id);
    return tool is not null ? Results.Ok(tool) : Results.NotFound("Arac bulunamadi!");
});

// Yeni araç ekle
app.MapPost("/tools", (DevTool newTool, ToolService service) =>
{
    service.Add(newTool);
    return Results.Created($"/tools/{newTool.Id}", newTool);
});

// Güncelle
app.MapPut("/tools/{id}", IResult (int id, DevTool updatedTool, ToolService service) => 
{
    var success = service.Update(id, updatedTool);
    return success ? Results.NoContent() : Results.NotFound("Guncellenecek arac bulunamadi!");
});

// Sil
app.MapDelete("/tools/{id}", IResult (int id, ToolService service) =>
{
    var success = service.Delete(id);
    return success ? Results.Ok($"{id} numaralı arac sistemden silindi.") : Results.NotFound("Silinecek arac bulunamadi!");
});

// --- UYGULAMAYI ÇALIŞTIR ---
app.Run();

// --- TANIMLAMALAR (SINIFLAR VE MODELLER) ---
// Not: app.Run()'dan sonra sadece tanımlamalar yer alabilir.

public class ToolService
{
    private List<DevTool> _tools = new();
    private readonly string _path = "tools.json";

    public ToolService()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            _tools = System.Text.Json.JsonSerializer.Deserialize<List<DevTool>>(json) ?? new();
        }
        else
        {
            // Eğer dosya yoksa varsayılan verilerle başla
            _tools = new List<DevTool>
            {
                new DevTool(1, "Docker", "Containerization", true),
                new DevTool(2, "Kubernetes", "Orchestration", false)
            };
            Save();
        }
    }

    public List<DevTool> GetAll() => _tools;

    public DevTool? GetById(int id) => _tools.FirstOrDefault(t => t.Id == id);

    public void Add(DevTool tool)
    {
        _tools.Add(tool);
        Save();
    }

    public bool Update(int id, DevTool updatedTool)
    {
        var index = _tools.FindIndex(t => t.Id == id);
        if (index == -1) return false;

        _tools[index] = updatedTool;
        Save();
        return true;
    }

    public bool Delete(int id)
    {
        var tool = GetById(id);
        if (tool == null) return false;

        _tools.Remove(tool);
        Save();
        return true;
    }

    private void Save()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(_tools);
        File.WriteAllText(_path, json);
    }
}

public record DevTool(int Id, string Name, string Category, bool IsIntalled);