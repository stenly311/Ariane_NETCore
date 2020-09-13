using Ariane.Type.Configuration;
using Ariane.ViewModels;
using System.Collections.Generic;

namespace Ariane.Helpers
{
    public static class ConverterHelper
    {
        public static ProcessConfiguration ToProcessConfiguration(this ProcessViewModel processModel)
        {
            var mss = new List<MeasureSettingsConfiguration>();
            foreach (var item in processModel.MeasureSettings)
            {
                mss.Add(item.ToConfiguration());
            }
            return new ProcessConfiguration
            {
                Arguments = processModel.StartingArguments,
                DisplayName = processModel.DisplayName,
                MeasureSettings = mss,
                ProcessFileName = processModel.FileName,
                RootPath = processModel.RootFolderPath,
                LoggingSourceType = processModel.LoggingSourceType.ToString(),
                RabbitMQTopicName = processModel.RabbitMQTopicName
            };
        }

        public static ProcessViewModel ToViewModel(this ProcessConfiguration pc)
        {
            return new ProcessViewModel(pc);
        }

        public static MeasureSettingsConfiguration ToConfiguration(this MeasureSettingViewModel vm)
        {
            return new MeasureSettingsConfiguration
            {
                IsActive = vm.IsActive,
                Name = vm.Name,
                StartCode = vm.StartCode,
                StopCode = vm.StopCode,
                ThresholdMaxTimeInSec = vm.ThresholdMaxTimeInSec,
                UseContainsEvaluation = vm.UseContainsEvaluation,
                UseKeyLockedMatch = vm.UseKeyLockedMatch
            };
        }

        public static MeasureSettingViewModel ToViewModel(this MeasureSettingsConfiguration cn)
        {
            return new MeasureSettingViewModel(cn.Name)
            {
                IsActive = cn.IsActive,
                StartCode = cn.StartCode,
                StopCode = cn.StopCode,
                ThresholdMaxTimeInSec = cn.ThresholdMaxTimeInSec,
                UseContainsEvaluation = cn.UseContainsEvaluation,
                UseKeyLockedMatch = cn.UseKeyLockedMatch
            };
        }
    }
}
