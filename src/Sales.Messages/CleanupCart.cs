namespace Sales.Messages
{
    public class CleanupCart
    {
        public int ProductId { get; set; }
        public string CartId { get; set; }
        public string RequestId { get; set; }
    }
}
