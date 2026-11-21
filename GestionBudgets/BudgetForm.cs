using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace GestionBudgets;

internal sealed partial class BudgetForm : Form
{
    private readonly Budget budget;
    private readonly bool nouveau;
    private readonly TextBox libelle = new() { Dock = DockStyle.Fill, MaxLength = 120 };
    private readonly DateTimePicker debut = new() { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };
    private readonly DateTimePicker fin = new() { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };
    private readonly DataGridView lignes = new()
    {
        Dock = DockStyle.Fill,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = SystemColors.Window,
        SelectionMode = DataGridViewSelectionMode.CellSelect
    };

    public BudgetForm(Budget budget, bool nouveau)
    {
        this.budget = budget;
        this.nouveau = nouveau;
        Text = nouveau ? "Nouveau budget" : "Modifier le budget";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(580, 420);
        ClientSize = new Size(880, 580);
        Font = new Font("Segoe UI", 10);

        var disposition = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 7 };
        disposition.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        disposition.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 4; i++)
            disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        disposition.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        disposition.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var onglets = new TabControl { Dock = DockStyle.Fill };
        var plan = new TabPage("Planification");
        plan.Controls.Add(disposition);
        onglets.TabPages.Add(plan);
        Controls.Add(onglets);
        if (!nouveau)
            AjouterOngletsSuivi(onglets);

        AjouterChamp(disposition, "&Libellé", libelle, 0);
        AjouterChamp(disposition, "&Début", debut, 1);
        AjouterChamp(disposition, "&Fin", fin, 2);
        libelle.Text = budget.Libelle;
        debut.Value = budget.DateDebut;
        fin.Value = budget.DateFin;

        var titreLignes = new Label
        {
            Text = "Lignes budgétaires (facultatif)", AutoSize = true, Padding = new Padding(0, 10, 0, 4)
        };
        disposition.Controls.Add(titreLignes, 0, 3);
        disposition.SetColumnSpan(titreLignes, 2);

        lignes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Catégorie", MaxInputLength = 120 });
        lignes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Montant prévu (DT)", MaxInputLength = 24 });
        foreach (var ligne in budget.Lignes.OrderBy(l => l.IdLigne))
        {
            int index = lignes.Rows.Add(ligne.Categorie, ligne.MontantPrevu.ToString("0.00", CultureInfo.CurrentCulture));
            lignes.Rows[index].Tag = ligne.IdLigne;
        }
        disposition.Controls.Add(lignes, 0, 4);
        disposition.SetColumnSpan(lignes, 2);

        if (!nouveau)
            AjouterChamp(disposition, "Motif si un montant prévu change", motifAjustement, 5);

        var actions = new FlowLayoutPanel
        {
            AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };
        var enregistrer = new Button { Text = "Enregistrer", AutoSize = true };
        var annuler = new Button { Text = "Fermer", AutoSize = true, DialogResult = DialogResult.Cancel };
        var retirer = new Button { Text = "Retirer la ligne", AutoSize = true };
        actions.Controls.AddRange([enregistrer, annuler, retirer]);
        disposition.Controls.Add(actions, 0, 6);
        disposition.SetColumnSpan(actions, 2);
        AcceptButton = enregistrer;
        onglets.SelectedIndexChanged += (_, _) => AcceptButton = onglets.SelectedIndex == 0 ? enregistrer : null;
        CancelButton = annuler;
        enregistrer.Click += (_, _) => Enregistrer();
        retirer.Click += (_, _) =>
        {
            if (lignes.CurrentRow is { IsNewRow: false } ligne)
                lignes.Rows.Remove(ligne);
        };
    }

    private static void AjouterChamp(TableLayoutPanel disposition, string texte, Control champ, int index)
    {
        disposition.Controls.Add(new Label { Text = texte, AutoSize = true, Anchor = AnchorStyles.Left }, 0, index);
        disposition.Controls.Add(champ, 1, index);
    }

    private void Enregistrer()
    {
        if (!lignes.EndEdit())
            return;

        string nom = libelle.Text.Trim();
        if (nom.Length == 0 || fin.Value.Date < debut.Value.Date)
        {
            MessageBox.Show(this, "Saisissez un libellé et une date de fin au moins égale à la date de début.", Text);
            return;
        }

        var saisies = new List<(int Id, string Categorie, decimal Montant)>();
        foreach (DataGridViewRow ligne in lignes.Rows)
        {
            if (ligne.IsNewRow)
                continue;

            string categorie = Convert.ToString(ligne.Cells[0].Value)?.Trim() ?? "";
            string texteMontant = Convert.ToString(ligne.Cells[1].Value)?.Trim() ?? "";
            if (categorie.Length == 0 && texteMontant.Length == 0 && ligne.Tag == null)
                continue;

            const NumberStyles formatMontant = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            if (categorie.Length == 0 ||
                !decimal.TryParse(texteMontant, formatMontant, CultureInfo.CurrentCulture, out decimal montant) ||
                montant < 0 || montant > 999_999_999_999m || decimal.Round(montant, 2) != montant)
            {
                string separateur = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                MessageBox.Show(this, $"Ligne {ligne.Index + 1} : saisissez une catégorie et un montant positif ou nul, avec deux décimales au maximum (ex. 125{separateur}50).", Text);
                return;
            }

            saisies.Add((ligne.Tag is int id ? id : 0, categorie, montant));
        }

        try
        {
            using var contexte = new BudgetContext();
            var cible = nouveau
                ? new Budget(0, nom, debut.Value.Date, fin.Value.Date)
                : contexte.Budgets.Include(b => b.Lignes).ThenInclude(l => l.Transactions)
                    .Include(b => b.Lignes).ThenInclude(l => l.Ajustements).AsSplitQuery().SingleOrDefault(b => b.IdBudget == budget.IdBudget);
            if (cible == null || (!nouveau && !cible.Lignes.Select(l => l.IdLigne).ToHashSet()
                    .SetEquals(budget.Lignes.Select(l => l.IdLigne))))
            {
                MessageBox.Show(this, "Le budget a changé. Fermez cette fenêtre et ouvrez-le de nouveau.", Text);
                return;
            }


            var existantes = cible.Lignes.ToDictionary(l => l.IdLigne);
            var conserves = saisies.Select(l => l.Id).ToHashSet();
            var retirees = existantes.Values.Where(l => !conserves.Contains(l.IdLigne)).ToList();
            if (retirees.Any(l => l.Transactions.Count > 0 || l.Ajustements.Count > 0))
            {
                MessageBox.Show(this, "Une ligne avec des dépenses ou un historique ne peut pas être retirée.", Text);
                return;
            }
            if (cible.Lignes.SelectMany(l => l.Transactions).Any(t =>
                t.DateTransaction < debut.Value.Date || t.DateTransaction > fin.Value.Date))
            {
                MessageBox.Show(this, "La période doit inclure les dépenses déjà enregistrées.", Text);
                return;
            }

            string motif = motifAjustement.Text.Trim();
            if (motif.Length == 0 && saisies.Any(s => s.Id != 0 && existantes[s.Id].MontantPrevu != s.Montant))
            {
                MessageBox.Show(this, "Indiquez le motif du changement de montant prévu.", Text);
                motifAjustement.Focus();
                return;
            }

            cible.ModifierDetails(nom, debut.Value.Date, fin.Value.Date);
            foreach (var ligne in retirees)
                cible.Lignes.Remove(ligne);

            foreach (var saisie in saisies)
            {
                if (saisie.Id == 0)
                    cible.ajouterLigne(new LigneBudgetaire(0, saisie.Categorie, saisie.Montant));
                else
                {
                    var ligne = existantes[saisie.Id];
                    ligne.AjusterMontant(saisie.Montant, motif);
                    ligne.ModifierDetails(saisie.Categorie, saisie.Montant);
                }
            }

            if (nouveau)
                contexte.Budgets.Add(cible);
            contexte.SaveChanges();
            DialogResult = DialogResult.OK;
        }
        catch (Exception erreur)
        {
            MessageBox.Show(this, $"Impossible d'enregistrer le budget.\n\n{erreur.GetBaseException().Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}




