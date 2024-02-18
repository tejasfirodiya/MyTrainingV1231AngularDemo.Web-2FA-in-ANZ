using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTrainingV1231AngularDemo.MultiTenancy.HostDashboard.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}