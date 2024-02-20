namespace CleanDDDArchitecture.Domains.Shared.Core;

public class AppSettings
{
    public string Title { get; set; }
    public Dictionary<string, string> SocialMedia { get; set; }
    public LegalSettings Legal { get; set; }
    public string BaseUrl { get; set; }
    public EmailSettings Emails { get; set; }

    public class LegalSettings
    {
        public string TermsOfService { get; set; }
        public string PrivacyPolicy { get; set; }
    }

    public class EmailSettings
    {
        public string Contact { get; set; }
        public string NoReply { get; set; }
    }
}
