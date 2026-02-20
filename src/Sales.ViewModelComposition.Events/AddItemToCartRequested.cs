namespace Sales.ViewModelComposition.Events
{
    public class AddItemToCartRequested
    {
        public string RequestId { get; set; }
        public string CartId { get; set; }
    }
}
