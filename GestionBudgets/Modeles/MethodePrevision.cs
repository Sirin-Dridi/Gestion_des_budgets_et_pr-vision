using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class MethodePrevision
    {
        private int idMethode;
        private string nom;
        private string description;

        public MethodePrevision(int id, string nom, string description)
        {
            idMethode = id;
            this.nom = nom;
            this.description = description;
        }

        public decimal estimer(Budget budget)
        {
            return budget.calculerTotal();
        }
    }
}
