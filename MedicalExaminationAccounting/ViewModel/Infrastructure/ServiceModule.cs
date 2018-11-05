using MedicalExaminationAccounting.Model.Interfaces;
using MedicalExaminationAccounting.Model.Repositories;
using Ninject.Modules;

namespace MedicalExaminationAccounting.ViewModel.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        private readonly string connectionString;
        public ServiceModule(string connection)
        {
            connectionString = connection;
        }
        public override void Load()
        {
            Bind<IUnitOfWork>().To<EFUnitOfWork>().WithConstructorArgument(connectionString);
        }
    }
}