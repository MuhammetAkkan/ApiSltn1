namespace ApiCourseSltn2
{
    public class Order
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalPrice { get; set; }
    }
}
