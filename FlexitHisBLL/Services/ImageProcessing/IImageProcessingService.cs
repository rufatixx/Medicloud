

namespace Medicloud.BLL.Services.ImageProcessing
{
	public interface IImageProcessingService
	{
		Task<byte[]> ResizeImageTo720pAsync(byte[] imageBytes);
	}
}
