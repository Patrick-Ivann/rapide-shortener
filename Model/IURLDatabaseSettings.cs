namespace rapide_shortener_service.Model
{
    public interface IURLDatabaseSettings
    {
        string UrlCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}