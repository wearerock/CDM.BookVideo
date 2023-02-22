using System.Reflection;
using CDM.BookVideo.API.Extensions;
using CDM.BookVideo.Application.Background;
using CDM.BookVideo.Application.BusinessRules;
using CDM.BookVideo.Application.Commands;
using CDM.BookVideo.Application.Commands.Update;
using CDM.BookVideo.Application.Events;
using CDM.BookVideo.Application.Events.EventHandling;
using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.Application.Queries.Orders;
using CDM.BookVideo.Domain;
using CDM.BookVideo.EventBus;
using CDM.BookVideo.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<BookVideoContext>(opt => { opt.UseInMemoryDatabase("BookVideoShop"); });
builder.Services.AddMediatR(opt => { 
  opt.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBusinessRuleFactory, BusinessRuleFactory>();
builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, CreateOrderCommandResult>, CreateOrderCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateOrderCommand, UpdateOrderCommandResult>, UpdateOrderCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DeleteOrderCommand, bool>, DeleteOrderCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetAllOrdersQuery, List<GetOrderQueryResult>>, GetAllOrdersQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetOrderQuery, GetOrderQueryResult>, GetOrderQueryHandler>();
builder.Services.AddTransient<OrderPurchasedEventHandler>();
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddHostedService<ShippingService>();

builder.Services.AddSwaggerGen(c => {
  c.SwaggerDoc("v1", new OpenApiInfo {
    Version = "v1",
    Title = "Ordering API",
    Description = "An API for the Ordering application"
  });

  c.EnableAnnotations();

  var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ConfigureEventBus(app);

void ConfigureEventBus(IApplicationBuilder app) {
  var bus = app.ApplicationServices.GetRequiredService<IEventBus>();
  bus.Subscribe<OrderPurchasedEvent, OrderPurchasedEventHandler>();
}

app.UseCustomExceptionHandler();

app.Run();
