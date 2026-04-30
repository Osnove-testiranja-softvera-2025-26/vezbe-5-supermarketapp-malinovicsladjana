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

            [Test]
            public void AddOneToCart_TooManyItems_ThrowsException()
            {
                Cart cart = new Cart();
                cart.Size = 10; // vec je pun
                Monitor monitor = new Monitor();

                var ex = Assert.Throws<Exception>(() => cart.AddOneToCart(monitor));
                Assert.That(ex.Message, Is.EqualTo("Number of items in cart must be 10 or less!"));
            }

            [Test]
            public void AddMultipleToCart_ShouldIncreaseSpecificCounter()
            {
                Cart cart = new Cart();
                Laptop laptop = new Laptop { Price = 500 };

                cart.AddMultipleToCart(laptop, 3);

                Assert.Multiple(() =>
                {
                    Assert.That(cart.Size, Is.EqualTo(3));
                    Assert.That(cart.Laptop_counter, Is.EqualTo(3));
                    Assert.That(cart.Amount, Is.EqualTo(1500));
                });
            }

            [Test]
            public void DeleteAll_ShouldResetCartProperties()
            {
                Cart cart = new Cart();
                cart.AddOneToCart(new Chair());
                cart.DeleteAll();

                Assert.Multiple(() =>
                {
                    Assert.That(cart.Size, Is.EqualTo(0));
                    Assert.That(cart.Items.Count, Is.EqualTo(0));
                    Assert.That(cart.Chair_counter, Is.EqualTo(0));
                });
            }

            [Test]
            public void DeleteAll_EmptyCart_ThrowsException()
            {
                Cart cart = new Cart();
                var ex = Assert.Throws<Exception>(() => cart.DeleteAll());
                Assert.That(ex.Message, Is.EqualTo("Cannot restore empty cart!"));
            }

            [Test]
            public void Calculate_WrongDateFormat_ThrowsException()
            {
                Cart cart = new Cart();
                var ex = Assert.Throws<Exception>(() => cart.Calculate("30-04-2024")); // Pogrešan format (treba yyyy-MM-dd)
                Assert.That(ex.Message, Is.EqualTo("Wrong date format! Date must be in format yyyy-MM-dd"));
            }

            [Test]
            public void Calculate_InsufficientBudget_ThrowsException()
            {
                Cart cart = new Cart();
                cart.Budget = 100;
                Laptop laptop = new Laptop { Price = 1000 };
                cart.AddOneToCart(laptop);

                // Postavljamo datum u buducnosti (npr. sutra) da izbegnemo druge izuzetke
                string tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                var ex = Assert.Throws<Exception>(() => cart.Calculate(tomorrow));
                Assert.That(ex.Message, Is.EqualTo("Not enough budget!"));
            }

            [Test]
            public void Calculate_Apply5PercentDiscount_LaptopComputerChairCombo()
            {
                Cart cart = new Cart();
                cart.Budget = 5000;

                // Dodajemo 6 artikala da pređemo Size > 5
                cart.AddOneToCart(new Laptop { Price = 1000 });
                cart.AddOneToCart(new Computer { Price = 1000 });
                cart.AddOneToCart(new Chair { Price = 100 });
                cart.AddMultipleToCart(new Keyboard { Price = 10 }, 3);

                string deliveryDate = DateTime.Today.AddDays(2).ToString("yyyy-MM-dd");
                cart.Calculate(deliveryDate);

                // 2130 - 5% = 2023.5. Budzet: 5000 - 2023.5 = 2976.5
                Assert.That(cart.Budget, Is.EqualTo(2976.5));
            }

            [Test]
            public void Calculate_DeliveryToday_ThrowsException()
            {
                Cart cart = new Cart();
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                var ex = Assert.Throws<Exception>(() => cart.Calculate(today));
                Assert.That(ex.Message, Is.EqualTo("Date of delivery can't be today's date!"));
            }


            [Test]
            public void Calculate_Apply20PercentDiscount_ConditionMet()
            {
                Cart cart = new Cart();
                cart.Budget = 10000;

                // Uslov: Size > 8, Amount > 1500, Laptop >= 3, Monitor >= 3
                cart.AddMultipleToCart(new Laptop { Price = 500 }, 3);
                cart.AddMultipleToCart(new Monitor { Price = 200 }, 3);
                cart.AddMultipleToCart(new Chair { Price = 100 }, 3); // Ukupno 9 artikala, Amount = 2400

                // Pronalazenje datuma koji je radni dan i za 4-7 dana od danas
                DateTime futureDate = DateTime.Today.AddDays(5);
                if (futureDate.DayOfWeek == DayOfWeek.Saturday) futureDate = futureDate.AddDays(2);
                if (futureDate.DayOfWeek == DayOfWeek.Sunday) futureDate = futureDate.AddDays(1);

                cart.Calculate(futureDate.ToString("yyyy-MM-dd"));

                // 2400 - 20% = 1920. Budzet: 10000 - 1920 = 8080
                Assert.That(cart.Budget, Is.EqualTo(8080));
            }

            [Test]
            public void Print_EmptyCart_ThrowsException()
            {
                Cart cart = new Cart();
                var ex = Assert.Throws<Exception>(() => cart.Print());
                Assert.That(ex.Message, Is.EqualTo("Cannot print empty cart!"));
            }

    }
}

