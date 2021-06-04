using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKU.Models
{
    public class CostEngineRepository: ICostEngine
    {
        private List<Promotion> Promotions { get; set; }
        private List<Cost> ProductCost { get; set; }


        public CostEngineRepository()
        {
            Promotions = new List<Promotion>();
            ProductCost = new List<Cost>();

            Configure();
        }

        private void Configure()
        {
            ProductCost.Add(new Cost { Product="A", Price=50 });
            ProductCost.Add(new Cost { Product = "B", Price = 30 });
            ProductCost.Add(new Cost { Product = "C", Price = 20 });
            ProductCost.Add(new Cost { Product = "D", Price = 15 });

            Promotions.Add(new Promotion { PromotionFactor = "3A", Scenario = Scenario.FIXED, ConsiderableFactor = 130 });
            Promotions.Add(new Promotion { PromotionFactor = "2B", Scenario = Scenario.FIXED, ConsiderableFactor = 45 });
            Promotions.Add(new Promotion { PromotionFactor = "1C1D", Scenario = Scenario.FIXED, ConsiderableFactor = 30 });
        }

        public decimal Calculate(List<Order> orders)
        {

            decimal orderCost = 0;
            var orderGroup = orders.GroupBy(x => x.Product).Select(x => new  Order{ Product = x.Key, Quantity = x.Sum(y => y.Quantity) }).ToList();
            List<Order> FinalOrder = new List<Order>();

            foreach (var promotion in Promotions)
            {
                if(orderGroup.Count==0)
                {
                    break;
                }

                List<Order> promoOrders = new List<Order>();
                var promo = promotion.PromotionFactor.ToCharArray();
                var num = "";
                for (int i = 0; i < promo.Length; i++)
                {
                    if (char.IsDigit(promo[i]))
                    {
                        num += promo[i].ToString();
                    }
                    else if(char.IsLetter(promo[i]))
                    {
                        promoOrders.Add(new Order { Product = promo[i].ToString(), Quantity = int.Parse(num) });
                        num = "";
                    }
                }


                bool flag = true;
                do
                {
                    flag = true;
                    foreach (var p in promoOrders)
                    {
                        if (orderGroup.Where(x => x.Product.Equals(p.Product) && x.Quantity >= p.Quantity).FirstOrDefault() == null)
                        {
                            flag = false;
                            break;
                        }
                    }


                    if (flag)
                    {
                        for (int i = 0; i < orderGroup.Count; i++)
                        {
                            var orderPromo = promoOrders.Where(x => x.Product.Equals(orderGroup[i].Product)).FirstOrDefault();
                            if(orderPromo==null)
                            {
                                continue;
                            }
                            orderGroup[i].Quantity = orderGroup[i].Quantity- orderPromo.Quantity;
                            if(orderGroup[i].Quantity==0)
                            {
                                orderGroup.RemoveAt(i);
                                i--;
                            }
                        }

                        orderCost += (decimal)promotion.ConsiderableFactor;
                    }
                } while (flag);

            }

            foreach (var order in orderGroup.Where(x=>x.Quantity>0))
            {
                var o = ProductCost.Where(x => x.Product.Equals(order.Product)).FirstOrDefault();
                if(o==null)
                {
                    continue;
                }

                orderCost += order.Quantity * o.Price;

            }

            return orderCost;

        }

    }
}
