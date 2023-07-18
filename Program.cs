using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product{
        Code = productRequest.Code,
        name = productRequest.name,
        Description = productRequest.Description,
        Category = category
    };
   context.Products.Add(product);
   context.SaveChanges();
    return Results.Created($"/products/{product.Id}",product.Id);
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
