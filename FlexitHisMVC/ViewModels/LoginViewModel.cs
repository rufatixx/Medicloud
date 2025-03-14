namespace Medicloud.WebUI.ViewModels
{
	public class LoginViewModel
	{
		public int Type { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
		public bool UserExist { get; set; }
		public string errorMessage { get; set; }
	}
}
