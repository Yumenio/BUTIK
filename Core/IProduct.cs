using System;

namespace Core
{
    public interface IProduct
    {
        int ProductID { get; set; }
        int Price { get; set; }
        string ProductName { get; set; }
    }
}
