using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Threading.Tasks;

namespace Reciever
{
    class Program
    {

        static string accessKey = "";
        static string secretKey = "";
        static string queueOwnerAWSAccountId = "";
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'q' to exit and any other to continue reading");
            var client = InitAWSSQSClient();
            while (Console.ReadKey().KeyChar != 'q')
            {
                var response = RecieveMessageFromQueue(client);
                if (response.Result == false)
                {
                    Console.WriteLine("Nothing to read for now");
                }
                Console.WriteLine("Press a key");
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

        async static private Task<bool> RecieveMessageFromQueue(AmazonSQSClient client)
        {
            try
            {
                var queueUrl = await GetQueueUrl(client);
                var response = await client.ReceiveMessageAsync(queueUrl);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK && response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        Console.WriteLine(message.Body);
                        await client.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }

}
