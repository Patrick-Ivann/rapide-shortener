namespace rapide_shortener_service.Model
{
    public class URLDatabaseSettings : IURLDatabaseSettings
    {
        public string UrlCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}