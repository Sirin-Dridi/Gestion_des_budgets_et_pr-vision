using Microsoft.EntityFrameworkCore;

namespace GestionBudgets;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BudgetContext.CheminBase));
            using var contexte = new BudgetContext();
            contexte.Database.Migrate();
        }
        catch (Exception erreur)
        {
            MessageBox.Show($"Impossible d'ouvrir la base de données.\n\n{erreur.Message}",
                "Gestion des budgets", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new ListeBudgetsForm());
    }
}

