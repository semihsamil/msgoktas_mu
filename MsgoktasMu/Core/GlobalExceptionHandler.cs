namespace MsgoktasMu.Core;

internal static class GlobalExceptionHandler
{
    public static void Register()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ShowFatalError("Program hatası", e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                ShowFatalError("Kritik hata", ex);
        };
    }

    private static void ShowFatalError(string title, Exception ex)
    {
        var message = ex.Message;
        if (string.IsNullOrWhiteSpace(message))
            message = "Beklenmeyen bir hata oluştu. Program kapanmadan devam edebilirsiniz.";

        MessageBox.Show(
            $"{message}\n\nDetay: {ex.GetType().Name}",
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
