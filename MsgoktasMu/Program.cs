using MsgoktasMu.Data;
using MsgoktasMu.Forms;

namespace MsgoktasMu;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        LocalDatabase.Initialize();
        Application.Run(new HomeForm());
    }
}
