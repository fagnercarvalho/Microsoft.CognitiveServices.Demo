using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using static System.Console;

public class Program
{
    private const string ApiKey = "<YOUR API KEY>";

    private const string DescribeImageApiUrl = 
        "https://api.projectoxford.ai/vision/v1.0/describe?maxCandidates=3";

    private const string ContentType = "application/json";

    public static void Main(string[] args)
    {
        var imageUrls = new[] { "http://i.imgur.com/OJ6lzhz.jpg", "http://i.imgur.com/OV80Pr8.jpg" };

        foreach(var imageUrl in imageUrls)
        {
            var response = DescribeImage(imageUrl);

            WriteLine($"Image: {imageUrl}");
            WriteLine($"Descriptions:");

            foreach (var caption in response.Description.Captions)
            {
                WriteLine(caption.Text);
            }

            WriteLine(new string('-', 5));
        }

        Read();
    }

    private static DescribeImageResponse DescribeImage(string imageUrl)
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

        var imageJson = new { url = imageUrl };

        var response = client.PostAsync(
            DescribeImageApiUrl,
            new StringContent(
                JsonConvert.SerializeObject(imageJson),
                Encoding.UTF8, ContentType)).Result;

        var content = response.Content.ReadAsStringAsync().Result;

        return JsonConvert.DeserializeObject<DescribeImageResponse>(content);
    }

    private class DescribeImageResponse
    {
        public DescribeImageResponseDescription Description { get; set; }

        public string RequestId { get; set; }

        public DescribeImageResponseMetadata Metadata { get; set; }
    }

    private class DescribeImageResponseDescription
    {
        public List<string> Tags { get; set; }

        public List<DescribeImageResponseDescriptionCaption> Captions { get; set; }
    }

    private class DescribeImageResponseDescriptionCaption
    {
        public string Text { get; set; }

        public decimal Confidence { get; set; }
    }

    private class DescribeImageResponseMetadata
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public string Format { get; set; }
    }
}