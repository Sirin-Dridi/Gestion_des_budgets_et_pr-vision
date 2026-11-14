using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Prevision
    {
        private int idPrevision;
        private DateTime dateCreation;
        private decimal montantEstime;
        private string periode;

        public MethodePrevision Methode { get; set; }
        public Budget Budget { get; set; }
        public decimal MontantEstime { get { return montantEstime; } }

        public Prevision(int id, string periode)
        {
            idPrevision = id;
            dateCreation = DateTime.Today;
            this.periode = periode;
        }

        public decimal calculer()
        {
            if (Methode != null && Budget != null)
                montantEstime = Methode.estimer(Budget);
            return montantEstime;
        }

        public decimal actualiser()
        {
            return calculer();
        }
    }
}
