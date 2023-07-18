using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product{
        Code = productRequest.Code,
        Name = productRequest.name,
        Description = productRequest.Description,
        Category = category
    };
    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach(var item in  productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item});

        }
    }
   context.Products.Add(product);
   context.SaveChanges();
    return Results.Created($"/products/{product.Id}",product.Id);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)   
        .Where(p => p.Id == id).First();

    if(product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) => {
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)   
        .Where(p => p.Id == id).First();
     
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    
    product.Code = productRequest.Code;
    product.Name = productRequest.name;
    product.Description = productRequest.Description;
    product.Category = category;
    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach(var item in  productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item});
        }
    }
    context.SaveChanges();
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
