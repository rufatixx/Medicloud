namespace Medicloud.DAL.Entities
{
	public class AnamnesisFieldDAO
	{
		public int id { get; set; }
		public string name { get; set; }
		public List<AnamnesisTemplateDAO> Templates { get; set; }
	}
}
