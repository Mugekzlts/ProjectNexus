using System.Collections.Immutable;

string filePath = "tools.json";

var builder=  WebApplication.CreateBuilder(args);
// builder nesnesi; konfigürasyonları(portlar,şifreler), servisleri(veritabanı bağlantıları) ve 
// çevresel değişkenleri(staging mi, production mı) topladığımız yerdir.
// args -> parametremiz

builder.Services.AddSingleton<ToolService>();

var app = builder.Build();
// Build() dediğimiz anda artık ayarlar değiştirilemez hale gelir ve app adında, 
// çalışmaya hazır, canlı bir uyg. nesnemiz olur.

List<DevTool> myTools;

if (File.Exists(filePath))
{
    var json = File.ReadAllText(filePath);
    myTools = System.Text.Json.JsonSerializer.Deserialize<List<DevTool>>(json) ?? new List<DevTool>();
}
else
{
    myTools = new List<DevTool>
    {
        new DevTool(1, "Docker", "Containerization", true),
        new DevTool(2, "Kubernetes", "Orchestration", false)
    };
}

// List<DevTool> kullanarak geçici bir liste oluşturduk, bir nevi mini veritabanı


app.MapGet("/", () => "DevOps Toolbox API'sine Hos Geldiniz! ");
// Uygulamanın dış dünya ile konuşmasını sağlıyoruz.
// MapGet -> HTTP GET isteğidir. Sadece bakmak istenildiğinde çalışır.
// "/" -> root , localhost:5275 yazıldığında doğrudan burası tetiklenir.
// () => -> Lambda fonksiyonudur, istek buraya gelince şu işi yap der


app.MapGet("/tools", () => myTools);
// Kullanıcı /tools adresine gittiğinde, sistem myTools listesini otomatik olarak JSON
// formatına çevirip network üzerinden gönderecek.


app.MapPost("/tools", (DevTool newTool) =>
{
    myTools.Add(newTool);
    SaveToFile(myTools, filePath);
    return Results.Created($"/tools/{newTool.Id}", newTool);
});


app.MapGet("/tools/{id}", IResult(int id) =>
{
    var tool = myTools.FirstOrDefault(t => t.Id == id);
    return tool is not null ? Results.Ok(tool) : Results.NotFound("Arac bulunamadi!");
});
//FirstOrDefault -> Bu bir LINQ(language Integrated Query) sorgusudur. 
//IResult -> 'İçeride ne dönersem döneyim, bu bir HTTP sonucudur, sen bunu kabul et' demektir.

app.MapPut("/tools/{id}", IResult (int id, DevTool updatedTool) => 
{
    var toolIndex = myTools.FindIndex(t => t.Id == id);
    
    if (toolIndex == -1) return Results.NotFound("Guncellenecek arac bulunamadi!");

    myTools[toolIndex] = updatedTool;
    SaveToFile(myTools, filePath);

    return Results.NoContent();
});
//FindIndex -> FirstOrDefault gibi verinin kendisini değil, listedeki sırasını bulur.
// -1 Kontrolü -> Eger FindIndex aranan şeyi bulamazsa -1 döner.
//Results.NoContent() -> Bu HTTP 024 kodudur. 'İşlem başarılı, her şeyi güncelledim ama sana geri gönderecek...
//yeni bir verim yok(zaten sende var)' anlamına gelir.

app.MapDelete("/tools/{id}", IResult (int id) =>
{
    var tool = myTools.FirstOrDefault(t => t.Id ==id);
    if (tool is null) return Results.NotFound("Silinecek arac bulunamadi!");
    myTools.Remove(tool);
    SaveToFile(myTools, filePath);

    return Results.Ok($"{id} numaralı arac sistemden silindi.");
});
//Results.NotFound, Results.Ok tarzı komutlar .NET framework'ünün içinde hazır olarak varlar.

app.Run();
// Eklemezsek uygulama çalışır çalışmaz kapanır

void SaveToFile(List<DevTool> tools, string filePath)
{
    var json = System.Text.Json.JsonSerializer.Serialize(tools);
    File.WriteAllText(filePath, json);
}
//JsonSerializer.Serialize -> Bizim c# listemizi alıp bir metin yığınına dönüştürür
//File.WriteAllText -> Bu metni bilgisayarın diskine tools.json adıyla bir dosya olarak yazar.

public class ToolService
{
    private List<DevTool> _tools = new();
    private string _path = "tools.json";

    public ToolService()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            _tools = System.Text.Json.JsonSerializer.Deserialize<List<DevTool>>(json) ?? new();

        }
    }
    public List<DevTool> GetAll() => _tools;

    public void Add(DevTool tool)
    {
        _tools.Add(tool);
        var json = System.Text.Json.JsonSerializer.Serialize(_tools);
        File.WriteAllText(_path, json);
    }
}
//Depo sorumlumuz


// ---Tanımlama---
public record DevTool(int Id, string Name, string Category, bool IsIntalled);
// Model ? -> Verinin şablonu, içine kural koyduk.
// record -> Veriyi sadece taşımak için kullanılan hafif bir yapı

