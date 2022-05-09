namespace Emailit.Models.Pagination
{
    public class ReceivedMessagesPaginationParameters : PaginationParameters
    {
        public new int PageSize = 10;
        public string Search { get; set; }
    }
}
