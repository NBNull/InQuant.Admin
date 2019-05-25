using InQuant.Authorization.TwoFactor;
using InQuant.Framework.Utils;
using System;
using System.Threading;
using Xunit;

namespace InQuant.Framework.Tests.Security
{
    public class TwoFactorAuthenticatorTest
    {
        [Fact]
        public void TestValidateTwoFactorPIN()
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string securityKey = RandomUtil.GetUniqueStr(10);
            string email = "test.bcsystech.com";

            var setupInfo = tfa.GenerateSetupCode("hopex.com", email, securityKey, 200, 200);

            Console.WriteLine(setupInfo.QrCodeSetupImageUrl);
            Console.WriteLine(setupInfo.ManualEntryKey);

            string pin = tfa.GetCurrentPIN(securityKey);

            Thread.Sleep(10 * 1000);
            bool ok = tfa.ValidateTwoFactorPIN(securityKey, tfa.GetCurrentPIN(securityKey));
            Assert.True(ok);
        }
    }
}
