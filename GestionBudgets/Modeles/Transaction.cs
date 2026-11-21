using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Transaction
    {
        private int idTransaction;
        private DateTime dateTransaction;
        private decimal montant;
        private string description;
        private TypeTransaction type;

        public int IdTransaction { get { return idTransaction; } }
        public decimal Montant { get { return montant; } }
        public TypeTransaction Type { get { return type; } }
        public string Description { get { return description; } }
        public DateTime DateTransaction { get { return dateTransaction; } }
        public LigneBudgetaire Ligne { get; set; }
        public List<Justificatif> Justificatifs { get; set; }

        private Transaction() : this(0, 0, "", TypeTransaction.DEPENSE)
        {
        }

        public Transaction(decimal montant, string description, DateTime date)
            : this(0, montant, description, TypeTransaction.DEPENSE)
        {
            dateTransaction = date.Date;
        }

        public Transaction(int id, decimal montant, string description, TypeTransaction type)
        {
            idTransaction = id;
            dateTransaction = DateTime.Today;
            this.montant = montant;
            this.description = description;
            this.type = type;
            Justificatifs = new List<Justificatif>();
        }

        public void enregistrer(LigneBudgetaire ligne)
        {
            ligne.Transactions.Add(this);
            Ligne = ligne;
            ligne.actualiserMontantRealise();
        }

        public void modifier(decimal montant, string description)
        {
            this.montant = montant;
            this.description = description;
            if (Ligne != null)
                Ligne.actualiserMontantRealise();
        }

        public void supprimer()
        {
            if (Ligne != null)
            {
                LigneBudgetaire ancienneLigne = Ligne;
                ancienneLigne.Transactions.Remove(this);
                Ligne = null;
                ancienneLigne.actualiserMontantRealise();
            }
        }

        public void joindreJustificatif(Justificatif justificatif)
        {
            Justificatifs.Add(justificatif);
            justificatif.Transaction = this;
        }
    }
}

