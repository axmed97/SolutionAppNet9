using Autofac;
using Business.Abstract;
using Business.Concrete;
using Business.Utilities.Storage;
using Business.Utilities.Storage.Concrete.LocalStorage;
using DataAccess.Concrete.EntityFramework;

namespace Business.DependencyResolver.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppDbContext>().SingleInstance();

            builder.RegisterType<AuthManager>().As<IAuthService>().SingleInstance();

            builder.RegisterType<LocalStorageManager>().As<IStorageService>().SingleInstance();
        }
    }
}
