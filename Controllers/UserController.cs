using Pna.core;
using Pna.core.DTO;
using Pna.Service.Interface;
using PnaAPI.Models;
using PnaAPI.Models.paramModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PnaAPI.Controllers
{

    //User模块
    public class UserController : ApiController
    {

        private IUser _userservice;
        public UserController(IUser userservice)
        {
            _userservice = userservice;
        }

        [ActionName("Login")]
        [HttpPost]
        public object GetItemInfo1(UserModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                //     var result = _shopService.ValidateUserInfo(string.Empty, WebUtility.UrlDecode(model.UserName), string.Empty);
                var result = _userservice.GetAllUsers();
                //首先是手机
                model.phone = string.IsNullOrEmpty(model.phone) ? string.Empty : model.phone;
                var entity = result.Where(n => n.UserName == HttpUtility.UrlDecode(model.phone) && n.PassWord == Untity.GetMd5(model.passWord)).FirstOrDefault();



                UserEtDTO entity1 = null;
                if (entity == null)
                {
                    var resultet = _userservice.GetAllUsersEt();
                    entity1 = resultet.Where(n => n.Phone == HttpUtility.UrlDecode(model.phone) && n.PassWord == Untity.GetMd5(model.passWord)).FirstOrDefault();

                }
                //     var entity1 = result.Where(n => n.UserValidate.Phone == HttpUtility.UrlDecode(model.phone) && n.PassWord == Untity.GetMd5(model.passWord)).FirstOrDefault();
                if (entity != null)
                {
                    dic.Add("ReturnData", new
                    {
                        Message = new
                            {
                                UserID = entity.UserID,
                                UserName = entity.UserName,
                                UserCode = entity.UserCode,
                                Phone = entity.UserValidate.Phone,
                                QQ = entity.UserValidate.QQ,
                                RealName = entity.RealName,
                                Logo = entity.UserValidate.Logo,
                            },
                        Code = "ok"
                    });
                }
                else if (entity1 != null)
                {
                    dic.Add("ReturnData", new
                    {
                        Message = new
                            {
                                UserID = entity1.UserID,
                                UserName = entity1.UserName,
                                UserCode = entity1.UserCode,
                                Phone = entity1.Phone,
                                QQ = entity1.QQ,
                                RealName = entity1.RealName,
                                Logo = entity1.Logo,
                            },
                        Code = "ok"
                    });
                }
                else
                {
                    dic.Add("ReturnData", new { Message = "登录不成功", Code = "E_10010" });


                }

            }
            catch (Exception ee)
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = ee.ToString(), State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }





        [ActionName("UpdatePwd")]
        [HttpPost]
        public object GetItemInfo2(UserModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var entity = _userservice.GetAllUsers().Where(n => n.UserID == model.userId);
                if (entity != null)
                {
                    _userservice.UpdatePwd(model.userId, Untity.GetMd5(model.newpwd));
                    dic.Add("ReturnData", new { Message = "成功", Code = "ok" });

                }
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
        /// 发送短信
        /// </summary>
        [ActionName("SendSMS")]
        [HttpPost]
        public object GetItemInfo3(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                var smsstr = Untity.ramNUM();
                var SmsContent = String.Format("您的验证码是{0},请于10分钟内输入。【工作人员不会向您索取，请勿泄露】", smsstr);

                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                //验证常用的手机号码
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.phone, @"^1(3[0-9]|5[0123456789]|8[0123456789]|4[57])\d{8}$"))
                {
                    //手机号码验证
                    dic.Add("ReturnData", new { Message = "手机验证不正确", Code = "E_10004" });
                    return dic;
                }
                var resultvalidate = _userservice.GetUserValidateList().Where(n => n.Phone == model.phone).FirstOrDefault();
                //首先要验证电话号码是否唯一
                if (resultvalidate != null)
                {
                    //已经存在相同的号码了
                    dic.Add("ReturnData", new { Message = "号码已经被注册了", Code = "E_10008" });
                    return dic;
                }


                //发短信
                if (true)//SMSUtility.SendSMS(model.PhoneNum, SmsContent))
                {
                    //需要回写一些内容（wcf验证）

                    //写入smskey 记录
                    _userservice.SaveSmsKey(new SmsKey { ActiveTime = DateTime.Now, ExpireTime = DateTime.Now.AddMinutes(10), Phone = model.phone, SmsStr = smsstr });
                    dic.Add("ReturnData", new { Message = smsstr, Code = "ok" });


                }
                else
                {
                    //发送短信失败
                    dic.Add("ReturnData", new { Message = "发送短信失败", Code = "E_10007" });
                }



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
        /// 校验验证码
        /// </summary>
        [ActionName("GetValidateNum")]
        [HttpPost]
        public object GetItemInfo4(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {


                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                //验证常用的手机号码
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.phone, @"^1(3[0-9]|5[0123456789]|8[0123456789]|4[57])\d{8}$"))
                {
                    //手机号码验证
                    dic.Add("ReturnData", new { Message = "手机验证不正确", Code = "E_10004" });
                    return dic;
                }
                var result = _userservice.GetAllSmsKey(model.phone).OrderByDescending(n => n.ActiveTime).FirstOrDefault();

                if (result == null)
                {
                    dic.Add("ReturnData", new { Message = "验证码不存在", Code = "E_10005" });
                    return dic;
                }
                else
                {

                    //没有过期
                    if (result.ExpireTime > DateTime.Now)
                    {

                        if (model.smskey == result.SmsStr)
                        {

                            _userservice.UpdateSmsKey(new SmsKey { ID = result.ID });
                            dic.Add("ReturnData", new { Message = "成功", Code = "ok" });


                            return dic;
                        }
                        else
                        {
                            dic.Add("ReturnData", new { Message = "验证码错误", Code = "E_10005" });
                            return dic;
                        }

                    }
                    else
                    {
                        dic.Add("ReturnData", new { Message = "验证码过期", Code = "E_10006" });
                        return dic;
                    }
                }

            }
            catch (Exception ee)
            {
                dic.Clear();
                dic.Add("ReturnInfo", new ReturnInfo { Message = ee.ToString(), State = -99 });
                dic.Add("ReturnData", -99);
            }
            return dic;
        }


        /// <summary>
        /// 通过手机找回密码
        /// </summary>
        [ActionName("FindPwdwithPhone")]
        [HttpPost]
        public object GetItemInfo5(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {


                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                //验证常用的手机号码
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.phone, @"^1(3[0-9]|5[0123456789]|8[0123456789]|4[57])\d{8}$"))
                {
                    //手机号码验证
                    dic.Add("ReturnData", new { Message = "手机验证不正确", Code = "E_10004" });
                    return dic;
                }
                var result = _userservice.GetAllSmsKey(model.phone).OrderByDescending(n => n.ActiveTime).FirstOrDefault();

                if (result == null)
                {
                    dic.Add("ReturnData", new { Message = "验证码不存在", Code = "E_10005" });
                    return dic;
                }
                else
                {

                    //没有过期
                    if (result.ExpireTime > DateTime.Now)
                    {

                        if (model.smskey == result.SmsStr)
                        {
                            //dic.Add("ReturnData", new { Message = result.SmsStr, Code = "ok" });
                            //return dic;
                            var resultvalidate = _userservice.GetUserValidateList().Where(n => n.Phone == model.phone).FirstOrDefault();
                            //修改

                            if (resultvalidate != null)
                            {
                                _userservice.UpdatePwd((int)resultvalidate.UserID, Untity.GetMd5(model.newpwd));
                                dic.Add("ReturnData", new { Message = "成功", Code = "ok" });
                            }

                        }
                        else
                        {
                            dic.Add("ReturnData", new { Message = "验证码错误", Code = "E_10005" });
                            return dic;
                        }





                    }
                    else
                    {
                        dic.Add("ReturnData", new { Message = "验证码过期", Code = "E_10006" });
                        return dic;
                    }
                }

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
        /// 发送短信用于找回密码
        /// </summary>
        [ActionName("SendSMSWithFindPwd")]
        [HttpPost]
        public object GetItemInfo6(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                var smsstr = Untity.ramNUM();
                var SmsContent = String.Format("你正在修改密码，验证码是{0},请于10分钟内输入。【工作人员不会向您索取，请勿泄露】", smsstr);

                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                //验证常用的手机号码
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.phone, @"^1(3[0-9]|5[0123456789]|8[0123456789]|4[57])\d{8}$"))
                {
                    //手机号码验证
                    dic.Add("ReturnData", new { Message = "手机验证不正确", Code = "E_10004" });
                    return dic;
                }


                //发短信
                if (true)//SMSUtility.SendSMS(model.PhoneNum, SmsContent))
                {

                    //写入smskey 记录
                    _userservice.SaveSmsKey(new SmsKey { ActiveTime = DateTime.Now, ExpireTime = DateTime.Now.AddMinutes(10), Phone = model.phone, SmsStr = smsstr });
                    dic.Add("ReturnData", new { Message = smsstr, Code = "ok" });


                }
                else
                {
                    //发送短信失败
                    dic.Add("ReturnData", new { Message = "发送短信失败", Code = "E_10007" });
                }



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
        /// 发送短信用于找回密码
        /// </summary>
        [ActionName("UpdateFindPwd")]
        [HttpPost]
        public object GetItemInfo7(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {


                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                //验证常用的手机号码
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.phone, @"^1(3[0-9]|5[0123456789]|8[0123456789]|4[57])\d{8}$"))
                {
                    //手机号码验证
                    dic.Add("ReturnData", new { Message = "手机验证不正确", Code = "E_10004" });
                    return dic;
                }


                var result = _userservice.GetAllSmsKey(model.phone).OrderByDescending(n => n.ActiveTime).FirstOrDefault();

                if (result == null)
                {
                    dic.Add("ReturnData", new { Message = "验证码不存在", Code = "E_10005" });
                    return dic;
                }
                else
                {

                    //没有过期
                    if (result.ExpireTime > DateTime.Now)
                    {

                        if (model.smskey == result.SmsStr)
                        {


                            var resultvalidate = _userservice.GetUserValidateList().Where(n => n.Phone == model.phone).FirstOrDefault();
                            //首先要验证电话号码是否唯一
                            if (resultvalidate != null)
                            {
                                //已经存在相同的号码了
                                dic.Add("ReturnData", new { Message = "成功", Code = "ok" });
                                _userservice.UpdatePwd((int)resultvalidate.UserID, Untity.GetMd5(model.newpwd));
                                return dic;
                            }


                        }
                        else
                        {
                            dic.Add("ReturnData", new { Message = "验证码错误", Code = "E_10005" });
                            return dic;
                        }

                    }
                    else
                    {
                        dic.Add("ReturnData", new { Message = "验证码过期", Code = "E_10006" });
                        return dic;
                    }
                }




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
        /// 添加用户名
        /// </summary>
        [ActionName("AddUser")]
        [HttpPost]
        public object GetItemInfo8(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {


                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                //验证常用的手机号码
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.phone, @"^1(3[0-9]|5[0123456789]|8[0123456789]|4[57])\d{8}$"))
                {
                    //手机号码验证
                    dic.Add("ReturnData", new { Message = "手机验证不正确", Code = "E_10004" });
                    return dic;
                }

                var resultvalidate = _userservice.GetUserValidateList().Where(n => n.Phone == model.phone).FirstOrDefault();
                //首先要验证电话号码是否唯一
                if (resultvalidate != null)
                {
                    //已经存在相同的号码了
                    dic.Add("ReturnData", new { Message = "号码已经被注册了", Code = "E_10008" });
                    return dic;
                }


                _userservice.SaveUser(new User { UserName = "p" + model.phone, PassWord = Untity.GetMd5(model.newpwd) });

                var result = _userservice.GetAllUsers().Where(n => n.UserName == "p" + model.phone).FirstOrDefault();

                _userservice.SaveUserValidate(new UserValidate { UserID = result.UserID, Phone = model.phone });

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
        /// 获取我的好友列表
        /// </summary>
        [ActionName("GetMyFriends")]
        [HttpPost]
        public object GetItemInfo9(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var list = _userservice.GetMyFriends(model.userId).AsEnumerable();
                //过滤条件
                if (!string.IsNullOrEmpty(model.keyWord))
                {
                    list = list.Where(n => n.UserName.Contains(model.keyWord));
                }

                dic.Add("ReturnData", new
                 {
                     Message = list.Select(n => new
                         {
                             UserID = n.UserID,
                             UserName = n.UserName,
                             UserCode = n.UserCode,
                             Phone = n.Phone,
                             QQ = n.QQ,
                             RealName = n.RealName,
                             Logo = n.Logo,
                             KeyId = n.KeyId
                         }),
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
        /// 新增我的好友
        /// </summary>
        [ActionName("AddMyFriends")]
        [HttpPost]
        public object GetItemInfo10(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                _userservice.SaveMyFriend(model.userId, model.refUserId);

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
        /// 删除我的好友
        /// </summary>
        [ActionName("DelMyFriends")]
        [HttpPost]
        public object GetItemInfo11(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                _userservice.DelMyFriend(model.KeyId);

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


        [ActionName("SaveLocation")]
        [HttpPost]
        public object GetItemInfo12(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });


                _userservice.SaveLocation(model.Longitude, model.Dimension, model.userId);

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





        [ActionName("GetLocationUsers")]
        [HttpPost]
        public object GetItemInfo13(UserModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();


            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                //获取所有的数据
                var list = _userservice.GetAllUserLocation(model.userId);
                //传过来的经纬度
                double lo1 = Convert.ToDouble(model.Longitude), la1 = Convert.ToDouble(model.Dimension);

                var dislist = list.Select(n => new { userName = n.UserName, userId = n.UserID, logo = n.Logo, phone = n.Phone, dis = Untity.getDistance(lo1, la1, Convert.ToDouble(n.Longitude), Convert.ToDouble(n.Dimension)) });


                dic.Add("ReturnData", new { Message = dislist.Where(n => n.dis < model.metre).OrderBy(n => n.dis), Code = "ok" });
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
