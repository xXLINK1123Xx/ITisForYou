﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using System.Threading;
using System.Web;
using AutoMapper;
using AutoMapper.Configuration;
using Learning_Platform.Controllers;
using Learning_Platform.Data;
using Learning_Platform.Mapping;
using Learning_Platform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Learning_Platform
{
    public class SimpleInjectorConfig
    {
        private readonly Container container;

        public SimpleInjectorConfig(IAppBuilder app)
        {
            container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            Configure(app);
        }

        private void Configure(IAppBuilder app)
        {
            
            container.Register<IAppBuilder>(()=> app, Lifestyle.Singleton);

            
            container.Register<MapperProvider>(()=> new MapperProvider(container), Lifestyle.Singleton);
            container.Register(()=> GetMapper(container), Lifestyle.Singleton);
            
            container.Register<LPDataContext>(() => new LPDataContext(), Lifestyle.Scoped);

            container.Register<ApplicationDbContext>(()=> new ApplicationDbContext(), Lifestyle.Scoped);

            container.Register<IUserStore<ApplicationUser>>(
                () => new UserStore<ApplicationUser>(container.GetInstance<ApplicationDbContext>()), Lifestyle.Scoped);

            container.Register<
                ApplicationUserManager>(Lifestyle.Scoped);

            container.RegisterInitializer<ApplicationUserManager>(
                manager => InitializeUserManager(manager, app));
            container.Register<ISecureDataFormat<AuthenticationTicket>,
                SecureDataFormat<AuthenticationTicket>>(Lifestyle.Scoped);
            container.Register<ITextEncoder, Base64UrlTextEncoder>(Lifestyle.Scoped);
            container.Register<IDataSerializer<AuthenticationTicket>, TicketSerializer>(
                Lifestyle.Scoped);
            container.Register<IDataProtector>(
                () => new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider()
                    .Create("ASP.NET Identity"),
                Lifestyle.Scoped);

            //container.Register(
            //    () => HttpContext.Current.GetOwinContext().Authentication);
        }

        private AutoMapper.IMapper GetMapper(Container container)
        {
            var mp = container.GetInstance<MapperProvider>();
            return mp.GetMapper();
        }

        private static void InitializeUserManager(
            ApplicationUserManager manager, IAppBuilder app)
        {
            manager.UserValidator =
                new UserValidator<ApplicationUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

            //Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider =
                app.GetDataProtectionProvider();

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }

        public Container GetContainer()
        {
            return container;
        }

        public static IEnumerable<Assembly> GetMaps()
        {
            yield return typeof(MappingProfiles).Assembly;
            //yield return typeof(LessonController.Mapping).Assembly;
        }


        public class MapperProvider
        {
            private readonly Container _container;

            public MapperProvider(Container container)
            {
                _container = container;
            }

            public IMapper GetMapper()
            {
                var mce = new MapperConfigurationExpression();
                mce.ConstructServicesUsing(_container.GetInstance);
                mce.Advanced.AllowAdditiveTypeMapCreation = true;
                mce.AddProfiles(GetMaps());

                var mc = new MapperConfiguration(mce);
                mc.AssertConfigurationIsValid();

                IMapper m = new Mapper(mc, t => _container.GetInstance(t));

                return m;
            }
        }
    }

    public interface IOwinContextAccessor
    {
        IOwinContext CurrentContext { get; }
    }

    public class CallContextOwinContextAccessor : IOwinContextAccessor
    {
        public static AsyncLocal<IOwinContext> OwinContext = new AsyncLocal<IOwinContext>();
        public IOwinContext CurrentContext => OwinContext.Value;
    }

}