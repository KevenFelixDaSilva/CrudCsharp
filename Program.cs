using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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

app.Run();

public static class ProductRepository{
    public static List<Product> Products { get; set; }

    public static void Add(Product product){
        if(Products == null){
            Products = new List<Product>();   
        }
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
 public string Code {get; set;}
 public string name {get; set;}
}
