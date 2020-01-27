using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrcTestMarianBot.Models
{
    public enum Status
    {
        Usual,
        EnterLogin,
        EnterPassword,
        SelectDB,
        EnterSapLogin,
        EnterSapPassword,
        CreateSapUser
    };

    public class User
    {
        
        public long Id { set; get; }
        public string login { set; get; }
        public string password { set; get; }
        public string saplogin { set; get; }
        public string sappassword { set; get; }
        public string nameDB { set; get; }
        public bool auth { set; get; }
        public bool sapauth { set; get; }

        public Status status;

        public User(long userId)
        {
            Id = userId;
            status = Status.Usual;
            auth = false;
        }
    }
}