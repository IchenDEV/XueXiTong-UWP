using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace XueXiTong.Models.Args
{
    public class LoginEventArgs : EventArgs
    {
        public User user { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
    }
}
