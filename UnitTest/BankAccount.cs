using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImpromptuInterface;
using Moq;
using Xunit;

//https://xunit.net/docs/comparisons
namespace UnitTest
{

    public interface ILog
    {
        void WriteMessage(string msg);
    }

    public class ConsoleLog : ILog
    {
        public void WriteMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public class Null<T> : DynamicObject where T : class
    {

        public static T Instance => new Null<T>().ActLike<T>();

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            result = default;

            if (typeof(T).GetMethod(binder.Name)?.ReturnType == typeof(void))
                return true;

            Activator.CreateInstance(typeof(T).GetMethod(binder.Name)?.ReturnType ?? null);
            return true;
        }
    }

    public class BankAccount
    {
        public int Balance { get; set; }
        private readonly ILog _log;
        public BankAccount(int balance, ILog log)
        {
            Balance = balance;
            _log = log;
        }

        public void Deposit(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Deposit Amount Must Be Positive");

            _log.WriteMessage("Depositing...");
            Balance += amount;
        }

        public bool Withdraw(int amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                return true;
            }

            return false;
        }

    }


    public class BankAccountTest
    {
        private BankAccount ba;

        public BankAccountTest()
        {
            var log = new Mock<ILog>();
            ba = new BankAccount(50, log.Object);
        }



        [Fact]

        public void BankAccountShouldIncreaseOnPositiveDeposit()
        {

            ba.Deposit(100);

            Assert.Equal(150, ba.Balance);
        }

       


        [Fact]
        public void BankAccountShouldThrowUnPositiveAmount()
        {

            var ex = Assert.Throws<ArgumentException>((() => ba.Deposit(-1)));
            Assert.StartsWith("Deposit Amount Must Be Positive", ex.Message);
        }

        [Theory]
        [InlineData(10, true, 40)]
        [InlineData(100, false, 50)]
        public void TestMultipleWithdrawScenario(int amount, bool shouldSucceed, int expectedBalance)
        {
            var result = ba.Withdraw(amount);

            Assert.Equal(shouldSucceed, result);
            Assert.Equal(expectedBalance, ba.Balance);
        }

        [Fact]
        public void DepositIntegrationTest()
        {
            ba.Deposit(100);

            Assert.Equal(150,ba.Balance);
        }

        [Fact]
        public void Test()
        {
            // var _actualFuel = 2 + 2;
            //   Assert.Inconclusive("The Result is not Set");
            // Assert.Warn("The Result is not Set");
            //   Assert.AreEqual(a,4);
            // Assert.That(_actualFuel, Is.EqualTo(4));

            //Assert.That(28, Is.EqualTo(_actualFuel)); // Tests whether the specified values are equal. 
            //Assert.That(28, Is.Not.EqualTo(_actualFuel)); // Tests whether the specified values are unequal. Same as AreEqual for numeric values.
            //Assert.That(_expectedRocket, Is.SameAs(_actualRocket)); // Tests whether the specified objects both refer to the same object
            //Assert.That(_expectedRocket, Is.Not.SameAs(_actualRocket)); // Tests whether the specified objects refer to different objects
            //Assert.That(_isThereEnoughFuel, Is.True); // Tests whether the specified condition is true
            //Assert.That(_isThereEnoughFuel, Is.False); // Tests whether the specified condition is false
            //Assert.That(_actualRocket, Is.Null); // Tests whether the specified object is null
            //Assert.That(_actualRocket, Is.Not.Null); // Tests whether the specified object is non-null
            //Assert.That(_actualRocket, Is.InstanceOf<Falcon9Rocket>()); // Tests whether the specified object is an instance of the expected type
            //Assert.That(_actualRocket, Is.Not.InstanceOf<Falcon9Rocket>()); // Tests whether the specified object is not an instance of type
            //Assert.That(_actualFuel, Is.GreaterThan(20)); // Tests whether the specified object greater than the specified value
            //Assert.That(28, Is.EqualTo(_actualFuel).Within(0.50));
            //// Tests whether the specified values are nearly equal within the specified tolerance.
            //Assert.That(28, Is.EqualTo(_actualFuel).Within(2).Percent);
            //// Tests whether the specified values are nearly equal within the specified % tolerance.
            //Assert.That(_actualRocketParts, Has.Exactly(10).Items);
            //// Tests whether the specified collection has exactly the stated number of items in it.
            //Assert.That(_actualRocketParts, Is.Unique);
            //// Tests whether the items in the specified collections are unique.
            //Assert.That(_actualRocketParts, Does.Contain(_expectedRocketPart));
            //// Tests whether a given items is present in the specified list of items.
            //Assert.That(_actualRocketParts, Has.Exactly(1).Matches<RocketPart>(part => part.Name == "Door" && part.Height == "200"));
            //// Tests whether the specified collection has exactly the stated item in it.


            //StringAssert.AreEqualIgnoringCase(_expectedBellatrixTitle, "Bellatrix"); // Tests whether the specified strings are equal ignoring their casing
            //StringAssert.Contains(_expectedBellatrixTitle, "Bellatrix"); // Tests whether the specified string contains the specified substring
            //StringAssert.DoesNotContain(_expectedBellatrixTitle, "Bellatrix"); // Tests whether the specified string doesn't contain the specified substring
            //StringAssert.StartsWith(_expectedBellatrixTitle, "Bellatrix"); // Tests whether the specified string begins with the specified substring
            //StringAssert.StartsWith(_expectedBellatrixTitle, "Bellatrix"); // Tests whether the specified string begins with the specified substring
            //StringAssert.IsMatch("(281)388-0388", @"(?d{3})?-? *d{3}-? *-?d{4}"); // Tests whether the specified string matches a regular expression
            //StringAssert.DoesNotMatch("281)388-0388", @"(?d{3})?-? *d{3}-? *-?d{4}"); // Tests whether the specified string does not match a regular expression

        }

    }
}
