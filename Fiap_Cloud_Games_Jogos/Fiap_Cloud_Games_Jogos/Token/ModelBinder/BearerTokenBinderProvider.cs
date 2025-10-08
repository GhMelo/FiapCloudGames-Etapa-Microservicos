using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AS.Library.Security.IAM.Credentials.Binder
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BearerTokenAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => BindingSource.Custom;
    }

    public class BearerTokenModelBinder : IModelBinder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BearerTokenModelBinder(IHttpContextAccessor accessor) => _httpContextAccessor = accessor;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            var token = header?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
            bindingContext.Result = ModelBindingResult.Success(token);
            return Task.CompletedTask;
        }
    }

    public class BearerTokenBinderProvider : IModelBinderProvider
    {
        private readonly IHttpContextAccessor _accessor;
        public BearerTokenBinderProvider(IHttpContextAccessor accessor) => _accessor = accessor;

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(string) &&
                context.BindingInfo.BindingSource == BindingSource.Custom)
            {
                return new BearerTokenModelBinder(_accessor);
            }
            return null;
        }
    }
}
