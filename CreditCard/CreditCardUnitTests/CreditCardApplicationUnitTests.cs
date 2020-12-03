using System.Security.Cryptography.X509Certificates;
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
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication {Age = 19};

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void Decline_low_income_applications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

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

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("12345");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        /*[Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);

            var isValid = true;

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            var decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }*/

        [Fact]
        public void Refer_when_license_key_expired()
        {
            // Mock property hierarchies 
            var mockLicenseData = new Mock<ILicenseData>();
            mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");

            var mockServiceInfo = new Mock<IServiceInformation>();
            mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);

            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication {Age = 42};

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
    }
}
