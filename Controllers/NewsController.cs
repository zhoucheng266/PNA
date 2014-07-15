using Pna.core;
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
    public class NewsController : ApiController
    {

        private INews _newService;
        public NewsController(INews newService)
        {
            _newService = newService;
        }

        [ActionName("GetNewsCategory")]
        [HttpPost]
        public object GetItemInfo1()
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                var result = _newService.GetAllNewsCategory();

                dic.Add("ReturnData", new
                {
                    Message = result.Select(n => new
                    {
                        newsCategoryName = n.newsCategoryName,
                        newsCategoryID = n.newsCategoryID
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


        [ActionName("GetNewsTheme")]
        [HttpPost]
        public object GetItemInfo2()
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                var result = _newService.GetAllTheme();

                dic.Add("ReturnData", new
                {
                    Message = result.Select(n => new
                    {
                        ThemeID = n.ThemeID,
                        ThemeName = n.ThemeName,
                        Flag = n.Flag
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


        [ActionName("GetNews")]

        //{"newsCategoryId":"1","firstPage":"20","endPage":"30"}
        [HttpPost]
        public object GetItemInfo3(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var restultTotal = _newService.GetAllNews(model.themeId, model.newsCategoryId);

                //搜索关键字
                if (!string.IsNullOrEmpty(model.keyWord))
                {
                    restultTotal = restultTotal.Where(n => n.Title.Contains(model.keyWord)).ToList();
                }
      
                var result = restultTotal.Skip(model.firstPage - 1).Take(model.endPage - model.firstPage + 1);

             

                dic.Add("ReturnData", new
                {
                    Message = result.Select(n => new
                    {
                        newsId = n.NewsID,
                        newsCategoryId = n.newsCategoryID,
                        newsTitle = n.Title,
                        newsCover = n.Cover,
                        summry=n.Summry,
                        ThreadCount = n.NewsTopic.Count(),
                        Total = restultTotal.Count(),
                   
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




        [ActionName("GetNewsDetail")]

        //{"newsId":"3"}
        [HttpPost]
        public object GetItemInfo4(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var result = _newService.GetAllNews(0, 0).Where(n => n.NewsID == model.newsId).FirstOrDefault();


                dic.Add("ReturnData", new
                {
                    Message = new
                    {
                        newsId = result.NewsID,
                        newsCategoryId = result.newsCategoryID,
                        newsTitle = result.Title,
                        ThreadCount = result.NewsTopic.Count(),
                        newContents = result.Contents,
                        newCreater = result.Creater,
                        newCreateTime = result.CreateTime,

                    }
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



        [ActionName("GetNewsTopic")]
        //{"newsId":"3","firstPage":"1","endPage":"30"}
        [HttpPost]
        public object GetItemInfo5(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                var result = _newService.GetAllTopic(model.newsId,0);


                //我的评论
                if (model.userId > 0)
                {
                    result = result.Where(n => n.CreateUserId == model.userId).ToList();
                }

                var total = result.Where(n => n.ParentID == 0).Skip(model.firstPage - 1).Take(model.endPage - model.firstPage + 1);

                dic.Add("ReturnData", new
                {
                    Message = total.Select(n => new
                    {
                        newsId = n.NewsID,
                        TopicID = n.TopicID,
                        TopicContents = n.TopicContents,
                        CreateTime = n.CreateTime,
                        Creater = n.Creater,
                        CreateUserId = n.CreateUserId,
                        Total = total.Count(),
                        Reply = result.Where(w => w.ParentID == n.TopicID).Select(w => new
                        {
                            newsId = w.NewsID,
                            TopicID = w.TopicID,
                            TopicContents = w.TopicContents,
                            CreateTime = w.CreateTime,
                            Creater = w.Creater,
                            CreateUserId = w.CreateUserId,
                        })
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


        [ActionName("AddNewsTopic")]

        //{"newsId":"3","topicContents":"ssss","creater":"周城","userId":"1","pId":"3"}
        [HttpPost]
        public object GetItemInfo6(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                _newService.SaveNewsTopic(new NewsTopic
                {
                    NewsID = model.newsId,
                    TopicContents = model.topicContents,
                    Creater = model.creater,
                    CreateUserId = model.userId,
                    ParentID = model.pId

                });


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




        [ActionName("GetMyCollection")]

        //{"userId":"1","firstPage":"1","endPage":"30"}
        [HttpPost]
        public object GetItemInfo7(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var restultTotal = _newService.GetMyCollection(model.userId);
                var result = restultTotal.Skip(model.firstPage - 1).Take(model.endPage - model.firstPage + 1);

                dic.Add("ReturnData", new
                {
                    Message = result.Select(n => new
                    {
                        newsId = n.NewsID,
                        newsCategoryId = n.newsCategoryID,
                        newsTitle = n.Title,
                        newsCover = n.Cover,

                        Total = restultTotal.Count(),
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




        [ActionName("SaveMyCollection")]

        //{"userId":"1","newsId":"1"}
        [HttpPost]
        public object GetItemInfo8(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                if (_newService.GetMyCollection(model.userId, model.newsId) != null)
                {
                    dic.Add("ReturnData", new { Message = "已经收藏本条新闻", Code = "E_20001" });
                    return dic;
                }

                _newService.SaveMyCollection(model.userId, model.newsId);


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


        [ActionName("GetMyNewsTopic")]
        //{"newsId":"3","firstPage":"1","endPage":"30"}
        [HttpPost]
        public object GetItemInfo9(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });
                var result = _newService.GetAllTopic(0,model.userId);


                //我的评论
                if (model.userId > 0)
                {
                    result = result.Where(n => n.CreateUserId == model.userId).ToList();
                }

                var total = result.Where(n => n.ParentID == 0).Skip(model.firstPage - 1).Take(model.endPage - model.firstPage + 1);

                dic.Add("ReturnData", new
                {
                    Message = total.Select(n => new
                    {
                        newsId = n.NewsID,
                        TopicID = n.TopicID,
                        TopicContents = n.TopicContents,
                        CreateTime = n.CreateTime,
                        Creater = n.Creater,
                        CreateUserId = n.CreateUserId,
                        Total = total.Count(),
                        newsTitle=n.News.Title,
                        newsCreateTime=n.News.CreateTime
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

        [ActionName("GetNewsLoop")]
        [HttpPost]
        public object GetItemInfo10(NewsModel model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                dic.Add("ReturnInfo", new ReturnInfo { Message = "成功", State = 1 });

                var result = _newService.GetAllNews(0, model.newsCategoryId).Where(n => n.IsStar == true);
   
   
                dic.Add("ReturnData", new
                {
                    Message = result.Select(n => new
                    {
               
                        newsCover = n.Cover,
                        newsId = n.NewsID
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

    }
}
