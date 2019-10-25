using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace WA_HyJ.Helpers
{
    public static partial class Script
    {
        public static MvcHtmlString Action(string action, string controller, object routeData)
        {
            var data = new RouteValueDictionary(routeData);
            var scriptVarsToReplace = getScriptVars(data);
            var requestContext = ((MvcHandler)HttpContext.Current.Handler).RequestContext;
            var urlPattern = UrlHelper.GenerateUrl(null, action, controller, data,
                                                   RouteTable.Routes, requestContext,
                                                   true);
            var url = generateFunction(urlPattern, scriptVarsToReplace);
            return MvcHtmlString.Create(url);
        }

        private static IEnumerable<string> getScriptVars(IDictionary<string, object> dictionary)
        {
            var scriptVariables = new List<string>();
            foreach (var prop in dictionary)
            {
                var value = prop.Value as string;
                if (value != null && value.StartsWith("js:") && value.Length > 3)
                {
                    var varName = value.Substring(3);
                    scriptVariables.Add(varName);
                }
            }
            return scriptVariables;
        }

        private static string generateFunction(string urlPattern, IEnumerable<string> scriptVars)
        {

            var sb = new StringBuilder();
            sb.Append("(function() {");
            sb.AppendFormat(" return '{0}'", urlPattern);
            foreach (var variable in scriptVars)
            {
                sb.AppendFormat(".replace(/{0}/, encodeURIComponent({1}))",
                                "js(%)3[aA]" + variable,
                                variable);
            }
            sb.Append("; })()");
            return sb.ToString();
        }

        public static MvcHtmlString Replace<TResult>(Expression<Func<string, string, TResult>> expression)
        {
            return replaceInternal(expression);
        }

        private static MvcHtmlString replaceInternal(LambdaExpression expression)
        {
            var parameters = expression.Parameters
                                .Select(param => paramPlaceHolder(param.Name)).ToArray();

            var urlPattern = getResultPattern(expression, parameters);
            var sb = new StringBuilder();
            sb.Append("(function() {");
            sb.AppendFormat(" return '{0}'", urlPattern);
            foreach (var parameter in expression.Parameters)
            {
                sb.AppendFormat(".replace('{0}', encodeURIComponent({1}))",
                                paramPlaceHolder(parameter.Name), parameter.Name);
            }
            sb.Append("; })()");
            return MvcHtmlString.Create(sb.ToString());
        }

        private static string paramPlaceHolder(string paramName)
        {
            return "param-" + paramName;
        }

        private static string getResultPattern(LambdaExpression expression, params object[] args)
        {
            var func = expression.Compile();
            object result = func.DynamicInvoke(args);
            return result.ToString();
        }
    }
}