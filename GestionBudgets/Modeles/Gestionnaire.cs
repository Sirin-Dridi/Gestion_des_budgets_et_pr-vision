using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Gestionnaire : Utilisateur
    {
        public List<Budget> Budgets { get; set; }
        public List<Ajustement> Ajustements { get; set; }

        public Gestionnaire(int id, string nom, string email, string hash) : base(id, nom, email, hash)
        {
            Budgets = new List<Budget>();
            Ajustements = new List<Ajustement>();
        }

        public Budget creerBudget(int id, string libelle, DateTime debut, DateTime fin)
        {
            Budget budget = new Budget(id, libelle, debut, fin);
            budget.Gestionnaire = this;
            Budgets.Add(budget);
            return budget;
        }

        public void saisirTransaction(LigneBudgetaire ligne, Transaction transaction)
        {
            ligne.Transactions.Add(transaction);
            transaction.Ligne = ligne;
            ligne.actualiserMontantRealise();
        }

        public Prevision etablirPrevision(Budget budget, Prevision prevision, MethodePrevision methode)
        {
            prevision.Methode = methode;
            prevision.Budget = budget;
            budget.Previsions.Add(prevision);
            prevision.calculer();
            return prevision;
        }

        public List<Ecart> consulterEcarts(Budget budget)
        {
            List<Ecart> ecarts = new List<Ecart>();
            foreach (LigneBudgetaire ligne in budget.Lignes)
            {
                ecarts.Add(ligne.calculerEcart());
            }
            return ecarts;
        }

        public Ajustement ajusterBudget(int id, LigneBudgetaire ligne, decimal montant, string motif)
        {
            Ecart ecart = ligne.calculerEcart();
            Ajustement ajustement = new Ajustement(id, ligne, ecart, montant, motif);
            ajustement.appliquer();
            Ajustements.Add(ajustement);
            return ajustement;
        }
    }
}
