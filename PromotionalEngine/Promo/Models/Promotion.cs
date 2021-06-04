using System;
using System.Collections.Generic;
using System.Text;

namespace SKU.Models
{
    public enum Scenario
    {
        FIXED,
        PERCENTAGE
    }
    public class Promotion
    {
        public string PromotionFactor { get; set; }
        public Scenario Scenario { get; set; }
        public decimal ConsiderableFactor { get; set; }
    }
}
