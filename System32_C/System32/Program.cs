using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
var config = new ConsumerConfig {
    BootstrapServers = "localhost:9092" ,
    GroupId="order-consumer-group",
    AutoOffsetReset=AutoOffsetReset.Earliest
};

using var consumer=new ConsumerBuilder<Null,string>(config).Build();
consumer.Subscribe("order-topic");
var token=new CancellationTokenSource();
try
{
    while (true)
    {
        var resp=consumer.Consume(token.Token);
        if(resp.Message!=null)
        {
            Console.WriteLine(resp.Message.Value);
            // var record = JsonConvert.DeserializeObject(resp.Message.Value);
            var jsonObj = JObject.Parse(resp.Message.Value);
            var productValue = (string)jsonObj["product"];
            Console.WriteLine("Product: " + productValue);
            // Tạo một tiến trình cmd
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // Bắt đầu tiến trình
            process.Start();

            // Gửi lệnh cmd
            process.StandardInput.WriteLine(productValue);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            // Đọc kết quả trả về từ cmd
            var output = process.StandardOutput.ReadToEnd();

            // Chờ tiến trình kết thúc
            process.WaitForExit();

            // In kết quả
            Console.WriteLine(output);
        }
    }
}
catch (ConsumeException ex)
{
  Console.WriteLine(ex.Message);
}


public record Order(string product, int quantity);
