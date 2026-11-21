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

        public int IdLigne { get { return idLigne; } private set { idLigne = value; } }
        public string Categorie { get { return categorie; } private set { categorie = value; } }
        public decimal MontantPrevu { get { return montantPrevu; } set { montantPrevu = value; } }
        public decimal MontantRealise { get { return montantRealise; } }
        public Budget Budget { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Ecart> Ecarts { get; set; }
        public List<Ajustement> Ajustements { get; set; }

        private LigneBudgetaire() : this(0, "", 0)
        {
        }

        internal void ModifierDetails(string nom, decimal montant)
        {
            Categorie = nom;
            MontantPrevu = montant;
        }

        public LigneBudgetaire(int id, string categorie, decimal montantPrevu)
        {
            idLigne = id;
            this.categorie = categorie;
            this.montantPrevu = montantPrevu;
            Transactions = new List<Transaction>();
            Ecarts = new List<Ecart>();
            Ajustements = new List<Ajustement>();
        }

        public void AjusterMontant(decimal montant, string motif)
        {
            if (montant == MontantPrevu)
                return;

            actualiserMontantRealise();
            var ajustement = new Ajustement(0, this, calculerEcart(), montant, motif);
            ajustement.appliquer();
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

