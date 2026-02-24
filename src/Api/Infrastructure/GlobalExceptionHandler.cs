using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Infrastructure;

/// <summary>
/// Global exception handler that converts exceptions to RFC 9457 Problem Details.
/// Business exceptions are exposed to users; system exceptions are hidden.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;
	private readonly IHostEnvironment _environment;

	public GlobalExceptionHandler(
		ILogger<GlobalExceptionHandler> logger,
		IHostEnvironment environment)
	{
		_logger = logger;
		_environment = environment;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var problemDetails = exception switch
		{
			BusinessException businessException => CreateBusinessProblemDetails(businessException),
			_ => CreateGenericProblemDetails(exception)
		};

		// Log the exception
		LogException(exception, problemDetails.Status ?? 500);

		httpContext.Response.StatusCode = problemDetails.Status ?? 500;
		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true; // Exception handled
	}

	private ProblemDetails CreateBusinessProblemDetails(BusinessException exception)
	{
		var problemDetails = new ProblemDetails
		{
			Type = exception.Type,
			Title = exception.Title,
			Status = exception.StatusCode,
			Detail = exception.Message
		};

		// Add validation errors if present
		if (exception is ValidationException validationException && validationException.Errors.Count > 0)
		{
			problemDetails.Extensions["errors"] = validationException.Errors;
		}

		// Add rule name if present
		if (exception is BusinessRuleException ruleException && ruleException.Rule is not null)
		{
			problemDetails.Extensions["rule"] = ruleException.Rule;
		}

		return problemDetails;
	}

	private ProblemDetails CreateGenericProblemDetails(Exception exception)
	{
		// In development, include exception details for debugging
		// In production, hide internal details for security
		var problemDetails = new ProblemDetails
		{
			Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
			Title = "An unexpected error occurred",
			Status = 500,
			Detail = _environment.IsDevelopment()
				? exception.Message
				: "An internal error occurred. Please try again later."
		};

		if (_environment.IsDevelopment())
		{
			problemDetails.Extensions["exception"] = exception.ToString();
		}

		return problemDetails;
	}

	private void LogException(Exception exception, int statusCode)
	{
		if (statusCode >= 500)
		{
			_logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
		}
		else
		{
			_logger.LogWarning("Business exception: {ExceptionType} - {Message}",
				exception.GetType().Name, exception.Message);
		}
	}
}
