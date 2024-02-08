namespace DishesAPI.ResourseParameters
{
    public class DisheParamters
    {
        const int maxPageSize = 20;
        public string? SearchQuery { get; set; }
        public string? Name { get; set; }
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
