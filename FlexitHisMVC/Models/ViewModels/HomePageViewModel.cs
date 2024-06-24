using System;
using Medicloud.Models.DTO;

namespace Medicloud.Models.ViewModels
{
	public class HomePageViewModel
	{
		public PatientCardStatisticsDTO patientCardStatisticsDTO { get; set; }
		public PatientStatisticsDTO patientStatisticsDTO { get; set; }
		public PaymentOperationStatisticsDTO paymentOperationStatisticsDTO { get; set; }
		public List<ServiceStatisticsDTO> top5sellingServiceStatistics { get; set; }
		public List<DailyIncomeStatistics> dailyIncomeStatistics { get; set; }
		public List<WeeklyIncomeStatisticsDTO> weeklyIncomeStatistics { get; set; }
	}
}

