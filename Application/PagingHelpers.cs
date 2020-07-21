//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Application
//{
//    public static class PagingHelpers
//    {
//        public static IHtmlContent Dir(this IHtmlHelper html)
//        {
//            bool isRtl = CultureInfo.CurrentCulture.Name == "ar-EG";

//            return new HtmlString(isRtl ? "rtl" : "ltr");
//        }

//        public static string CultureCode(this IHtmlHelper html)
//            => CultureInfo.CurrentCulture.Name == "ar-EG" ? "ar" : "en";

//        /// <summary>
//        /// This is applicable for Bootstrap version 3.0
//        /// </summary>
//        /// <param name="html"></param>
//        /// <param name="currentPage"></param>
//        /// <param name="totalPages"></param>
//        /// <param name="pageUrl"></param>
//        /// <returns></returns>
//        public static IHtmlContent BootstrapPageLinks(this IHtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
//        {
//            bool isRtl = CultureInfo.CurrentCulture.Name == "ar-EG";
//            string records = isRtl ? "سجلات" : "records";

//            int currentPage = pagingInfo.CurrentPage;
//            int totalPages = pagingInfo.TotalPages;

//            //number of pages to be displayed
//            const short max = 10;
//            double level = Math.Ceiling(currentPage / (double)max) * max;

//            var list = new TagBuilder("ul");
//            if (isRtl)
//                list.MergeAttribute("direction", "rtl");

//            list.MergeAttribute("class", "pagination");

//            int startPage = (int)level - max + 1;

//            IHtmlContent TagMaker(string text, string url, bool isActive)
//            {
//                var pageNumberTag = new TagBuilder("a");
//                pageNumberTag.MergeAttribute("class", "page-link");

//                if (!string.IsNullOrWhiteSpace(url))
//                    pageNumberTag.MergeAttribute("href", url);

//                pageNumberTag.InnerHtml.AppendHtml(new HtmlString(text));

//                var listItem = new TagBuilder("li");
//                list.MergeAttribute("class", "page-item");

//                if (isActive)
//                    listItem.MergeAttribute("class", "active", replaceExisting: false);

//                listItem.InnerHtml.AppendHtml(pageNumberTag);
//                return listItem;
//            }

//            IHtmlContent DirectionMaker(string destination, string url, bool isLink = true)
//            {
//                var liChildTag = new TagBuilder("a");
//                liChildTag.MergeAttribute("class", "page-link");

//                if (isLink)
//                {
//                    if (!string.IsNullOrWhiteSpace(url))
//                        liChildTag.MergeAttribute("href", url);
//                }
//                else
//                {
//                    liChildTag.MergeAttribute("class", "page-link disabled", true);
//                }

//                var iconTag = new TagBuilder("span");
//                string liClasses = "";
//                if (!string.IsNullOrWhiteSpace(destination))
//                {
//                    iconTag.InnerHtml.AppendHtml(destination);

//                    if (destination == "Next")
//                        liClasses += "next";

//                    if (destination == "Previous")
//                        liClasses += "prev";
//                }

//                liChildTag.InnerHtml.AppendHtml(iconTag);

//                var listItem = new TagBuilder("li");
//                listItem.MergeAttribute("class", liClasses, true);
//                listItem.InnerHtml.AppendHtml(liChildTag);
//                return listItem;
//            }

//            if (totalPages > 1 && currentPage != 1)
//            {
//                //if (isRtl)
//                //{
//                list.InnerHtml.AppendHtml(DirectionMaker("Previous", pageUrl(currentPage - 1)));
//                list.InnerHtml.AppendHtml(DirectionMaker("First Page", pageUrl(1)));
//                //}
//                //else
//                //{
//                //    list.InnerHtml.AppendHtml(DirectionMaker("fa fa-angle-double-left", pageUrl(1)));
//                //    list.InnerHtml.AppendHtml(DirectionMaker("fa fa-angle-left", pageUrl(currentPage - 1)));
//                //}
//            }
//            else
//            {
//                list.InnerHtml.AppendHtml(DirectionMaker("Previous", pageUrl(currentPage - 1), false));
//            }

//            for (int i = startPage; i <= level; i++)
//            {
//                if (i > totalPages)
//                    break;

//                if (i == currentPage)
//                    list.InnerHtml.AppendHtml(TagMaker(i + "", "", true));
//                else
//                    list.InnerHtml.AppendHtml(TagMaker(i + "", pageUrl(i), false));
//            }

//            if (totalPages > 1 && currentPage != totalPages)
//            {
//                //if (isRtl)
//                //{
//                list.InnerHtml.AppendHtml(DirectionMaker("Last Page", pageUrl(totalPages)));

//                if (pagingInfo.TotalResults > 1)
//                    list.InnerHtml.AppendHtml(TagMaker($"{pagingInfo.TotalResults} {records}", null, false));

//                list.InnerHtml.AppendHtml(DirectionMaker("Next", pageUrl(currentPage + 1)));
//                //}
//                //else
//                //{
//                //    list.InnerHtml.AppendHtml(DirectionMaker("fa fa-angle-right", pageUrl(currentPage + 1)));
//                //    list.InnerHtml.AppendHtml(DirectionMaker("fa fa-angle-double-right", pageUrl(totalPages)));
//                //}
//            }
//            else
//            {
//                if (pagingInfo.TotalResults > 1)
//                    list.InnerHtml.AppendHtml(TagMaker($"{pagingInfo.TotalResults} {records}", null, false));

//                list.InnerHtml.AppendHtml(DirectionMaker("Next", pageUrl(currentPage + 1), false));
//            }


//            if (totalPages > 1)
//            {
//                //direct page navigation
//                var boxTag = new TagBuilder("input");
//                boxTag.MergeAttribute("placeholder", "page");
//                boxTag.MergeAttribute("type", "text");
//                boxTag.MergeAttribute("style", "width:60px;margin:0;");
//                //long nasty javascript call. Be careful when modifying this.
//                string jsNavigate = "if (event.keyCode==13){" +
//                                 "if(isNaN(this.value) || this.value < 1){alert('Please enter positive number');return;}" +
//                                 "if(this.value > " + totalPages + "){alert('Maximum number allowed is " + totalPages +
//                                 "');return;}" +
//                                 "var url ='" + pageUrl(1).Replace("page=1", "page=") + "';" + //REMEMBER that page= must be all lowercase otherwise replacement won't work
//                                 "url = url.replace('page=', 'page=' + this.value);" +
//                                 "window.location = url;" +
//                                 "}";

//                boxTag.MergeAttribute("onkeypress", jsNavigate);

//                var listItem = new TagBuilder("li");
//                listItem.MergeAttribute("class", "page-item");

//                var wrapper = new TagBuilder("a");
//                wrapper.MergeAttribute("class", "page-link");
//                wrapper.InnerHtml.AppendHtml(boxTag);
//                wrapper.MergeAttribute("style", "padding-top:9px;padding-bottom:9px;");
//                listItem.InnerHtml.AppendHtml(wrapper);
//            }

//            var result = new TagBuilder("nav");
//            result.InnerHtml.AppendHtml(list);
//            return result;
//        }
//    }
//}
