

namespace EmailService.Interfaces
{
    public interface IEmailConfiguration
    {
        public string From { get; }
        public string SmtpServer { get; }
		public int SmtpPort { get; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
	}
}
