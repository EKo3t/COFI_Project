using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using InternetBankingDal;
using Newtonsoft.Json.Linq;

namespace Internet_Banking.Utilities
{
    public class PassportChecker
    {
        private InternetBankingEntities entities = new InternetBankingEntities();
        public string CheckPassportFormat(string format, string nation)
        {
            if (format == null)
                return String.Empty;
            format = format.ToUpper();
            List<string> posFormats = entities.PassportFormats.Where(x => x.Country == nation).Select(x => x.CountryPasFormat).ToList();
            bool globalflag = false;
            StringBuilder result = new StringBuilder("Формат согласно гражданству: ");
            foreach (string formatStr in posFormats)
            {
                int i = 0;
                bool flag = true;
                int k = 0;
                while ((i < formatStr.Length))
                {
                    if ((formatStr[i] > '0') && (formatStr[i] <= '9'))
                    {
                        int numb = Convert.ToInt32(formatStr[i])-48;
                        i += 1;                        
                        string[] enums = { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "0123456789" };
                        char[] resultEnum = {'Б', 'Ц'};
                        int index = formatStr[i] == 'L' ? 0 : 1;
                        for (int j = 0; j < numb; j++)
                        {
                            if ((k < format.Length) && (!enums[index].Contains(format[k + j])))
                            {
                                flag = false;
                            }
                            result.Append(resultEnum[index]);
                        }
                        k += numb;                        
                    }
                    if (formatStr[i] == 'S')
                    {
                        if ((k < format.Length)&&(format[k] != ' '))
                            flag = false;
                        result.Append(' ');
                        k += 1;
                    }
                    i += 1;
                }
                if (k != format.Length)
                    flag = false;
                if (flag == true)
                    globalflag = true;
            }
            if (globalflag)
                return String.Empty;
            else
                return result.ToString();
        }
        public string CheckRegion(string format, string nation)
        {
            if ((format == null)||(nation == null))
                return String.Empty;
            string result = "Ваш номер региона не корректен";
            bool flag = false;
            List<PassportRegion> regionList = entities.PassportRegions.Where(x => x.Country == nation).ToList();
            foreach (var region in regionList)
            {
                string subStr = format.Substring(region.Index, region.Region.Length);
                if (subStr == region.Region)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                return String.Empty;
            else
                return result;
        }
    }
}