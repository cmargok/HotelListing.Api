using HotelListing.API.Commos.Configurations;
using HotelListing.API.Data;
using HotelListing.API.Handlers.Middleware;
using HotelListing.API.Handlers.Security;
using HotelListing.API.Handlers.Security.Data;
using HotelListing.API.Repository;
using HotelListing.API.Repository.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<HotelListingDBContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("HotelConnectionString")));


builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>(builder.Configuration["JwtSettings:Issuer"])
    .AddEntityFrameworkStores<HotelListingDBContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<IAuthHandler, AuthHandler>();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Swagger **************************************************************************
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Hotel Listing API",
            Version= "v1",
            Contact = new OpenApiContact{
                Name = "Kevin Camargo",
                Email = "cmargokk@gmail.com",
            },
            License = new OpenApiLicense
            {
                Name= "License - Free",               
            },
            Description = "This Api is base of many things that you need to create your own API",
        });

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Description = "@JWT Authorization header using the Bearer Scheme." +
            "Enter 'Bearer [space] and then your token in the text input below." +
            "Example = 'Bearer 123456abcdefg'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        
                    },
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    In = ParameterLocation.Header,
                    Scheme = "0auth2"
                },
                new List<string>()

            }
        });
    });


//***********************************************************************************************

//adding Cors Policy**********************************************************************

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        policy => policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

//***********************************************************************************************

//adding versioning********************************************************************************
builder.Services.AddApiVersioning(
    options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
      /*  options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Version"),
            new MediaTypeApiVersionReader("ver")
            );*/
    }
);

builder.Services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });




//***********************************************************************************************

//adding serilog
builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.WriteTo.Console()
                                                            .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddAutoMapper(typeof(MapperConfig));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository,CountriesRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();

builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
       // options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(JWToptions =>
    {
        JWToptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        };
    }
);

//CACHING ********************************************************************

builder.Services.AddResponseCaching(
    options =>
    {
        options.MaximumBodySize = 1024;
        options.UseCaseSensitivePaths= true;     
    } );


//****************************************************************************



builder.Services.AddControllers().AddOData(
    options =>
    {
        options.Select().Filter().OrderBy();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors("AllowAll");


//CACHING ********************************************************************

app.UseResponseCaching();

app.Use(async (context, next) 
    =>{
        context.Response.GetTypedHeaders().CacheControl =
                new Microsoft.Net.Http.Headers.CacheControlHeaderValue() 
                { 
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(10)
                };
        context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = 
                new string[] 
                { 
                    "Accept-Encoding" 
                };
        await next();
    });

//****************************************************************************

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
