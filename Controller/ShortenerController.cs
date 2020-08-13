using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using rapide_shortener_service.Services.Abstract;
using rapide_shortener_service.Model;
using rapide_shortener_service.Services;


namespace rapide_shortener_service.Controller
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShortenerController : ControllerBase
    {

        //   private IShortenerService shortUrlService;

        private readonly ILogger<ShortenerController> logger;
        private readonly ShortenerService _shortenerService;

        public ShortenerController(ILogger<ShortenerController> logger, ShortenerService shortenerService)
        {
            this.logger = logger;
            this._shortenerService = shortenerService;

        }

        // GET: api/Default
        [HttpGet]
        public ActionResult<List<URLModel>> Get()
        {
            return _shortenerService.Get();
        }

        [HttpGet("{shorturlString}")]
        public IActionResult Get([FromRoute] string shorturlString, bool redirect = true)
        {

            var result = _shortenerService.Get(shorturlString);

            System.Console.WriteLine("jfhsdjkfhsjdhfksdjh");
            logger.LogDebug("fjshdkfjhf");
            logger.LogDebug(shorturlString);

            if (result != null)
            {
                if (redirect)
                {

                    var metadatas = _shortenerService.ScrapMeta(result.OriginalURL).GetAwaiter().GetResult();
                    //var html = _shortenerService.GenerateHtml(metadatas);
                    return new ContentResult()
                    {
                        Content = metadatas,
                        ContentType = "text/html",
                    };



                    //return Ok();
                    //return Redirect(result.OriginalURL);
                }
                else
                {
                    return Ok(result);
                }
            }
            logger.LogDebug("no result");

            return NotFound();
        }


    }
}