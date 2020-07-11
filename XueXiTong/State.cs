using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XueXiTong.Models;

namespace XueXiTong
{
    public static class State
    {

        private static User currentUser = null;
        public static User CurrentUser
        {
            get
            {
                if (currentUser != null)
                {
                    return currentUser;
                }
                else
                {
                   
                    try
                    {
                        var localObjectStorageHelper = new LocalObjectStorageHelper();
                        var cookie=localObjectStorageHelper.Read<string>("CurrentUserCookie");
                        if (cookie == "") return null;
                        currentUser = new User(cookie.Split(';').ToList());
                        return currentUser;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    var localObjectStorageHelper = new LocalObjectStorageHelper();
                    localObjectStorageHelper.Save<string>("CurrentUserCookie", string.Join(';', value.Cookie));
                    currentUser = value;
                }
                else
                {
                    var localObjectStorageHelper = new LocalObjectStorageHelper();
                    localObjectStorageHelper.Save<string>("CurrentUserCookie", "");
                    currentUser = value;
                }
              
            }

        }
    }
}
