﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.Json;
using UKHO.ExternalNotificationService.API.Filters;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger<T> Logger;
        protected new HttpContext HttpContext => _httpContextAccessor.HttpContext!;
        public const string InternalServerError = "Internal Server Error";
        public readonly JsonSerializerOptions JOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        protected BaseController(IHttpContextAccessor httpContextAccessor, ILogger<T> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            Logger = logger;
        }

        protected string GetCurrentCorrelationId()
        {
            string? correlationId = _httpContextAccessor.HttpContext?.Request.Headers[CorrelationIdMiddleware.XCorrelationIdHeaderKey].FirstOrDefault();
            if (Guid.TryParse(correlationId, out Guid correlationGuid))
            {
                correlationId = correlationGuid.ToString();
            }
            else
            {
                LogError(EventIds.BadRequest.ToEventId(), null, "Invalid Correlation Id", correlationGuid.ToString());
                correlationId = correlationGuid.ToString();
            }
            return correlationId;
        }
        protected IActionResult BuildBadRequestErrorResponse(List<Error>? errors)
        {
            LogError(EventIds.BadRequest.ToEventId(), errors, "BadRequest", GetCurrentCorrelationId());

            return new BadRequestObjectResult(new ErrorDescription
            {
                Errors = errors ?? [],
                CorrelationId = GetCurrentCorrelationId()
            });
        }

        protected IActionResult BuildOkRequestErrorResponse(List<Error> errors)
        {
            LogError(EventIds.OK.ToEventId(), errors, "Ok", GetCurrentCorrelationId());

            return new OkObjectResult(new ErrorDescription
            {
                Errors = errors,
                CorrelationId = GetCurrentCorrelationId()
            });
        }
        protected IActionResult BuildInternalServerErrorResponse()
        {
            LogError(EventIds.InternalServerError.ToEventId(), null, "InternalServerError", GetCurrentCorrelationId());

            var objectResult = new ObjectResult
                (new InternalServerError
                {
                    CorrelationId = GetCurrentCorrelationId(),
                    Detail = InternalServerError,
                });
            objectResult.StatusCode = StatusCodes.Status500InternalServerError;
            return objectResult;
        }

        protected IActionResult GetEnsResponse(ExternalNotificationServiceResponse model, List<Error>? errors = null)
        {
            switch (model.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    return BuildOkResponse();

                case HttpStatusCode.Accepted:
                    return BuildAcceptedResponse();

                case HttpStatusCode.Created:
                    return BuildOkResponse();

                case HttpStatusCode.InternalServerError:
                    return BuildInternalServerErrorResponse();

                case HttpStatusCode.BadRequest:
                    return BuildBadRequestErrorResponse(errors);

                default:
                    return BuildInternalServerErrorResponse();
            }
        }

        private IActionResult BuildOkResponse()
        {
            LogInfo(EventIds.OK.ToEventId(), "OK", GetCurrentCorrelationId());
            return Ok();
        }

        private IActionResult BuildAcceptedResponse()
        {
            LogInfo(EventIds.Accepted.ToEventId(), "Accepted", GetCurrentCorrelationId());
            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }

        private void LogError(EventId eventId, List<Error>? errors, string errorType, string correlationId)
        {
            Logger.LogError(eventId, $"{HttpContext.Request.Path} - {errorType} - {{Errors}} for CorrelationId - {{correlationId}}", errors, correlationId);
        }

        private void LogInfo(EventId eventId, string infoType, string correlationId)
        {
            Logger.LogInformation(eventId, $"{HttpContext.Request.Path} - {infoType} - for CorrelationId - {{correlationId}}", correlationId);
        }
    }
}
