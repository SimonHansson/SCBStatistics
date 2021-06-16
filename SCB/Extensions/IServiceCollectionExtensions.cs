using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;

namespace SCB.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IServiceCollection"/>
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		private const int _retryBase = 2;
		private const int _maxRetryCount = 5;

		/// <summary>
		/// Registers the refit API.
		/// </summary>
		/// <typeparam name="TService">Client to register</typeparam>
		/// <param name="services">The services.</param>
		/// <param name="endpointUri">The endpoint URI.</param>
		/// <exception cref="ArgumentNullException">Thrown if services is null</exception>
		/// <exception cref="ArgumentException">Thrown if endpointUri is null or whitespace</exception>
		public static void RegisterRefitApi<TService>(this IServiceCollection services, string endpointUri)
			where TService : class
		{

			if (services is null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (String.IsNullOrWhiteSpace(endpointUri))
			{
				throw new ArgumentException(nameof(endpointUri));
			}

			services.AddRefitClient<TService>()
				.ConfigureHttpClient(client => client.BaseAddress = GetUri(endpointUri))
				.AddTransientHttpErrorPolicy(builder =>
                {
                    return builder
                        .WaitAndRetryAsync(_maxRetryCount,
                            (retryCount) =>
                                TimeSpan.FromSeconds(Math.Pow(_retryBase, retryCount)),
                            (result, timeSpan) =>
                            {
                                TraceException(result, endpointUri, timeSpan.Seconds);
                            });
                });
        }

		/// <summary>
		/// Registers the refit API.
		/// </summary>
		/// <typeparam name="TService">Client to register</typeparam>
		/// <typeparam name="TMessageHandler">Message handler to register</typeparam>
		/// <param name="services">The services.</param>
		/// <param name="endpointUri">The endpoint URI.</param>
		/// <exception cref="ArgumentNullException">Thrown if services is null</exception>
		/// <exception cref="ArgumentException">Thrown if endpointUri is null or whitespace</exception>
		public static void RegisterRefitApi<TService, TMessageHandler>(this IServiceCollection services, string endpointUri)
			where TService : class
			where TMessageHandler : DelegatingHandler
		{

			if (services is null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (String.IsNullOrWhiteSpace(endpointUri))
			{
				throw new ArgumentException(nameof(endpointUri));
			}

			services.AddRefitClient<TService>()
				.ConfigureHttpClient(client => client.BaseAddress = GetUri(endpointUri))
				.AddHttpMessageHandler<TMessageHandler>()
				.AddTransientHttpErrorPolicy(builder =>
				{
					return builder
						.WaitAndRetryAsync(_maxRetryCount,
							(retryCount) =>
								TimeSpan.FromSeconds(Math.Pow(_retryBase, retryCount)),
							(result, timeSpan) =>
							{
								TraceException(result, endpointUri, timeSpan.Seconds);
							});
				});
		}

		private static void TraceException(DelegateResult<HttpResponseMessage> result, string endpointUri, int seconds)
		{
			if (result?.Exception != null)
			{
				Trace.TraceError($"Exception adding refit client for endpoint {endpointUri}: {result.Exception.Message}");
			}
			else
			{
				Trace.TraceError($"Exception adding refit client for endpoint {endpointUri}");
			}

			Trace.WriteLine($"Retrying in {seconds} seconds");
		}

		private static Uri GetUri(string uriString)
		{
			if (String.IsNullOrWhiteSpace(uriString) || !Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
			{
				throw new ArgumentException($"{nameof(uriString)} is not a well formed URI", nameof(uriString));
			}

			return new Uri(uriString, UriKind.Absolute);
		}

	}
}
