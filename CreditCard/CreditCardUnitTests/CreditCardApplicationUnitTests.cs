using CreditCard;
using Moq;
using Xunit;

namespace CreditCardUnitTests
{
    public class CreditCardApplicationUnitTests
    {
        [Fact]
        public void Accept_high_income_applications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication{GrossAnnualIncome = 100_000};

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void Refer_young_applicants()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication {Age = 19};

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void Decline_low_income_applications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            // returns true when the specific string "x" is passed to isValid method
            mockValidator.Setup(x => x.IsValid("x")).Returns(true);

            // returns true when any string is passed to the isValid method
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            
            // returns true when any string is passed to the isValid method that begins with "x"
            mockValidator.Setup(x => 
                    x.IsValid(It.Is<string>(x => x.StartsWith("x"))))
                .Returns(true);

            // returns true when any string is passed to the isValid method that is between a-z
            mockValidator.Setup(x =>
                x.IsValid(It.IsInRange("a", "z", Range.Inclusive))).Returns(true);

            // returns true when the string is either "x" or "z"
            mockValidator.Setup(x =>
                x.IsValid(It.IsIn("x", "z"))).Returns(true);

            // return true when a regex is satisfied
            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
    }
}
