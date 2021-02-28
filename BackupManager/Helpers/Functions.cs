using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace BackupManager.Helpers
{
    public class Functions
    {

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return str;
        }

        public static string AddPrefix(string prefix, string str, short stringlength)
        {
            if (str == null) str = "";
            if (str.Length > stringlength)
                return str;
            //return (str.Substring(0, stringlength));

            while (str.Length < stringlength)
            {
                str = prefix + str;
            }
            return str;
        }

        public static string HashToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string GetMonthName(int monthNumber)
        {
            if (monthNumber < 1 || monthNumber > 12) return "Invalid";
            DateTime date = new DateTime(2000, monthNumber, 1);
            return date.ToString("MMM");
        }

        public static string GetErrorFromException(Exception ex)
        {
            if (ex == null) return "Exception is null";
            string error = ex.Message ?? "Error is null";
            if (ex.InnerException != null) error = ex.InnerException.Message ?? "Level 1 InnerException message is null";
            if (ex.InnerException != null && ex.InnerException.InnerException != null) error = ex.InnerException.InnerException.Message ?? "Level 2 InnerException message is null";
            return error;
        }

        const string EmailRegex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                               + "@"
                                               + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        public static bool IsValidEmail(string email)
        {
            var regex = new Regex(EmailRegex, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

    }
}
