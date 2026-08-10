using System;
using System.Windows;
using System.Windows.Threading;

namespace NimbusChat
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Registered before base.OnStartup(): StartupUri creates MainWindow
            // synchronously inside base.OnStartup(), before the Dispatcher
            // message loop is pumping. AppDomain.UnhandledException is the only
            // one of these two that can still fire for exceptions at that point.
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowFatalErrorMessage(e.Exception);

            // Runs on the UI thread and is cancelable: mark it handled so the
            // app keeps running instead of crashing with the raw .NET dialog.
            e.Handled = true;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Can fire on any thread and is NOT cancelable - the CLR
            // terminates the process right after this returns regardless.
            // Best effort: show the user something readable before that happens.
            ShowFatalErrorMessage(e.ExceptionObject as Exception);
        }

        private static void ShowFatalErrorMessage(Exception ex)
        {
            try
            {
                var detail = ex != null ? Environment.NewLine + Environment.NewLine + ex.Message : string.Empty;
                var message = "NimbusChat ran into an unexpected problem." + detail;

                var dispatcher = Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(() => AppMessageBox.Show(message, "NimbusChat - Unexpected Error", AppMessageBoxIcon.Error));
                }
                else
                {
                    AppMessageBox.Show(message, "NimbusChat - Unexpected Error", AppMessageBoxIcon.Error);
                }
            }
            catch
            {
                // Showing the dialog itself failed - don't throw a second
                // exception out of an exception handler.
            }
        }
    }
}
