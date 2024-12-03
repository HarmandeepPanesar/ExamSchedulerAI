//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.WebUtilities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ExamsSchedule
//{
//    public static class NavigationManagerExtensions
//    {
//        public static string TryGetQueryString<T>(this NavigationManager navManager, string key)
//        {
//            var uri = navManager.ToAbsoluteUri(navManager.Uri);

//            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
//            {
//                if (typeof(T) == typeof(string))
//                {
//                    return valueFromQueryString.ToString();
//                }
//            }
//            return "";
//        }
//    }
//}
