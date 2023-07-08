using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/user", () => new {name = "keven", age = 12});

app.MapGet("/AddHeader", (HttpResponse response) => response.Headers.Add("Teste","Keven felix"));

app.MapPost("/SaveProduct", (Product product) => {
   ProductRepository.Add(product);
    return StatusCodes.Status200OK;
});

app.MapGet("/GetProduct/{Code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);
    return product;
});

app.MapPut("/EditProduct", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.name = product.name;
    return StatusCodes.Status200OK;
});

app.MapDelete("/DeleteProduct/{Code}", ([FromRoute] string code) => {
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Delete(productSaved);
    return "Sucesso";
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
