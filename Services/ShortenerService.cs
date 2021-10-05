using rapide_shortener_service.Model;
using rapide_shortener_service.Services.Abstract;
using rapide_shortener_service.Services.Utility;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HtmlAgilityPack;
using OpenGraphNet;
using Newtonsoft.Json;

namespace rapide_shortener_service.Services
{
    [Serializable]
    class DuplicateUrlException : Exception
    {
        public DuplicateUrlException()
        {

        }

        public DuplicateUrlException(string name)
            : base(String.Format("This Url already exists", name))
        {

        }

    }
    [Serializable]
    class DuplicateHashException : Exception
    {
        public DuplicateHashException()
        {

        }

        public DuplicateHashException(string name)
            : base(String.Format("This hash already exists", name))
        {

        }

    }

    class RawMetadata
    {
        public string local
        {
            get;
            set;
        }
        public string title
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }
        public string image
        {
            get;
            set;
        }

    }
    public class ShortenerService : IShortenerService
    {
        private readonly IMongoCollection<URLModel> _urls;

        public ShortenerService(IURLDatabaseSettings settings)
        {
            System.Console.WriteLine(settings.ConnectionString);
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _urls = database.GetCollection<URLModel>(settings.UrlCollectionName);
        }

        public List<URLModel> Get() =>
                   _urls.Find(el => true).ToList();
        public URLModel Check(string url) => _urls.Find<URLModel>(el => el.OriginalURL == url).FirstOrDefault();
        public URLModel Get(string url) => _urls.Find<URLModel>(el => el.ShortURL == url).FirstOrDefault();

        public URLModel Create(URLModel newUrlDoc)
        {
            try
            {

                _urls.InsertOne(newUrlDoc);
            }
            catch (MongoWriteException e)
            {


                if (e.ToString().Contains("unique_url"))
                    throw new DuplicateUrlException(newUrlDoc.OriginalURL);
                if (e.ToString().Contains("unique_hash"))
                    throw new DuplicateHashException(newUrlDoc.ShortURL);

                throw new MongoException("Unable to insert");
            }

            return newUrlDoc;
        }

        public string CreateUrl() => URLGenerator.GenerateShortUrl();

        public string SplitAnchorAndUrl(string url)
        {
            return url.Split("#")[0];
        }

        public async Task<String> ScrapMeta(string url)
        {
            try
            {
                System.Console.WriteLine(SplitAnchorAndUrl(url));
                OpenGraph graph = await OpenGraph.ParseUrlAsync(SplitAnchorAndUrl(url));
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(graph.ToString());
                string newContent = "<html><head><title>Fc lunaar</title>" + pageDocument.DocumentNode.OuterHtml + "</head><body></body></html><script>window.location.replace('" + url + "')</script>";
                return newContent;
            }
            catch (System.Exception)
            {

                string newContent = @"<html><head><title>Fc lunaar</title>
                                    <meta name= 'description' content='Shared Resource from Lunaar Application>< meta property = 'og:title' content = 'Welcome to Lunaar!' >
                                    <meta property ='og:description' content='Shared Resource from Lunaar Application'>
                                    <meta property ='og:type' content='website'>
                                    <meta property ='og:image' content='https://i.postimg.cc/pPGJSKfy/OG-Image.png'>
                                    <meta property ='twitter:image' content ='https://i.postimg.cc/pPGJSKfy/OG-Image.png'>
                                    </head>
                                    <body></body></html><script>window.location.replace('" + url + "')</script>";
                return newContent;

            }


        }
        public async Task<String> ScrapMetaRaw(string url)
        {
            try
            {
                OpenGraph graph = await OpenGraph.ParseUrlAsync(SplitAnchorAndUrl(url));
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(graph.ToString());

                RawMetadata newMetadata = new RawMetadata();


                if (graph.Metadata.ContainsKey("og:locale"))
                {
                    newMetadata.local = graph.Metadata["og:locale"][0].Value;
                }
                if (graph.Metadata.ContainsKey("og:title"))
                {
                    newMetadata.title = graph.Metadata["og:title"][0].Value;
                }
                if (graph.Metadata.ContainsKey("og:description"))
                {
                    newMetadata.description = graph.Metadata["og:description"][0].Value;
                }
                if (graph.Metadata.ContainsKey("og:image"))
                {
                    newMetadata.image = graph.Metadata["og:image"][0].Value;
                }

                string json = JsonConvert.SerializeObject(newMetadata);

                return json;
            }
            catch (System.Exception)
            {

                return JsonConvert.SerializeObject(new { });

            }


        }
    }
}


