using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Services.Anamnesis
{
	public interface IAnamnesisService
	{
		Task<List<AnamnesisFieldDAO>> GetFieldsWithTemplatesByDoctorId(int doctorID);
		Task<int> AddAnamnesis(AddAnamnesisDTO dto);
		Task<int> UpdateAnamnesis(AddAnamnesisDTO dto);
		Task<List<AnamnesisDAO>> GetAnamnesisByCardId(int cardId);
		Task<AnamnesisDAO> GetAnamnesisById(int id);
		Task<bool> RemoveAnamnesis(int anamnesisId);
	}
}
