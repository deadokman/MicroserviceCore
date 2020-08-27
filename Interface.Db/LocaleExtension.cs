using System;

namespace Msc.Interface.Db
{
    public static class LocaleExtension
    {
        /// <summary>
        /// Транслятор из enum в коды языков БД
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static string GetLocaleString(this Locale locale)
        {
            switch (locale)
            {
                case Locale.Ru:
                {
                    return "RUS";
                }
                case Locale.Eng:
                {
                    return "ENG";
                }

                case Locale.Srb:
                {
                    return "SRP";
                }
            }
            throw new NotImplementedException("Not implemented Locale");
        }
    }
}