using System;
using ConsoleGame;
using Xunit;

namespace ConsoleGameUnitTests
{
    public class CreditCardApplicationUnitTests
    {
        [Fact]
        public void Accept_high_income_applications()
        {
            var sut = new CreditCardApplicationEvaluator();

            var application = new CreditCardApplication{GrossAnnualIncome = 100_000};

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }
    }
}
