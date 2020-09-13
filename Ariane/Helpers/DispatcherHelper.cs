using System;
using System.Windows;

namespace Ariane.Helpers
{
    internal static class DispatcherHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TD"> Data collection</typeparam>
        /// <typeparam name="TS"> SelectedData collection</typeparam>
        /// <typeparam name="TR"> Return type</typeparam>
        /// <param name="function"> Function where data are getting injected</param>
        /// <param name="parameters"> input params</param>
        public static void ExecuteActionToUI<TD, TS, TR>(Func<TD, TS, TR> function, params object[] parameters)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Delegate d = function;

                d.DynamicInvoke(parameters);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(function, parameters);
            }
        }

        public static void ExecuteActionToUI(Delegate del, params object[] parameters)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Delegate d = del;

                d.DynamicInvoke(parameters);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(del, parameters);
            }
        }

        public static void ExecuteActionToUIAsync<TD, TR>(Func<TD, TR> function, params object[] parameters)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Delegate d = function;
                d.DynamicInvoke(parameters);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(function, parameters);
            }
        }

        public static void ExecuteActionToUI<TD, TR>(Func<TD, TR> function, params object[] parameters)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Delegate d = function;

                d.DynamicInvoke(parameters);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(function, parameters);
            }
        }
    }
}
