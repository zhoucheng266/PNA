using Autofac;
using Autofac.Integration.WebApi;
using Pna.core;
using Pna.Service.Instantiation;
using Pna.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PnaAPI.App_Start.Extend
{
    /// <summary>
    /// autofac配置
    /// </summary>
    public class AutofacConfig
    {
        /// <summary>
        /// 初始化Autofac
        /// </summary>
        public void Init(Action<ContainerBuilder> buliderfunc, ContainerBuilder builders)
        {
            //注册系统
            buliderfunc(builders);
            //自定义注册
            Dependency(builders);

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(builders.Build());
        }

        /// <summary>
        /// 自定义注册
        /// </summary>
        /// <param name="builder"></param>
        public void Dependency(ContainerBuilder builder)
        {



            //注册edm
            builder.Register(t => new pnadata_entiy()).InstancePerLifetimeScope();

            //注册接口
            builder.RegisterType<MailService>().As<IMail>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUser>().InstancePerLifetimeScope();
            builder.RegisterType<NewsService>().As<INews>().InstancePerLifetimeScope();
        }

    }
}