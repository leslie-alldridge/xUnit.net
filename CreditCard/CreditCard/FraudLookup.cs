﻿namespace CreditCard
{
    public class FraudLookup
    {
        public virtual bool IsFraudRisk(CreditCardApplication application)
        {
            if (application.LastName == "Smith")
            {
                return true;
            }

            return false;
        }
    }
}
