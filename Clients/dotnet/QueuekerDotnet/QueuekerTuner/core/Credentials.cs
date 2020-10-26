using System;
using System.Collections.Generic;
using System.Text;

namespace QueuekerTuner.core
{
    public static class Credentials
    {
        public static string Host { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }

        public static string NewLogin { get; set; }
        public static string NewPassword { get; set; }

        public static bool Check()
        {
            return !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        }

        public static bool CheckNewCred()
        {
            return !string.IsNullOrEmpty(NewLogin) && !string.IsNullOrEmpty(NewPassword);
        }
    }
}
