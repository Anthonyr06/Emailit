namespace Emailit.Models.Pagination
{
    public class SentMessagesPaginationParameters : PaginationParameters
    {
        public new int PageSize = 20;
        public string Search { get; set; }
    }
}
