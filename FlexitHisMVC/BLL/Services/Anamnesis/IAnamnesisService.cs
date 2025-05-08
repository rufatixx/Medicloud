using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Services.Anamnesis
{
	public interface IAnamnesisService
	{
		Task<List<AnamnesisFieldDAO>> GetFieldsWithTemplatesByDoctorId(int doctorID);
		Task<int> AddAnamnesis(AddAnamnesisDTO dto);
		Task<List<AnamnesisDAO>> GetAnamnesisByCardId(int cardId);

	}
}
