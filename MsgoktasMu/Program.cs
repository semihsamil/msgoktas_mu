using MsgoktasMu.Core;
using MsgoktasMu.Data;
using MsgoktasMu.Forms;

namespace MsgoktasMu;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        GlobalExceptionHandler.Register();

        try
        {
            LocalDatabase.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Veritabanı başlatılamadı:\n" + ex.Message,
                "Başlangıç Hatası",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new HomeForm());
    }
}
