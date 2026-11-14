using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class TableauDeBord
    {
        private string periode;
        public Budget BudgetAffiche { get; set; }

        public void afficherIndicateurs(Budget budget)
        {
            BudgetAffiche = budget;
            Console.WriteLine("Montant prévu : " + budget.calculerTotal());
        }

        public void filtrerParPeriode(string periode)
        {
            this.periode = periode;
        }

        public List<Ecart> visualiserEcarts()
        {
            List<Ecart> ecarts = new List<Ecart>();
            if (BudgetAffiche != null)
            {
                foreach (LigneBudgetaire ligne in BudgetAffiche.Lignes)
                    ecarts.Add(ligne.calculerEcart());
            }
            return ecarts;
        }
    }
}
