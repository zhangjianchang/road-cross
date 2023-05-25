namespace Api.Entity
{
    public class SuggestionRequest
    {
        public int ID { get; set; }
        public string Suggestion { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        public string Status { get; set; }
        public string Answer { get; set; }
        public string CreateDate { get; set; }
        public string AnswerDate { get; set; }
    }
}
