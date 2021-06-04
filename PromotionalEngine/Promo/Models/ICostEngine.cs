using System;
using System.Collections.Generic;
using System.Text;

namespace SKU.Models
{
    public interface ICostEngine
    {
        decimal Calculate(List<Order> orders);
    }
}
