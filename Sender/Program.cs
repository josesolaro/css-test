using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Threading.Tasks;

namespace Sender
{
    class Program
    {
        static string accessKey = "";
        static string secretKey = "";
        static string queueOwnerAWSAccountId = "";
        static void Main(string[] args)
        {
            var awsClient = InitAWSSQSClient();

            string message = "Insert the message you want to send, if you want to exit press enter\n";
            while (message.Length != 0)
            {
                Console.WriteLine(message);
                message = Console.ReadLine();
                if (message.Length != 0)
                {
                    var httpResponse = SendMessageToQueue(awsClient, message);
                    if (httpResponse.Result != System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Fail To send Message. Exiting...");
                        message = "";
                    }
                }
            }
        }

        static private AmazonSQSClient InitAWSSQSClient()
        {
            var sqsConfig = new AmazonSQSConfig();

            sqsConfig.ServiceURL = "http://sqs.us-east-2.amazonaws.com";

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

            var client = new AmazonSQSClient(credentials, sqsConfig);

            return client;
        }

        async static private Task<string> GetQueueUrl(AmazonSQSClient client)
        {
            var request = new GetQueueUrlRequest
            {
                QueueName = "SCS-test-project",
                QueueOwnerAWSAccountId = queueOwnerAWSAccountId
            };
            var response = await client.GetQueueUrlAsync(request);
            return response.QueueUrl;
        }

        async static private Task<System.Net.HttpStatusCode> SendMessageToQueue(AmazonSQSClient client, string message)
        {
            try
            {
                var queueUrl = await GetQueueUrl(client);

                var messageRequest = new SendMessageRequest(queueUrl, message);
                var response = await client.SendMessageAsync(messageRequest);
                return response.HttpStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }
    }
}
