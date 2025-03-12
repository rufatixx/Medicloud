
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Medicloud.BLL.Services.ImageProcessing
{
	public class ImageProcessingService:IImageProcessingService
	{
		public async Task<byte[]> ResizeImageTo720pAsync(byte[] imageBytes)
		{
			using (var memoryStream = new MemoryStream(imageBytes))
			{
				using (var image = await Image.LoadAsync(memoryStream))
				{

					int maxWidth = 1280;
					int maxHeight = 720;

					if (image.Width > maxWidth || image.Height > maxHeight)
					{

						var newWidth = image.Width > maxWidth ? maxWidth : image.Width;
						var newHeight = image.Height > maxHeight ? maxHeight : image.Height;

						image.Mutate(x => x.Resize(newWidth, newHeight));
					}

					using (var outputStream = new MemoryStream())
					{
						await  image.SaveAsync(outputStream, new JpegEncoder() { Quality = 90 }); 
						return outputStream.ToArray(); 
					}
				}
			}
		}
	}
}
