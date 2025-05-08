using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.AnamnesisTemplate
{
	public class AnamnesisTemplateRepository:IAnamnesisTemplateRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public AnamnesisTemplateRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(AnamnesisTemplateDAO dao)
		{
			string AddSql = $@"
			INSERT INTO doctor_anamnesis_field_templates
            (field_id,doctor_id,templateText)
			VALUES (@{nameof(AnamnesisTemplateDAO.field_id)},
            @{nameof(AnamnesisTemplateDAO.doctor_id)},
            @{nameof(AnamnesisTemplateDAO.templateText)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.BeginConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
