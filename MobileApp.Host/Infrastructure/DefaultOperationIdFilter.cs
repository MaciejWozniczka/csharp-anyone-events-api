namespace MobileApp.Host.Infrastructure;

public class DefaultOperationIdFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        string actionName = ((ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).ActionName;
        operation.OperationId = actionName;
    }
}