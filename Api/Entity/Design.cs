namespace Api.Entity
{
    public class Design
    {
        public string Guid { get; set; }
        public string RoadName { get; set; }
        public string DesignJson { get; set; }
        public string UserName { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
    }

    public class SaveRequest : Design { }
}
