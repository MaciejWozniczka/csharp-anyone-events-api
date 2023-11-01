namespace MobileApp.Host.Infrastructure;

public class UnitOfWorkBehavior<TRequest, TResponse, TContext> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TContext : DbContext
{
    private readonly TContext _db;
    public UnitOfWorkBehavior(TContext db)
    {
        _db = db;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next();

        if (request.GetType().Name.EndsWith("Command"))
        {
            var result = response as Result;

            if (result == null || result.Success)
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        return response;
    }
}