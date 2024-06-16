using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace DeliverySystem.Helpers
{
    public class TwilioHelper
    {
        public static void SendMessage(string body, string phoneNumber)
        {
            const string accountSid = "ACd02a363c424ba3e89eb690656bc1f470";
            const string authToken = "c4bbe091c2341b5b1822ab167825c181";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber("+16504899406"),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
        }
    }
}
