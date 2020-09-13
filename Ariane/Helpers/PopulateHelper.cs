using Ariane.Model;
using System;
using System.Collections.Generic;
namespace Ariane.Helpers
{
    static class PopulateHelper
    {
        internal static IEnumerable<Process> MockMeasureSettingsForReport()
        {
            var p = new Process
            {
                DisplayName = "test",
                MeasureSettings = new List<MeasureSetting> { new MeasureSetting()
                {
                    Name = "measure name",
                    ThresholdMaxTimeInSec = 25,
                    MeasuredUnits = new[]{
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 5,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 14,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 14,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 14,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 14,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 17,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 20,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 20,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 20,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                        },
                        new MeasuredUnit{
                            ElapseTimeInSeconds = 60,
                            MatchKey = "fas",
                            StartTime = DateTime.Now,
                            StopTime = DateTime.Now.AddMinutes(10)
                            }
                        }
                    }
                }

            };

            return new List<Process> { p, p};
        }

    }
}
