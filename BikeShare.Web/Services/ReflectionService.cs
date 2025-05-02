using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BikeShare.Web.Services;

public class ReflectionService
{
    private readonly XDocument? _xmlDoc;
    
    public ReflectionService(IWebHostEnvironment environment)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        var xmlPath = Path.Combine(assemblyDirectory ?? string.Empty, xmlFile);
        
        if (File.Exists(xmlPath))
        {
            _xmlDoc = XDocument.Load(xmlPath);
        }
    }
    
    public object GetApiOverview()
    {
        var apiControllers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller")
                        && t.Namespace?.Contains(".Api") == true);
            
        var endpoints = new List<ApiEndpointInfo>();
        
        foreach (var controller in apiControllers)
        {
            var routeAttr = controller.GetCustomAttribute<RouteAttribute>();
            var controllerRoute = routeAttr?.Template ?? string.Empty;
            
            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == controller && m.GetCustomAttributes<HttpMethodAttribute>().Any());
                
            foreach (var method in methods)
            {
                var httpAttr = method.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();
                var route = httpAttr?.Template ?? string.Empty;
                var httpMethod = GetHttpMethod(httpAttr);
                
                var fullPath = CombineRoutes(controllerRoute, route);
                
                endpoints.Add(new ApiEndpointInfo
                {
                    Path = fullPath,
                    Method = httpMethod,
                    Description = GetMethodDocumentation(method),
                    Parameters = GetParameterDocumentation(method),
                    ReturnValue = GetReturnDocumentation(method)
                });
            }
        }
        
        return new { Endpoints = endpoints };
    }
    
    private string GetMethodDocumentation(MethodInfo method)
    {
        var memberName = $"M:{method.DeclaringType?.FullName}.{method.Name}";
        
        if (method.GetParameters().Length > 0)
        {
            memberName += "(" + string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName?.Replace('+', '.'))) + ")";
        }
        
        return _xmlDoc?.Descendants("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)
            ?.Element("summary")?.Value.Trim() ?? string.Empty;
    }
    
    private Dictionary<string, string> GetParameterDocumentation(MethodInfo method)
    {
        var result = new Dictionary<string, string>();
        var memberName = $"M:{method.DeclaringType?.FullName}.{method.Name}";
        
        if (method.GetParameters().Length > 0)
        {
            memberName += "(" + string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName?.Replace('+', '.'))) + ")";
        }
        
        var xmlMember = _xmlDoc?.Descendants("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value == memberName);
            
        if (xmlMember != null)
        {
            foreach (var param in method.GetParameters())
            {
                var paramDoc = xmlMember.Elements("param")
                    .FirstOrDefault(p => p.Attribute("name")?.Value == param.Name)?.Value.Trim();
                    
                if (!string.IsNullOrEmpty(paramDoc))
                {
                    result[param.Name] = paramDoc;
                }
            }
        }
        
        return result;
    }
    
    private string GetReturnDocumentation(MethodInfo method)
    {
        var memberName = $"M:{method.DeclaringType?.FullName}.{method.Name}";
        
        if (method.GetParameters().Length > 0)
        {
            memberName += "(" + string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName?.Replace('+', '.'))) + ")";
        }
        
        return _xmlDoc?.Descendants("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)
            ?.Element("returns")?.Value.Trim() ?? string.Empty;
    }
    
    private string CombineRoutes(string baseRoute, string route)
    {
        baseRoute = baseRoute.Trim('/');
        route = route.Trim('/');
        
        if (string.IsNullOrEmpty(route))
            return $"/{baseRoute}";
            
        return $"/{baseRoute}/{route}";
    }
    
    private string GetHttpMethod(HttpMethodAttribute attr)
    {
        return attr switch
        {
            HttpGetAttribute => "GET",
            HttpPostAttribute => "POST",
            HttpPutAttribute => "PUT",
            HttpDeleteAttribute => "DELETE",
            HttpPatchAttribute => "PATCH",
            _ => "UNKNOWN"
        };
    }
}

public class ApiEndpointInfo
{
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
    public string ReturnValue { get; set; }
}