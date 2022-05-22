using EmailService.Interfaces;

namespace EmailService
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
	}
}
