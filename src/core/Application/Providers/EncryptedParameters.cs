using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Application.Providers
{
    public class EncryptedParameters : ActionFilterAttribute
    {
        public string ParameterName { get; }

        public EncryptedParameters(string parameterName = "secret")
        {
            ParameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IOptions<CryptoEngine.Secrets>>();
            var encrypted = context.HttpContext.Request.Query[ParameterName].FirstOrDefault();
            //if (!encrypted?.Contains(" "))
            //{
            //    encrypted = HttpUtility.UrlEncode(encrypted);
            //}
            ////

            var encryptesd = context.HttpContext.Request.QueryString.Value;
            encrypted = encryptesd.Replace("?secret=", "").Split('&')[0];
            int index = encrypted.LastIndexOf('?');
            if (index != -1)
            {
                encrypted.Remove(index);
            }
            // decrypt secret
            var decrypted = CryptoEngine.Decrypt(encrypted, config.Value.Key);
            var collection = HttpUtility.ParseQueryString(decrypted);
            var actionParameters = context.ActionDescriptor.Parameters;

            foreach (var parameter in actionParameters)
            {
                try
                {
                    var value = collection[parameter.Name];

                    if (value == null)
                        continue;

                    // set the action arguments to the values 
                    // from the encrypted parameter
                    context.ActionArguments[parameter.Name] =
                        ConvertToType(value, parameter.ParameterType);
                }
                catch (Exception e)
                {
                    context.ModelState.TryAddModelException(parameter.Name, e);
                }
            }
        }

        private static object? ConvertToType(string value, Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (value.Length > 0)
            {
                if (type == typeof(DateTimeOffset) || underlyingType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
                }

                if (type == typeof(DateTime) || underlyingType == typeof(DateTime))
                {
                    return DateTime.Parse(value, CultureInfo.InvariantCulture);
                }
                 if (type == typeof(Enum) || underlyingType == typeof(Enum))
                {
                    return Enum.Parse(typeof(Enum), value);
                }

                if (type == typeof(Guid) || underlyingType == typeof(Guid))
                {
                    return new Guid(value);
                }

                if (type == typeof(Uri) || underlyingType == typeof(Uri))
                {
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
                    {
                        return uri;
                    }

                    return null;
                }
                if (type == typeof(string) || underlyingType == typeof(string))
                {
                    return value;
                }
                if (type == typeof(int) || underlyingType == typeof(int))
                {
                    return int.Parse(value);
                }
            }
            else
            {
                if (type == typeof(Guid))
                {
                    return default(Guid);
                }

                if (underlyingType != null)
                {
                    return null;
                }
            }

            if (underlyingType is not null)
            {
                return Convert.ChangeType(value, underlyingType);
            }

            return Convert.ChangeType(value, type);
        }
    }
}
