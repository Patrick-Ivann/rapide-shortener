using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using rapide_shortener_service.Services;
namespace rapide_shortener_service.Controller
{
    public class ShortenerGrpcController : ShortenerServiceGrpc.ShortenerServiceGrpcBase
    {
        private readonly ILogger<ShortenerGrpcController> logger;
        private readonly ShortenerService _shortenerService;

        public ShortenerGrpcController(ILogger<ShortenerGrpcController> logger, ShortenerService service)
        {
            this.logger = logger;
            this._shortenerService = service;

        }

        public override Task<ShortenResponse> Shorten(ShortenRequest request, ServerCallContext context)
        {

            this.logger.LogInformation($"url received, url: {request.Url} , id: {request.Id} ");

            DateTime created_at = new DateTime();
            ObjectId newId = new ObjectId();
            String newUrl = _shortenerService.CreateUrl();


            try
            {
                _shortenerService.Create(new Model.URLModel { OriginalURL = request.Url, CreatedAt = created_at, UpdatedAt = created_at, Id = newId, ShortURL = newUrl });
                return Task.FromResult(new ShortenResponse { Ok = "Ok", Url = newUrl });

            }
            catch (Exception ex)
            {
                if (ex is DuplicateHashException)
                {
                    return Shorten(new ShortenRequest { Id = request.Id, Url = request.Url }, context);
                }
                if (ex is DuplicateUrlException)
                {

                    var existingUrl = _shortenerService.Check(request.Url);
                    return Task.FromResult(new ShortenResponse { Ok = "Ok", Url = existingUrl.ShortURL });
                }

                throw;

            }
        }
    }
}