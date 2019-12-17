using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Attention, indispensable pour lire et ecrire un fichier 
using System.IO;

namespace ReductionFichierTexte
{
    class Program
    {

        public static void reduireFichier(string fichier_source, int ratio, string fichier_cible)
        {
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier source 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichier_source, encoding);

                // Création d'une instance de StreamWriter pour permettre l'ecriture de notre fichier cible
                StreamWriter monStreamWriter = File.CreateText(fichier_cible);

                int nbMots = 0;
                string mot = monStreamReader.ReadLine();

                // Lecture de tous les mots du fichier (un par lignes) 
                while (mot != null)
                {
                    nbMots++;
                    if (nbMots % ratio == 0)              // tous les "ratio" mots...
                        monStreamWriter.WriteLine(mot);   //... on écrit dans le fichier cible
                    mot = monStreamReader.ReadLine();

                }
                // Fermeture du StreamReader (attention très important) 
                monStreamReader.Close();
                // Fermeture du StreamWriter (attention très important) 
                monStreamWriter.Close();
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'opération :");
                Console.WriteLine(ex.Message);
            }
        }

        static void Main(string[] args)
        {
            string fichierSource = "dico_fr.txt";
            string fichierCible = "dico_reduit.txt";

            int ratio = 1000;

            Console.WriteLine(@"
But :
      Ce programme lit un fichier texte et n'en retient qu'une ligne sur n pour créer un nouveau fichier.
      En sortie, le fichier créé est donc une réduction du fichier d'entrée d'un certain ratio.

");



            reduireFichier(fichierSource, ratio, fichierCible);
            Console.WriteLine("Réduction terminée.");
            Console.ReadKey();


        }
    }
}
