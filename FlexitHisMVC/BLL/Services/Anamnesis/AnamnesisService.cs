using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Anamnesis;

namespace Medicloud.BLL.Services.Anamnesis
{
	public class AnamnesisService:IAnamnesisService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAnamnesisRepository _anamnesisRepository;

		public AnamnesisService(IUnitOfWork unitOfWork, IAnamnesisRepository anamnesisRepository)
		{
			_unitOfWork = unitOfWork;
			_anamnesisRepository = anamnesisRepository;
		}

		public async Task<List<AnamnesisFieldDAO>> GetFieldsWithTemplatesByDoctorId(int doctorID)
		{
			using var con=_unitOfWork.BeginConnection();
			var result = await _anamnesisRepository.GetFieldsWithTemplatesByDoctorId(doctorID);
			return result;
		}
		public async  Task<int> AddAnamnesis(AddAnamnesisDTO dto)
		{
			using var transaction = _unitOfWork.BeginTransaction();
			try
			{
				int anamnesisId = await _anamnesisRepository.AddAnamnesisAsync(new()
				{
					doctorId = dto.DoctorId,
					patientCardId = dto.CardId,
					createDate = DateTime.Now
				});
				foreach (var item in dto.Fields)
				{
					if(item.anamnesisFieldId>0 && !string.IsNullOrWhiteSpace(item.answerText))
					{
						await _anamnesisRepository.AddAnamnesisAnswerAsync(new()
						{
							anamnesisFieldId = item.anamnesisFieldId,
							anamnesisId = anamnesisId,
							answerText = item.answerText
						});
					}
					else
					{
						return 0;
					}
				}
				_unitOfWork.SaveChanges();
				return anamnesisId;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"error add anamnesis {ex.Message}");
				transaction.Rollback();
				return 0;
			}
		}

		public async Task<List<AnamnesisDAO>> GetAnamnesisByCardId(int cardId)
		{
			using var con = _unitOfWork.BeginConnection();
			var result= await _anamnesisRepository.GetAnamnesisByCardId(cardId);
			return result;
		}
	}
}
