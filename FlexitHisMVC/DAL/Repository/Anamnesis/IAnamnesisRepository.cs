using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Anamnesis
{
	public interface IAnamnesisRepository
	{
		Task<List<AnamnesisFieldDAO>> GetFieldsWithTemplatesByDoctorId(int doctorID);
		Task<int> AddAnamnesisAsync(AnamnesisDAO dao);
		Task<int> AddAnamnesisAnswerAsync(AnamnesisAnswerDAO dao);
		Task<List<AnamnesisDAO>> GetAnamnesisByCardId(int cardId);
		Task<AnamnesisDAO> GetAnamnesisById(int id);
		Task<int>GetAnamnesisAnswerByFieldAndAnamnesisId(int fieldId,int anamnesisId);
		Task<int>UpdateAnamnesisAnswer(int answerId, string answerText);
		Task<bool> RemoveAnamnesis(int anamnesisId);
	}
}
