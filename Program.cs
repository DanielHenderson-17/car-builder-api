using CarBuilder.Models;
using CarBuilder.Models.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services to the container (necessary even if configuration is in development block)
builder.Services.AddCors();

var app = builder.Build(); // Build the app

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Apply CORS policy in development only
    app.UseCors(options =>
    {
        options.AllowAnyOrigin();
        options.AllowAnyMethod();
        options.AllowAnyHeader();
    });
}

app.UseHttpsRedirection();

// In-memory collections acting as our "database"
List<PaintColor> paintColors = new List<PaintColor>
{
    new PaintColor { Id = 1, Color = "Silver", Price = 500 },
    new PaintColor { Id = 2, Color = "Midnight Blue", Price = 750 },
    new PaintColor { Id = 3, Color = "Firebrick Red", Price = 700 },
    new PaintColor { Id = 4, Color = "Spring Green", Price = 600 }
};

List<Interior> interiors = new List<Interior>
{
    new Interior { Id = 1, Material = "Beige Fabric", Price = 300 },
    new Interior { Id = 2, Material = "Charcoal Fabric", Price = 350 },
    new Interior { Id = 3, Material = "White Leather", Price = 800 },
    new Interior { Id = 4, Material = "Black Leather", Price = 850 }
};

List<Technology> technologies = new List<Technology>
{
    new Technology { Id = 1, Package = "Basic Package", Price = 200 },
    new Technology { Id = 2, Package = "Navigation Package", Price = 600 },
    new Technology { Id = 3, Package = "Visibility Package", Price = 750 },
    new Technology { Id = 4, Package = "Ultra Package", Price = 1200 }
};

List<Wheels> wheels = new List<Wheels>
{
    new Wheels { Id = 1, Style = "17-inch Pair Radial", Price = 400 },
    new Wheels { Id = 2, Style = "17-inch Pair Radial Black", Price = 450 },
    new Wheels { Id = 3, Style = "18-inch Pair Spoke Silver", Price = 500 },
    new Wheels { Id = 4, Style = "18-inch Pair Spoke Black", Price = 550 }
};

List<Order> orders = new List<Order>();

app.MapGet("/paintcolors", () => paintColors);
app.MapGet("/interiors", () => interiors);
app.MapGet("/technologies", () => technologies);
app.MapGet("/wheels", () => wheels);


app.MapGet("/orders", (int? paintId) => 
{
    foreach (Order order in orders)
    {
        order.Wheel = wheels.First(w => w.Id == order.WheelId);
        order.Technology = technologies.First(t => t.Id == order.TechnologyId);
        order.PaintColor = paintColors.First(p => p.Id == order.PaintId);
        order.Interior = interiors.First(i => i.Id == order.InteriorId);
    }

    List<Order> filteredOrders = orders.Where(o => !o.Complete).ToList();

    if (paintId != null)
    {
        filteredOrders = filteredOrders.Where(order => order.PaintId == paintId).ToList();
    }

    return filteredOrders.Select(o => new OrderDTO
    {
        Id = o.Id,
        Timestamp = o.Timestamp,
        TechnologyId = o.TechnologyId,
        Technology = new TechnologyDTO
        {
            Id = o.Technology.Id,
            Package = o.Technology.Package,
            Price = o.Technology.Price
        },
        WheelId = o.WheelId,
        Wheels = new WheelsDTO
        {
            Id = o.Wheel.Id,
            Style = o.Wheel.Style,
            Price = o.Wheel.Price
        },
        InteriorId = o.InteriorId,
        Interior = new InteriorDTO
        {
            Id = o.Interior.Id,
            Material = o.Interior.Material,
            Price = o.Interior.Price
        },
        PaintId = o.PaintId,
        PaintColor = new PaintColorDTO
        {
            Id = o.PaintColor.Id,
            Color = o.PaintColor.Color,
            Price = o.PaintColor.Price
        },
        Complete = o.Complete
    }).ToList();
});

// Endpoint to create a new order
app.MapPost("/orders", (Order order) =>
{
    Console.WriteLine($"PaintId: {order.PaintId}");
    Console.WriteLine($"InteriorId: {order.InteriorId}");
    Console.WriteLine($"TechnologyId: {order.TechnologyId}");
    Console.WriteLine($"WheelId: {order.WheelId}");

    order.Id = orders.Any() ? orders.Max(o => o.Id) + 1 : 1;
    order.Timestamp = DateTime.Now;
    order.Complete = false;

    orders.Add(order);

    Console.WriteLine($"Order ID: {order.Id}");
    Console.WriteLine($"Order Timestamp: {order.Timestamp}");
    Console.WriteLine($"Order Complete: {order.Complete}");

    return Results.Created($"/orders/{order.Id}", new OrderDTO
    {
        Id = order.Id,
        Timestamp = order.Timestamp,
        Complete = order.Complete,
        WheelId = order.WheelId,
        Wheels = new WheelsDTO
        {
            Id = order.WheelId,
            Price = order.Wheel.Price,
            Style = order.Wheel.Style
        },
        TechnologyId = order.TechnologyId,
        Technology = new TechnologyDTO
        {
            Id = order.TechnologyId,
            Price = order.Technology.Price,
            Package = order.Technology.Package
        },
        PaintId = order.PaintId,
        PaintColor = new PaintColorDTO
        {
            Id = order.PaintId,
            Price = order.PaintColor.Price,
            Color = order.PaintColor.Color
        },
        InteriorId = order.InteriorId,
        Interior = new InteriorDTO
        {
            Id = order.InteriorId,
            Price = order.Interior.Price,
            Material = order.Interior.Material
        }
    });
});

// Endpoint to fulfill an existing order
app.MapPost("/orders/{id}/fulfill", (int id) => {
    Order selectedOrder = orders.FirstOrDefault(o => o.Id == id);
    if (selectedOrder != null)
    {
        selectedOrder.Complete = true;
        return Results.Ok(new OrderDTO
        {
            Id = selectedOrder.Id,
            WheelId = selectedOrder.WheelId,
            InteriorId = selectedOrder.InteriorId,
            PaintId = selectedOrder.PaintId,
            TechnologyId = selectedOrder.TechnologyId,
            Timestamp = selectedOrder.Timestamp,
            Complete = selectedOrder.Complete
        });
    }
    else
    {
        return Results.NotFound(); // Return 404 if the order was not found
    }
});

app.Run();
