namespace Msc.Interface.Db
{
    public class LocaleProvider : ILocaleProvider
    {
        public LocaleProvider()
        {
            Locale = Locale.Ru;
        }

        public Locale Locale { get; set; }
    }
}