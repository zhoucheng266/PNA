using PnaAPI.Models;
using PnaAPI.Models.paramModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Text;

namespace PnaAPI.Controllers
{
    public class UpLoadController : ApiController
    {
        [ActionName("UpFile")]
        [HttpPost]
        public object GetItemInfo(FileModel model)
        {


            var buffer = Convert.FromBase64String(model.fileString.Replace(' ', '+'));


            //model.FileByte = buffer;
            //判断用户是否属于
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {

                if (!string.IsNullOrEmpty(model.pathMame))
                {
                    model.pathMame = "mylogo";
                }

                var savepath = string.Format(@"~/Identify/{0}/{1}/", model.pathMame, model.userId);


                var fullFileSavePath = System.Web.Hosting.HostingEnvironment.MapPath(savepath);

                if (buffer.Length == 0) return 0;
                //创建文件夹
                if (!Directory.Exists(fullFileSavePath))
                    Directory.CreateDirectory(fullFileSavePath);

                //定义并实例化一个内存流
                var MemoryStream = this.ConvertFileToMemoryStream(buffer);
                //保存的图片名称 时间+后缀名
                string fileName = DateTime.Now.ToFileTime() + ".png";

                FileStream fs = new FileStream(fullFileSavePath + fileName, FileMode.Create);//创建文件  
                MemoryStream.WriteTo(fs);
                MemoryStream.Close(); fs.Close();

                dic.Add("ReturnInfo", new ReturnInfo { Message = "ok", State = 1 });
             //   dic.Add("ReturnData", new { FileName = fileName, FileUrl = "sasasa" });

                dic.Add("ReturnData", new { Message = new { FileName = fileName, FileUrl = "sasasa" }, Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", null);
            }
            return dic;
        }

        /// <summary>
        /// 将流转换成为字节数组
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public MemoryStream ConvertFileToMemoryStream(byte[] filebyte)//把stream转换成byte[]数组  
        {

            //临时数据流
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream(filebyte);

            return tempStream;
        }
    }
}
