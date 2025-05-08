using Dapper;
using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.Anamnesis
{
	public class AnamnesisRepository : IAnamnesisRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public AnamnesisRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		public async Task<int> AddAnamnesisAsync(AnamnesisDAO dao)
		{
			string AddSql = $@"
			INSERT INTO anamnesis
            (patientCardId,doctorId,createDate)
			VALUES (@{nameof(AnamnesisDAO.patientCardId)},
            @{nameof(AnamnesisDAO.doctorId)},
            @{nameof(AnamnesisDAO.createDate)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var transaction = _unitOfWork.GetTransaction();
			try
			{
				var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao, transaction);
				return newId;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error add anamnesis {ex.Message}");
				throw;
			}
		}

		public async Task<int> AddAnamnesisAnswerAsync(AnamnesisAnswerDAO dao)
		{
			string AddSql = $@"
			INSERT INTO anamnesis_answers
            (anamnesisId,anamnesisFieldId,answerText)
			VALUES (@{nameof(AnamnesisAnswerDAO.anamnesisId)},
            @{nameof(AnamnesisAnswerDAO.anamnesisFieldId)},
            @{nameof(AnamnesisAnswerDAO.answerText)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var transaction = _unitOfWork.GetTransaction();
			try
			{
				var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao, transaction);
				return newId;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error add anamnesis answer {ex.Message}");
				throw;
			}

		}


		public async Task<List<AnamnesisDAO>> GetAnamnesisByCardId(int cardId)
		{
			//			string sql = @"
			//SELECT 
			//    a.*,
			//	aa.anamnesisId as AnamnesisId,
			//    aa.*
			//FROM 
			//    anamnesis a
			//LEFT JOIN 
			//    anamnesis_answers aa ON a.id = aa.anamnesisId
			//WHERE a.isActive=1 AND a.patientCardId=@CardId
			//";




			//var anamnesisDict = new Dictionary<int, AnamnesisDAO>();
			//var con = _unitOfWork.GetConnection();

			//var result = await con.QueryAsync<AnamnesisDAO, AnamnesisAnswerDAO, AnamnesisDAO>(
			//	sql,
			//	(anamnesis, answer) =>
			//	{
			//		Console.WriteLine($"Anamnesis: {anamnesis?.id}, Answer: {answer?.id}");
			//		if (!anamnesisDict.TryGetValue(anamnesis.id, out var existingAnamnesis))
			//		{
			//			existingAnamnesis = anamnesis;
			//			existingAnamnesis.AnamnesisAnswers = new List<AnamnesisAnswerDAO>();
			//			anamnesisDict.Add(existingAnamnesis.id, existingAnamnesis);
			//		}

			//		if (answer != null && answer.id != 0)
			//		{
			//			existingAnamnesis.AnamnesisAnswers.Add(answer); // ← Doğru nesneye ekleme
			//		}

			//		return existingAnamnesis;
			//	},
			//	new { CardId = cardId },
			//	splitOn: "AnamnesisId"
			//);

			//return anamnesisDict.Values.ToList();




			string sql = @"
SELECT 
    a.id,
    a.doctorId,
    a.patientCardId,
    a.createDate
FROM anamnesis a
WHERE 
    a.patientCardId = @CardId AND a.isActive=1;
			";


			string answersQuery = @"
SELECT 
    aa.*
FROM anamnesis_answers aa
WHERE 
    aa.anamnesisId=@AnamnesisId;
			";
			var con = _unitOfWork.GetConnection();

			var anamnesis = await con.QueryAsync<AnamnesisDAO>(sql, new { CardId = cardId });
			if (anamnesis != null)
			{
				//Console.WriteLine("Count");
				foreach (var item in anamnesis)
				{
					var result= await con.QueryAsync<AnamnesisAnswerDAO>(answersQuery, new { AnamnesisId = item.id });
					item.AnamnesisAnswers=result.ToList();
				}
			}
			return anamnesis.ToList();
		}

		public async Task<List<AnamnesisFieldDAO>> GetFieldsWithTemplatesByDoctorId(int doctorID)
		{
			string query = $@"SELECT 
    af.*,
    aft.id AS TemplateId,
	aft.*
FROM 
    anamnesis_field af
LEFT JOIN 
    doctor_anamnesis_field_templates aft ON af.id = aft.field_id
WHERE 
    aft.doctor_id = @DoctorId OR aft.doctor_id is null
 ;
            ";

			var con = _unitOfWork.GetConnection();
			//var result =  con.Query<PatientDocDTO>(query, new { docID = docID, orgID = orgID });
			//return result.ToList();

			var fields = await con.QueryAsync<AnamnesisFieldDAO, AnamnesisTemplateDAO, AnamnesisFieldDAO>(
				query,
				(field, template) =>
				{
					field.Templates = new();
					field.Templates.Add(template);
					return field;
				},
				new { DoctorId = doctorID },
				splitOn: "TemplateId"
			);

			return fields.ToList();
		}
	}
}
