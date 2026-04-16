using NUnit.Framework;
using OTS_Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS_Supermarket.Test
{
    [TestFixture]
    public class CartTest
    {
        [Test]
        public void AddOneToCart_ShouldAddItemToCart_Success()
        {
            //ARRANGE preduslovi
            Cart cart = new Cart();
            cart.Size = 5;
            Monitor monitor = new Monitor();

            //ACT
            cart.AddOneToCart(monitor);

            //ASSERT
            Assert.That(cart.Size, Is.EqualTo(6));
            Assert.That(cart.Amount, Is.EqualTo(100));
        }
    }
}
