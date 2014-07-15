using Pna.core;
using Pna.core.Eumn;
using Pna.Service.Interface;
using PnaAPI.Models;
using PnaAPI.Models.paramModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PnaAPI.Controllers
{
    public class MailController : ApiController
    {

        private IMail _Imail;
        public MailController(IMail Imail)
        {
            _Imail = Imail;
        }


        /// <summary>
        /// 获取第一页内容
        /// </summary>
        /// <returns></returns>
        [ActionName("GetMainMail")]
        [HttpPost]
        public object GetItemInfo1(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                IOrderedEnumerable<Mail_Link_User> result;
                if (model.userId > 0)
                {
                    result = _Imail.GetMaillinkUser().Where(n => n.UserID == model.userId).OrderByDescending(n => n.CreateTime);
                }
                else
                {
                    //    result = _Imail.GetMaillinkUser().Where(n => n.ID == 1).OrderByDescending(n => n.CreateTime);
                    var re = _Imail.GetMailMailEt();

                    dic.Add("ReturnData", new
                    {
                        Message = re.Select(n => new
                        {
                            ID = n.ID,
                            MainMail = n.MainName,
                            MailDescription = n.Des,
                            LinkCreater = n.Creater,
                            LinkCreateCompay = n.CreateCompay,
                            LinkEMail = n.Mail,
                            LinkPhone = n.phone,
                            Cover = n.Conver,
                            StatusID = 1
                        })

                    ,
                        Code = "ok"
                    });

                    return dic;
                }

                List<MainMail> listresult = new List<MainMail>();
                Dictionary<int, int> dicfind = new Dictionary<int, int>();
                foreach (var item in result)
                {
                    if (item.StatusID == (int)E_Status.已审核 || item.StatusID == (int)E_Status.未核实)
                    {
                        listresult.Add(item.MainMail);
                        dicfind.Add(item.ID, (int)item.StatusID);
                    }
                }

                dic.Add("ReturnData", new
                {
                    Message = listresult.OrderByDescending(n => n.OrderID).Select(n => new
                {
                    ID = n.ID,
                    MainMail = n.MainName,
                    MailDescription = n.Des,
                    LinkCreater = n.Creater,
                    LinkCreateCompay = n.CreateCompay,
                    LinkEMail = n.Mail,
                    LinkPhone = n.phone,
                    Cover = n.Conver,
                    StatusID = dicfind[n.ID]
                })

                ,
                    Code = "ok"
                });


            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }


        /// <summary>
        /// 获取详细类型
        /// </summary>
        /// <returns></returns>
        [ActionName("GetCategory")]
        [HttpPost]
        public object GetItemInfo2(MailModel model)
        {


            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var result = _Imail.GetCategory().Where(n => n.ID == model.mainId).OrderByDescending(n => n.CreateTime);
                dic.Add("ReturnData", new
                {
                    Message = result.Select(n => new
                    {
                        CategoryName = n.CategoryName,
                        CategoryID = n.CategoryID
                    })
                ,
                    Code = "ok"
                });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }

        /// <summary>
        /// 获取详细类型
        /// </summary>
        /// <returns></returns>
        [ActionName("GetMailList")]
        [HttpPost]
        public object GetItemInfo3(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var result = _Imail.GetMaillist().Where(n => n.CategoryID == model.categoryId);


                //搜索条件
                if (!string.IsNullOrEmpty(model.keyWord))
                {
                    result = result.Where(n => n.KeyWord == model.keyWord);
                }
                var h = result.OrderByDescending(n => n.CreateTime).Select(n => new
                {
                    MaillD = n.MaillD,
                    CategoryID = n.CategoryID,
                    MailName = n.MailName,
                    MialIco = n.MialIco,
                    CreateTime = n.CreateTime,
                    phone = n.phone
                });
                dic.Add("ReturnData", new { Message = h, Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }


        /// <summary>
        /// 置顶
        /// </summary>
        /// <returns></returns>
        [ActionName("SetTop")]
        [HttpPost]
        public object GetItemInfo4(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                _Imail.SetTop(model.mainId);
                dic.Add("ReturnData", new { Message = "成功", Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }

        /// <summary>
        /// 删除关联表信息
        /// </summary>
        /// <returns></returns>
        [ActionName("DelMailLinkUser")]
        [HttpPost]
        public object GetItemInfo5(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                _Imail.DelLinkUser(model.userId, model.mainId);
                dic.Add("ReturnData", new { Message = "成功", Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }

        /// <summary>
        /// 获取未审核列表
        /// </summary>
        /// <returns></returns>
        [ActionName("GetUnauditedMailList")]
        [HttpPost]
        public object GetItemInfo6(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                var result = _Imail.Unaudited(model.userId);
                dic.Add("ReturnData", new { Message = result, Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }

        /// <summary>
        /// 审核电话博
        /// </summary>
        /// <returns></returns>
        [ActionName("AuditMainMail")]
        [HttpPost]
        public object GetItemInfo7(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                _Imail.AuditwMainMail(model.userId, model.mainId);
                dic.Add("ReturnData", new { Message = "成功", Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }


        /// <summary>
        /// 获取本人或者其他人创建的电话薄
        /// </summary>
        /// <returns></returns>
        [ActionName("GetOtherMainMail")]
        [HttpPost]
        public object GetItemInfo10(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                //获取所有的已经审核的电话薄
                var result = _Imail.GetMailMailExt(model.userId);

                if (!string.IsNullOrEmpty(model.keyWord))
                {
                    result = (from p in result
                              where p.MainName.Contains(model.keyWord) || p.Des.Contains(model.keyWord)
                              select p).ToList();
                }
                else
                {
                    dic.Add("ReturnData", new { Message = "请输入搜索条件", Code = "E_00010" });
                    return dic;
                }



                dic.Add("ReturnData", new
                {
                    Message = result.OrderByDescending(n => n.OrderID).Select(n => new
                    {
                        ID = n.ID,
                        MainMail = n.MainName,
                        MailDescription = n.Des,
                        LinkCreater = n.Creater,
                        LinkCreateCompay = n.CreateCompay,
                        LinkEMail = n.Mail,
                        LinkPhone = n.phone,
                        Cover = n.Conver,

                    })

                ,
                    Code = "ok"
                });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }



        /// <summary>
        /// 将电话薄添加到自己目录列表中
        /// </summary>
        /// <returns></returns>
        [ActionName("AddUserLinkMainMail")]
        [HttpPost]
        public object GetItemInfo11(MailModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                _Imail.SaveLinkUser(model.userId, model.mainId);
                dic.Add("ReturnData", new { Message = "成功", Code = "ok" });
            }
            catch
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = "出现错误，请查看日志", State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }

    }
}