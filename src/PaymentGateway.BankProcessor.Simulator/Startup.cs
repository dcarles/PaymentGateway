using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PaymentGateway.BankProcessor.Simulator
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration, IHostEnvironment env)
		{
			_configuration = configuration;
		}


		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddMvcCore()
				.AddAuthorization()
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());


			services.AddMediatR(Assembly.GetExecutingAssembly());

			services.AddControllers().AddNewtonsoftJson(jsonOptions =>
			{
				jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				};
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
		{			
			app.UseRouting();	

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
