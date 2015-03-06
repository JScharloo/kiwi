using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lisa.Kiwi.Web
{
    public static class HtmlHelpers
    {
        public static HtmlString ShowImage(this string value, string addClass = "", string addAttribute = "")
        {
            var validUrl = false;
            try
            {
                new Uri(value);
                validUrl = true;
            }
            catch { }

            if (validUrl != true)
            {
                return new HtmlString("<span class='error'>Make sure the string is a available url.</span>");
            }

            if (addClass != "")
            {
                addClass = string.Format("class='{0}'", addClass);
            }

            var showImage = string.Format("<img src='{0}' {1} {2}>", value, addClass, addAttribute);
            return new HtmlString(showImage);
        }
    }
}