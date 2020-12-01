using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace apisample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }



        [HttpGet("download")]
        public async Task<IActionResult> DownLoad()
        {
	        var filePath = @"D:\Download\测试文档.xlsx";   //要下载的文件地址，这个文件会被分成片段，通过循环逐步读取到ASP.NET Core中，然后发送给客户端浏览器
	        var fileName = Path.GetFileName(filePath); //测试文档.xlsx

	        int bufferSize = 1024;
	        Response.ContentType = "application/octet-stream";
	        var contentDisposition = "attachment;" + "filename=" + HttpUtility.UrlEncode(fileName);//在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名，注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
	        Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });
	        //使用FileStream开始循环读取要下载文件的内容
	        await using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
	        {
		        await using (Response.Body)//调用Response.Body.Dispose()并不会关闭客户端浏览器到ASP.NET Core服务器的连接，之后还可以继续往Response.Body中写入数据
		        {
			        long contentLength = fs.Length;         //获取下载文件的大小
			        Response.ContentLength = contentLength; //在Response的Header中设置下载文件的大小，这样客户端浏览器才能正确显示下载的进度
			        byte[] buffer;
			        long hasRead = 0; //变量hasRead用于记录已经发送了多少字节的数据到客户端浏览器
			        //如果hasRead小于contentLength，说明下载文件还没读取完毕，继续循环读取下载文件的内容，并发送到客户端浏览器
			        while (hasRead < contentLength)
			        {
				        //HttpContext.RequestAborted.IsCancellationRequested可用于检测客户端浏览器和ASP.NET Core服务器之间的连接状态，如果HttpContext.RequestAborted.IsCancellationRequested返回true，说明客户端浏览器中断了连接
				        if (HttpContext.RequestAborted.IsCancellationRequested)
				        {
					        //如果客户端浏览器中断了到ASP.NET Core服务器的连接，这里应该立刻break，取消下载文件的读取和发送，避免服务器耗费资源
					        break;
				        }
				        buffer = new byte[bufferSize];
				        int currentRead = fs.Read(buffer, 0, bufferSize); //从下载文件中读取bufferSize(1024字节)大小的内容到服务器内存中
				        await Response.Body.WriteAsync(buffer, 0, currentRead);      //发送读取的内容数据到客户端浏览器
				        await Response.Body.FlushAsync();                            //注意每次Write后，要及时调用Flush方法，及时释放服务器内存空间
				        hasRead += currentRead; //更新已经发送到客户端浏览器的字节数
			        }
		        }
	        }
	        return new EmptyResult();
        }

        [HttpGet("download2")]
		public async Task<IActionResult> Download2()
		{
			//byte[] array = Encoding.UTF8.GetBytes(ToCSV(dt));
			int bufferSize = 1024;
			Response.ContentType = "application/octet-stream";
			var contentDisposition = "attachment;" + "filename=" + HttpUtility.UrlEncode("fileName.csv");//在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名，注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
			Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });
			var header=string.Join(",",new string[]{"111","222","333"})+"\n";
		
			await using (Response.Body)
			{
				byte[] headerBytes = Encoding.UTF8.GetBytes(header);
				//Response.ContentLength = headerBytes.Length;
				await Response.Body.WriteAsync(headerBytes);  
				await Response.Body.FlushAsync();  
				var body1=string.Join(",",new string[]{"a",Guid.NewGuid().ToString(),Path.GetRandomFileName()})+"\n";
				var add=new List<string>{body1};
				for (int i = 0; i < 4000000; i++)
				{
					add.Add(body1);
				}
				foreach (var item in add)
				{
					if (HttpContext.RequestAborted.IsCancellationRequested)
					{
						//如果客户端浏览器中断了到ASP.NET Core服务器的连接，这里应该立刻break，取消下载文件的读取和发送，避免服务器耗费资源
						break;
					}
					headerBytes=Encoding.UTF8.GetBytes(item);
					//Response.ContentLength += headerBytes.Length;
					await Response.Body.WriteAsync(headerBytes);  
					await Response.Body.FlushAsync();  
				}
			}
			return new EmptyResult();
		}


        private static string ToCSV(DataTable table)
        {
	        var result = new StringBuilder();
	        for (int i = 0; i < table.Columns.Count; i++)
	        {
		        if (table.Columns[i].ColumnName.ToUpper()=="ID")
		        {
			        result.Append("主键");
		        }
		        else
		        {
			        result.Append(table.Columns[i].ColumnName);
		        }

		        result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
	        }

	        foreach (DataRow row in table.Rows)
	        {
		        for (int i = 0; i < table.Columns.Count; i++)
		        {
			        result.Append(row[i].ToString() == "" ? "-" : row[i].ToString());
			        result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
		        }
	        }
	        return result.ToString();
        }


    }
}
