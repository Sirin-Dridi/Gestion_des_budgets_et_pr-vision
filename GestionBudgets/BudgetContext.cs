using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GestionBudgets;

public sealed class BudgetContext : DbContext
{
    public static string CheminBase => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GestionBudgets", "budgets.sqlite");

    private readonly string chemin;

    public BudgetContext() : this(CheminBase)
    {
    }

    public BudgetContext(string chemin)
    {
        this.chemin = chemin;
    }

    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(new SqliteConnectionStringBuilder
        {
            DataSource = chemin,
            ForeignKeys = true
        }.ToString());
    }

    protected override void OnModelCreating(ModelBuilder modele)
    {
        var budget = modele.Entity<Budget>();
        budget.ToTable("Budgets");
        budget.HasKey(b => b.IdBudget);
        budget.Property(b => b.IdBudget).ValueGeneratedOnAdd();
        budget.Property(b => b.Libelle).IsRequired().HasMaxLength(120);
        budget.Ignore(b => b.MontantTotal);
        budget.Ignore(b => b.Gestionnaire);
        budget.Ignore(b => b.Previsions);
        budget.Ignore(b => b.Validations);
        budget.Ignore(b => b.Rapports);
        budget.HasMany(b => b.Lignes).WithOne(l => l.Budget)
            .HasForeignKey("BudgetId").IsRequired().OnDelete(DeleteBehavior.Cascade);

        var ligne = modele.Entity<LigneBudgetaire>();
        ligne.ToTable("LignesBudgetaires");
        ligne.HasKey(l => l.IdLigne);
        ligne.Property(l => l.IdLigne).ValueGeneratedOnAdd();
        ligne.Property(l => l.Categorie).IsRequired().HasMaxLength(120);
        ligne.Ignore(l => l.MontantRealise);
        ligne.HasMany(l => l.Transactions).WithOne(t => t.Ligne)
            .HasForeignKey("LigneId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        ligne.Ignore(l => l.Ecarts);
        ligne.HasMany(l => l.Ajustements).WithOne(a => a.Ligne)
            .HasForeignKey("LigneId").IsRequired().OnDelete(DeleteBehavior.Cascade);

        var transaction = modele.Entity<Transaction>();
        transaction.ToTable("Transactions");
        transaction.HasKey(t => t.IdTransaction);
        transaction.Property(t => t.IdTransaction).HasField("idTransaction").ValueGeneratedOnAdd();
        transaction.Property(t => t.Montant).HasField("montant");
        transaction.Property(t => t.Type).HasField("type");
        transaction.Property(t => t.Description).HasField("description").IsRequired().HasMaxLength(160);
        transaction.Property(t => t.DateTransaction).HasField("dateTransaction");
        transaction.Ignore(t => t.Justificatifs);

        var ajustement = modele.Entity<Ajustement>();
        ajustement.ToTable("Ajustements");
        ajustement.HasKey(a => a.IdAjustement);
        ajustement.Property(a => a.IdAjustement).HasField("idAjustement").ValueGeneratedOnAdd();
        ajustement.Property(a => a.DateAjustement).HasField("dateAjustement");
        ajustement.Property(a => a.AncienMontant).HasField("ancienMontant");
        ajustement.Property(a => a.NouveauMontant).HasField("nouveauMontant");
        ajustement.Property(a => a.Motif).HasField("motif").IsRequired().HasMaxLength(200);
        ajustement.Ignore(a => a.Ecart);
    }
}


