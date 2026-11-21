using Microsoft.EntityFrameworkCore;

namespace GestionBudgets;

internal sealed partial class BudgetForm
{
    private readonly TextBox motifAjustement = new() { Dock = DockStyle.Fill, MaxLength = 200 };
    private readonly ComboBox ligneDepense = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 185 };
    private readonly DateTimePicker dateDepense = new() { Format = DateTimePickerFormat.Short, Width = 115 };
    private readonly TextBox descriptionDepense = new() { PlaceholderText = "Description", Width = 165, MaxLength = 160 };
    private readonly NumericUpDown montantDepense = new() { DecimalPlaces = 2, Maximum = 999_999_999_999m, Width = 120 };
    private readonly DataGridView synthese = CreerGrille();
    private readonly DataGridView depenses = CreerGrille();
    private readonly DataGridView historique = CreerGrille();
    private readonly Button ajouterDepense = new() { Text = "Ajouter la dépense", AutoSize = true };
    private readonly ServicePrevision servicePrevision = new();

    private void AjouterOngletsSuivi(TabControl onglets)
    {
        AjouterColonnes(synthese, "Catégorie", "Prévu (DT)", "Réalisé (DT)", "Écart (DT)", "Prévision fin (DT)");
        AjouterColonnes(depenses, "Date", "Catégorie", "Description", "Montant (DT)");
        AjouterColonnes(historique, "Date", "Catégorie", "Avant (DT)", "Après (DT)", "Motif");
        var saisie = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 4, 0, 4) };
        ligneDepense.AccessibleName = "Ligne budgétaire";
        dateDepense.AccessibleName = "Date de la dépense";
        montantDepense.AccessibleName = "Montant de la dépense en DT";
        dateDepense.Value = DateTime.Today < budget.DateDebut ? budget.DateDebut
            : DateTime.Today > budget.DateFin ? budget.DateFin : DateTime.Today;
        saisie.Controls.AddRange([Etiquette("Ligne"), ligneDepense, Etiquette("Date"), dateDepense,
            descriptionDepense, Etiquette("Montant (DT)"), montantDepense, ajouterDepense]);
        ajouterDepense.Click += (_, _) => EnregistrerDepense();

        onglets.TabPages.Add(CreerOnglet("Suivi et prévisions", synthese,
            Etiquette("Écart = réalisé − prévu. La prévision prolonge le rythme moyen des dépenses jusqu’à la fin du budget.")));
        onglets.TabPages.Add(CreerOnglet("Dépenses", depenses, saisie));
        onglets.TabPages.Add(CreerOnglet("Historique des ajustements", historique,
            Etiquette("Pour ajuster un montant, modifiez la ligne et indiquez le motif dans Planification.")));
        Shown += (_, _) => ChargerSuivi();
    }

    private static Label Etiquette(string texte) => new() { Text = texte, AutoSize = true, Padding = new Padding(0, 5, 0, 4) };

    private static DataGridView CreerGrille() => new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AllowUserToDeleteRows = false, RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = SystemColors.Window
    };

    private static void AjouterColonnes(DataGridView grille, params string[] titres)
    {
        foreach (string titre in titres)
            grille.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = titre, SortMode = DataGridViewColumnSortMode.NotSortable });
    }

    private static TabPage CreerOnglet(string titre, DataGridView grille, Control entete)
    {
        var page = new TabPage(titre);
        var disposition = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 1, RowCount = 2 };
        disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        disposition.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        disposition.Controls.Add(entete, 0, 0);
        disposition.Controls.Add(grille, 0, 1);
        page.Controls.Add(disposition);
        return page;
    }

    private void ChargerSuivi()
    {
        try
        {
            using var contexte = new BudgetContext();
            var actuel = contexte.Budgets.AsNoTracking().Include(b => b.Lignes).ThenInclude(l => l.Transactions)
                .Include(b => b.Lignes).ThenInclude(l => l.Ajustements).AsSplitQuery()
                .Single(b => b.IdBudget == budget.IdBudget);
            var categories = actuel.Lignes.OrderBy(l => l.Categorie).ThenBy(l => l.IdLigne).ToList();
            int? id = (ligneDepense.SelectedItem as LigneBudgetaire)?.IdLigne;
            ligneDepense.DisplayMember = nameof(LigneBudgetaire.Categorie);
            ligneDepense.DataSource = categories;
            int index = categories.FindIndex(l => l.IdLigne == id);
            if (index >= 0)
                ligneDepense.SelectedIndex = index;
            ajouterDepense.Enabled = categories.Count > 0;

            synthese.Rows.Clear();
            foreach (var ligne in categories)
            {
                decimal realise = ligne.Transactions.Where(t => t.Type == TypeTransaction.DEPENSE).Sum(t => t.Montant);
                decimal ecart = new Ecart(0, ligne.MontantPrevu, realise).Difference;
                decimal? projection = servicePrevision.ProjeterDepenses(actuel, ligne, DateTime.Today);
                int row = synthese.Rows.Add(ligne.Categorie, ligne.MontantPrevu.ToString("N2"), realise.ToString("N2"),
                    ecart.ToString("+0.00;-0.00;0.00"), projection?.ToString("N2") ?? "Données insuffisantes");
                if (ecart > 0)
                    synthese.Rows[row].Cells[3].Style.ForeColor = Color.Firebrick;
            }

            depenses.Rows.Clear();
            foreach (var t in categories.SelectMany(l => l.Transactions.Select(t => (l.Categorie, Transaction: t)))
                .Where(e => e.Transaction.Type == TypeTransaction.DEPENSE)
                .OrderByDescending(e => e.Transaction.DateTransaction).ThenByDescending(e => e.Transaction.IdTransaction))
                depenses.Rows.Add(t.Transaction.DateTransaction.ToShortDateString(), t.Categorie,
                    t.Transaction.Description, t.Transaction.Montant.ToString("N2"));

            historique.Rows.Clear();
            foreach (var entree in categories.SelectMany(l => l.Ajustements.Select(a => (l.Categorie, Ajustement: a)))
                .OrderByDescending(e => e.Ajustement.DateAjustement).ThenByDescending(e => e.Ajustement.IdAjustement))
            {
                var a = entree.Ajustement;
                historique.Rows.Add(a.DateAjustement.ToString("g"), entree.Categorie, a.AncienMontant.ToString("N2"),
                    a.NouveauMontant.ToString("N2"), a.Motif);
            }
        }
        catch (Exception erreur)
        {
            MessageBox.Show(this, $"Impossible de charger le suivi.\n\n{erreur.GetBaseException().Message}", Text);
        }
    }

    private void EnregistrerDepense()
    {
        string description = descriptionDepense.Text.Trim();
        DateTime date = dateDepense.Value.Date;
        if (ligneDepense.SelectedItem is not LigneBudgetaire selection || description.Length == 0 || decimal.Round(montantDepense.Value, 2) <= 0)
        {
            MessageBox.Show(this, "Sélectionnez une ligne et saisissez une description et un montant supérieur à zéro.", Text);
            return;
        }

        try
        {
            using var contexte = new BudgetContext();
            var ligne = contexte.Set<LigneBudgetaire>().Include(l => l.Budget)
                .SingleOrDefault(l => l.IdLigne == selection.IdLigne && l.Budget.IdBudget == budget.IdBudget);
            if (ligne == null || date < ligne.Budget.DateDebut || date > ligne.Budget.DateFin || date > DateTime.Today)
            {
                MessageBox.Show(this, "Sélectionnez une ligne existante et une date passée ou actuelle dans la période du budget.", Text);
                return;
            }

            var depense = new Transaction(decimal.Round(montantDepense.Value, 2), description, date) { Ligne = ligne };
            contexte.Add(depense);
            contexte.SaveChanges();
            descriptionDepense.Clear();
            montantDepense.Value = 0;
            ChargerSuivi();
        }
        catch (Exception erreur)
        {
            MessageBox.Show(this, $"Impossible d'enregistrer la dépense.\n\n{erreur.GetBaseException().Message}", Text);
        }
    }
}

