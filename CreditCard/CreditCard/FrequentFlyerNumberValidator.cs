using System;

namespace CreditCard
{
    class FrequentFlyerNumberValidator : IFrequentFlyerNumberValidator
    {
        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException("hard to use");
        }
        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException("Yes, it is hard to use");
        }

        public string LicenseKey => throw new NotImplementedException("For demo purposes");
    }
}
