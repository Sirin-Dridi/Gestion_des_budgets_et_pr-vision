using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class LigneBudgetaire
    {
        private int idLigne;
        private string categorie;
        private decimal montantPrevu;
        private decimal montantRealise;

        public int IdLigne { get { return idLigne; } }
        public string Categorie { get { return categorie; } }
        public decimal MontantPrevu { get { return montantPrevu; } set { montantPrevu = value; } }
        public decimal MontantRealise { get { return montantRealise; } }
        public Budget Budget { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Ecart> Ecarts { get; set; }
        public List<Ajustement> Ajustements { get; set; }

        public LigneBudgetaire(int id, string categorie, decimal montantPrevu)
        {
            idLigne = id;
            this.categorie = categorie;
            this.montantPrevu = montantPrevu;
            Transactions = new List<Transaction>();
            Ecarts = new List<Ecart>();
            Ajustements = new List<Ajustement>();
        }

        public void actualiserMontantRealise()
        {
            montantRealise = 0;
            foreach (Transaction t in Transactions)
            {
                if (t.Type == TypeTransaction.DEPENSE)
                    montantRealise += t.Montant;
                else
                    montantRealise -= t.Montant;
            }
        }

        public decimal calculerSolde()
        {
            return montantPrevu - montantRealise;
        }

        public Ecart calculerEcart()
        {
            Ecart ecart = new Ecart(Ecarts.Count + 1, montantPrevu, montantRealise);
            ecart.Ligne = this;
            Ecarts.Add(ecart);
            return ecart;
        }
    }
}
