namespace Emailit.Models.Pagination
{
    public class UserPaginationParameters : PaginationParameters
    {
        public new int PageSize = 8;
        public new int NavPaginationMaxNumber = 7;

        public string _idCard;
        public string IdCard
        {
            get
            {
                return _idCard;
            }
            set
            {
                _idCard = value != null ? value.Remove(3, 1).Remove(10, 1) : value;
            }
        }
    }
}
