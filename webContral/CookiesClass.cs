using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;

namespace webContral
{
    public class CookiesClass
    {
        /// <summary>
        /// 创建身份验证
        /// </summary>
        /// <param name="Context">http封装请求信息</param>
        /// <param name="userName">身份名称</param>
        /// <param name="UserMessage">身份存储信息</param>
        /// <param name="continueUrl">重定向页面</param>
        public static void UserCookie(HttpContext Context, string userName, List<string> UserMessage, string continueUrl)
        {
            try
            {
                Context.Response.Cookies.Clear();
                string userRoles = "";
                for (int i = 0; i < UserMessage.Count; i++)
                {
                    userRoles += UserMessage[i];
                }
                FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(30), false, userRoles, "/ ");   //建立身份验证票对象 
                string HashTicket = FormsAuthentication.Encrypt(Ticket);   //加密序列化验证票为字符串 
                HttpCookie UserCookie = new HttpCookie(FormsAuthentication.FormsCookieName, HashTicket);//生成Cookie 
                UserCookie.HttpOnly = true;
                Context.Response.Cookies.Add(UserCookie);   //输出Cookie  }
            }
            catch (Exception ex)
            {
                Context.Session["error"] = ex.Message;
            }
        }

        /// <summary>
        /// 获取身份验证数据
        /// </summary>
        /// <param name="request">http服务请求</param>
        /// <returns></returns>
        public static string getCookiesDate(HttpRequest request)
        {
            try
            {
                HttpCookie cookie = request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket Ticket = FormsAuthentication.Decrypt(cookie.Value);
                string userRoles = Ticket.UserData;
                return userRoles;
            }
            catch(Exception ex)
            {
                return "::" + ex.Message;
            }
        }
        /// <summary>
        /// 获取身份验证用户
        /// </summary>
        /// <param name="request">http服务请求</param>
        /// <returns></returns>
        public static string getCookiesUserName(HttpRequest request)
        {
            try
            {
                HttpCookie cookie = request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket Ticket = FormsAuthentication.Decrypt(cookie.Value);
                string ID = Ticket.Name;
                return ID;
            }
            catch (Exception ex)
            {
                return "::" + ex.Message;
            }
        }
    }
}
