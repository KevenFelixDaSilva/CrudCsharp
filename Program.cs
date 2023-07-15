using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello World!");

app.MapPost("/user", () => new {name = "keven", age = 12});

app.MapGet("/AddHeader", (HttpResponse response) => response.Headers.Add("Teste","Keven felix"));

app.MapPost("/products", (Product product) => {
   ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}",product.Code);
});

app.MapGet("/products/{Code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);
    if(product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapPut("/products", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.name = product.name;
    return Results.Ok();
});

app.MapDelete("/products/{Code}", ([FromRoute] string code) => {
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Delete(productSaved);
    return Results.Ok();
});

app.MapGet("/GetProductByHeader", (HttpRequest request) => {
    return request.Headers["product-code"].ToString();
});

if(app.Environment.IsDevelopment()){
    app.MapGet("/Configuration/Database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["Database:connection"]}/{configuration["Database:Port"]}");
    });
}

app.Run();

public static class ProductRepository{
    public static List<Product> Products { get; set; } = new List<Product>();

    public static void Init(IConfiguration configuration){
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product){          
        Products.Add(product);
    }

    public static Product GetBy(string code){
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Delete(Product product){
        Products.Remove(product);
    }
}



public class Product{
    public int Id { get; set; }
    public string Code {get; set;}
    public string name {get; set;}
    public string Description { get; set; }
}

public class ApplicationDbContext:DbContext {

    public DbSet<Product> Products { get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Server=localhost;Database=Products;User Id=sa;Password=@Sql2022;MultipleActiveResultSets=True;Encrypt=YES;TrustServerCertificate=YES;");
    
}
