
namespace Api.Entity
{
    public class ApiLog
    {
        public string Source { get; set; }
        public string LogLevel { get; set; }
        public string Msg { get; set; }
        public string DetailTrace { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string RequestInfo { get; set; }
        public string ResponseTime { get; set; }
        public string LogTime { get; set; }


    }
    public class ApiLogParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Path { get; set; }
        public string Level { get; set; }
        public string SearchKey { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
