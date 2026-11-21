using Microsoft.EntityFrameworkCore;

namespace GestionBudgets;

internal sealed class ListeBudgetsForm : Form
{
    private readonly DataGridView grille = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        BackgroundColor = SystemColors.Window
    };
    private readonly Button modifier = new() { Text = "Ouvrir le budget", AutoSize = true, Enabled = false };
    private readonly Button supprimer = new() { Text = "Supprimer", AutoSize = true, Enabled = false };
    private readonly Label etat = new() { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };

    private readonly TextBox recherche = new() { PlaceholderText = "Rechercher un budget", AccessibleName = "Rechercher un budget", Width = 220 };
    private List<BudgetAffiche> budgetsCharges = [];

    public ListeBudgetsForm()
    {
        Text = "Gestion des budgets et prévisions";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 380);
        ClientSize = new Size(880, 500);
        Font = new Font("Segoe UI", 10);

        var disposition = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1, RowCount = 4 };
        disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        disposition.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(disposition);
        disposition.Controls.Add(new Label
        {
            Text = "Budgets", AutoSize = true, Font = new Font(Font.FontFamily, 22, FontStyle.Bold)
        }, 0, 0);

        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 8) };
        var ajouter = new Button { Text = "Nouveau budget", AutoSize = true };
        actions.Controls.AddRange([ajouter, modifier, supprimer, recherche]);
        disposition.Controls.Add(actions, 0, 1);
        disposition.Controls.Add(grille, 0, 2);
        disposition.Controls.Add(etat, 0, 3);

        ajouter.Click += (_, _) => OuvrirEditeur(null);
        modifier.Click += (_, _) => ModifierSelection();
        supprimer.Click += (_, _) => SupprimerSelection();
        grille.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) ModifierSelection(); };
        grille.SelectionChanged += (_, _) => ActualiserActions();
        recherche.TextChanged += (_, _) => AfficherBudgets();
        grille.CellFormatting += (_, e) =>
        {
            if (grille.Columns[e.ColumnIndex].Name == nameof(BudgetAffiche.Ecart) && e.Value is decimal ecart && ecart > 0)
                e.CellStyle.ForeColor = Color.Firebrick;
        };
        Shown += (_, _) => Charger();
    }

    private BudgetAffiche Selection => grille.CurrentRow?.DataBoundItem as BudgetAffiche;

    private void ActualiserActions()
    {
        modifier.Enabled = supprimer.Enabled = Selection != null;
    }

    private void Charger()
    {
        try
        {
            using var contexte = new BudgetContext();
            var budgets = contexte.Budgets.AsNoTracking().Include(b => b.Lignes).ThenInclude(l => l.Transactions)
                .OrderBy(b => b.Libelle).ThenBy(b => b.IdBudget).ToList();
            budgetsCharges = budgets.Select(b => new BudgetAffiche(
                b.IdBudget, b.Libelle, b.DateDebut, b.DateFin, b.calculerTotal(),
                b.Lignes.SelectMany(l => l.Transactions).Where(t => t.Type == TypeTransaction.DEPENSE).Sum(t => t.Montant))).ToList();
            AfficherBudgets();
        }
        catch (Exception erreur)
        {
            AfficherErreur("Impossible de charger les budgets.", erreur);
        }
    }

    private void AfficherBudgets()
    {
        string filtre = recherche.Text.Trim();
        var visibles = budgetsCharges.Where(b => b.Libelle.Contains(filtre, StringComparison.CurrentCultureIgnoreCase)).ToList();
        grille.DataSource = visibles;
        grille.Columns[nameof(BudgetAffiche.Id)].Visible = false;
        grille.Columns[nameof(BudgetAffiche.Libelle)].HeaderText = "Libellé";
        grille.Columns[nameof(BudgetAffiche.Debut)].HeaderText = "Début";
        grille.Columns[nameof(BudgetAffiche.Debut)].DefaultCellStyle.Format = "dd/MM/yyyy";
        grille.Columns[nameof(BudgetAffiche.Fin)].DefaultCellStyle.Format = "dd/MM/yyyy";
        grille.Columns[nameof(BudgetAffiche.Total)].HeaderText = "Total prévu (DT)";
        grille.Columns[nameof(BudgetAffiche.Total)].DefaultCellStyle.Format = "N2";
        grille.Columns[nameof(BudgetAffiche.Realise)].HeaderText = "Réalisé (DT)";
        grille.Columns[nameof(BudgetAffiche.Ecart)].HeaderText = "Écart (DT)";
        grille.Columns[nameof(BudgetAffiche.Realise)].DefaultCellStyle.Format = "N2";
        grille.Columns[nameof(BudgetAffiche.Ecart)].DefaultCellStyle.Format = "N2";
        etat.Text = visibles.Count == 0
            ? (budgetsCharges.Count == 0 ? "Aucun budget. Cliquez sur Nouveau budget pour commencer." : "Aucun résultat.")
            : $"{visibles.Count} budget(s)   |   Prévu {visibles.Sum(b => b.Total):N2} DT   |   Réalisé {visibles.Sum(b => b.Realise):N2} DT   |   Écart {visibles.Sum(b => b.Ecart):N2} DT";
        ActualiserActions();
    }

    private void ModifierSelection()
    {
        if (Selection is { } budget)
            OuvrirEditeur(budget.Id);
    }

    private void OuvrirEditeur(int? id)
    {
        try
        {
            using var contexte = new BudgetContext();
            var budget = id.HasValue
                ? contexte.Budgets.AsNoTracking().Include(b => b.Lignes).SingleOrDefault(b => b.IdBudget == id.Value)
                : new Budget(0, "", DateTime.Today, DateTime.Today.AddMonths(1));

            if (budget == null)
            {
                Charger();
                return;
            }

            using var formulaire = new BudgetForm(budget, !id.HasValue);
            formulaire.ShowDialog(this);
            Charger();
        }
        catch (Exception erreur)
        {
            AfficherErreur("Impossible d'ouvrir le budget.", erreur);
        }
    }

    private void SupprimerSelection()
    {
        if (Selection is not { } selection ||
            MessageBox.Show(this, $"Supprimer le budget « {selection.Libelle} » ainsi que ses lignes, dépenses et ajustements ?",
                "Confirmer la suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;

        try
        {
            using var contexte = new BudgetContext();
            var budget = contexte.Budgets.Find(selection.Id);
            if (budget != null)
            {
                contexte.Budgets.Remove(budget);
                contexte.SaveChanges();
            }
            Charger();
        }
        catch (DbUpdateConcurrencyException)
        {
            Charger();
        }
        catch (Exception erreur)
        {
            AfficherErreur("Impossible de supprimer le budget.", erreur);
        }
    }

    private void AfficherErreur(string message, Exception erreur)
    {
        MessageBox.Show(this, $"{message}\n\n{erreur.GetBaseException().Message}",
            Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private sealed record BudgetAffiche(int Id, string Libelle, DateTime Debut, DateTime Fin, decimal Total, decimal Realise)
    {
        public decimal Ecart => Realise - Total;
    }
}



