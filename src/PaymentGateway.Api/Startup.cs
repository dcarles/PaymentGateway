using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using PaymentGateway.Api.Authentication;
using PaymentGateway.Api.Core;
using PaymentGateway.BankProcessor;
using PaymentGateway.BankProcessor.Helpers;
using PaymentGateway.Data;
using PaymentGateway.PaymentsCore;
using PaymentGateway.PaymentsCore.Handlers;

namespace PaymentGateway.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostEnvironment env)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddMvcCore()
				.AddAuthorization()				
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

			DefaultContractResolver contractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			};

			services.AddControllers().AddNewtonsoftJson(jsonOptions =>
			{
				jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				};
			});


			services.AddHealthChecks();

			services.AddDbContext<GatewayDbContext>(c =>
			  c.UseSqlServer(Configuration.GetConnectionString("GatewayDbConnectionString")));
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


			// Register Mediator
			services.AddMediatR(typeof(CardPaymentHandler).Assembly);
			// Add AutoMapper
			services.AddAutoMapper(typeof(ApiMappingProfile), typeof(PaymentsMappingProfile),
				typeof(BankProcessorMappingProfile));

			// Transaction Processor Service Registrations
			services.AddTransient<IPaymentProcessor, PaymentProcessor>();
			services.AddTransient<IPaymentClient, PaymentClient>();
			// Http Client Registration
			services.AddHttpClient();


			services.AddMediatR(typeof(CardPaymentHandler).Assembly);
			// Add AutoMapper
			services.AddAutoMapper(typeof(ApiMappingProfile), typeof(PaymentsMappingProfile),
				typeof(BankProcessorMappingProfile));
		
		}
		

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
		{			
		
			app.UseRouting();		
			
			app.UseMiddleware<MerchantAuthenticationMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();			
			});
		}
	}
}
