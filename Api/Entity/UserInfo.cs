﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Password { get; set; }
        public string ChineseName { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int WaitSet { get; set; }
        public string Token { get; set; }
        public string EMail { get; set; }
        public string TelPhone { get; set; }
    }
}
