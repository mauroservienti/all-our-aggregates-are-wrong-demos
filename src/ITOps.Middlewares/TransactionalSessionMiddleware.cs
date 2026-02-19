using Microsoft.AspNetCore.Http;
using NServiceBus.Persistence.Sql;
using NServiceBus.TransactionalSession;
using System.Threading.Tasks;

namespace ITOps.Middlewares
{
    public class TransactionalSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionalSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITransactionalSession transactionalSession)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                await transactionalSession.Open(new SqlPersistenceOpenSessionOptions());
                // If _next throws, Commit is never called and the session is rolled back on disposal
                await _next(context);
                await transactionalSession.Commit();
            }
            else
            {
                await _next(context);
            }
        }
    }
}
