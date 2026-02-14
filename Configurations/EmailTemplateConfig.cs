namespace Poseidon.Configurations
{
    public class EmailTemplateConfig
    {
        public string Name { get; set; } = string.Empty;
        public Guid TemplateId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Variables { get; set; } = new List<string>();
    }
}
