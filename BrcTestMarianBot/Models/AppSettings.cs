﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrcTestMarianBot.Models
{
    public static class AppSettings
    {
        public static string Url { get; set; } = "https://telegrambotapp.azurewebsites.net:443/{0}";
        public static string Name { get; set; } = "BrcTestMarian_bot";

        public static string Key { get; set; } = "";
    }
}