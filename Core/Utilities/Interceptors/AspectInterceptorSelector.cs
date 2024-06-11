using Castle.DynamicProxy;
using Core.Aspects.Autofac.Exception;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;// Serilog logger'ını kullanacağız
using Serilog;
using System.Reflection;

namespace Core.Utilities.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();
            var methodAttributes = type.GetMethod(method.Name).GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            classAttributes.AddRange(methodAttributes);

            // Varsayılan olarak tüm metodlar için eklemek istediğiniz aspect'ler
            classAttributes.Add(new ExceptionLogAspect(typeof(FileLogger)));
            classAttributes.Add(new PerformanceAspect(5));

            // Loglama işlemi için Serilog'u kullanma
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.MSSqlServer(connectionString: "Server=ATILLA; Database=TobetoDb; Trusted_Connection=True; Encrypt=True; TrustServerCertificate=True;",sinkOptions:new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = true,
                    TableName = "logs",
                })
                .CreateLogger();

            var logInterceptor = new LogAspect(new FileLogger()); // LogInterceptor yerine LogAspect kullanıyoruz
            classAttributes.Add(logInterceptor); // LogInterceptor yerine logInterceptor değişkenini ekliyoruz

            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}
