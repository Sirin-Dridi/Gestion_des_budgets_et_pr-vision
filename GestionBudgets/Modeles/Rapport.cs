using System;
using System.Collections.Generic;

namespace GestionBudgets
{
public class Rapport
    {
        private int idRapport;
        private DateTime dateGeneration;
        private string periode;
        private string format;

        public Budget Budget { get; set; }
        public Rapport(int id, Budget budget, string periode)
        {
            idRapport = id;
            Budget = budget;
            this.periode = periode;
            format = "PDF";
        }

        public void generer()
        {
            dateGeneration = DateTime.Now;
            Console.WriteLine("Rapport : " + periode + " - Budget : " + Budget.calculerTotal());
        }

        public void exporterPDF()
        {
            format = "PDF";
            Console.WriteLine("Format sélectionné : PDF");
        }

        public void exporterExcel()
        {
            format = "Excel";
            Console.WriteLine("Format sélectionné : Excel");
        }
    }
}
