using del.shared.DTOs;
using delteaching_dotnet.Domain;
using delteaching_dotnet.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Adicionando serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gerenciamento de Contas Bancárias",
        Version = "v1",
        Description = "API para realizar operações de CRUD em contas bancárias."
    });
});

builder.Services.AddScoped<DelTeachingContext>();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddDbContext<DelTeachingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Contas Bancárias v1");
    });
}

// **GET** - Listar todas as contas bancárias
app.MapGet("/api/v1/bank-accounts", async (DelTeachingContext dbContext) =>
{
    return Results.Ok(await dbContext.BankAccounts.FindAsync());
})
.WithTags("BankAccounts")
.WithName("GetAllAccounts")
.WithOpenApi();

// **GET** - Buscar uma conta bancária por ID
app.MapGet("/api/v1/bank-accounts/{id}", (int id, DelTeachingContext dbContext) =>
{
    var account = dbContext.BankAccounts.FirstOrDefault(c => c.Id == id);
    return account is not null ? Results.Ok(account) : Results.NotFound(new { message = "Conta não encontrada" });
})
    .WithTags("BankAccounts")
    .WithName("GetBankAccountById")
    .WithOpenApi();

// **POST** - Criar uma nova conta bancária
app.MapPost("/api/v1/bank-accounts", async (BankAccountDTO newAccountDTO, HttpContext context, DelTeachingContext dbContext) =>
{
    var validationResults = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(newAccountDTO, new ValidationContext(newAccountDTO), validationResults, true);

    if (!isValid)
    {
        return Results.BadRequest(validationResults);  // Retorna erros de validação
    }

    var newEntity = BankAccount.FromDTO(newAccountDTO);

    var account = dbContext.BankAccounts.Add(newEntity);
    await dbContext.SaveChangesAsync();

    var host = app.Configuration["ASPNETCORE_URLS"]?.Split(";").FirstOrDefault();

    var links = new List<string>
    {
        $"<{host}/api/v1/bank-accounts/{newEntity.Id}>; rel=\"self\"",
        $"<{host}/api/v1/bank-accounts/{newEntity.Id}>; rel=\"update\"",
        $"<{host}/api/v1/bank-accounts/{newEntity.Id}>; rel=\"delete\""
    };

    context.Response.Headers.Add("Link", string.Join(", ", links));

    return Results.Created($"/api/v1/bank-accounts/{newEntity.Id}", newEntity);
})
    .WithTags("BankAccounts")
    .WithName("CreateBankAccount")
    .WithOpenApi();


// **PUT** - Atualizar uma conta bancária
app.MapPut("/api/v1/bank-accounts/{id}", async (int id, BankAccountDTO updateBankAccount, DelTeachingContext dbContext) =>
{
    var validationResults = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(updateBankAccount, new ValidationContext(updateBankAccount), validationResults, true);

    if (!isValid)
    {
        return Results.BadRequest(validationResults);  // Retorna erros de validação
    }

    var entityAccount = dbContext.BankAccounts.FirstOrDefault(c => c.Id == id);

    if (entityAccount is null)
        return Results.NotFound(new { message = "Conta não encontrada" });

    entityAccount.Update(updateBankAccount);
    dbContext.BankAccounts.Update(entityAccount);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/api/v1/bank-accounts/{entityAccount.Id}", entityAccount);
})
    .WithTags("BankAccounts")
    .WithName("UpdateBankAccount")
    .WithOpenApi();

app.Run();
