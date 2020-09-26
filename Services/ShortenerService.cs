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
using OpenGraphNet.Metadata;

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
    }
}


