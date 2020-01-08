using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Motus
{
    class Program
    {
        static void Main(string[] args)
        {
            // Affichage de début
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("          ~~~ MO-MO-MO-MOTUS ~~~          ");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("\nBy Coline BINET et Alban PERRIER G1 1A ENSC");

            // Joueur le son du début
            // JouerUnSon(@"E:\IPROG\Music\generique_intro.wav");

            // MENU 
            AfficherMenu();

            Console.ReadKey();

        }
        
        // Affichage du menu et permet d'appeler la fonction correspondant à ce que l'utilisateur choisi
        public static void AfficherMenu()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("                  MENU                    ");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("1: Nouvelle partie \n2: Statistiques \n3:Options \n4:Quitter");

            int caseSwitch = int.Parse(Console.ReadLine());

            // Permet d'appeler fonction permettant de réaliser l'action que l'utilisateur souhaite
            switch (caseSwitch)
            {
                case 1: // Nouvelle Partie
                    InstancierPartie();
                    break;
                case 2: // Statistiques

                    break;
                case 3: // Options

                    break;
                case 4: // Quitter
                    QuitterJeu();

                    break;
            }
        }

        // ---------------------------------------------------------------------------------- Nouvelle Partie---------------------------------------------------------------------------------- //
        // Initialise la partie en donnant le nom des dicos utilisés et demandant à l'utilisateur les paramètres de jeu pour la partie
        public static void InstancierPartie()
        {
            Console.Clear();
            string fichierSource = "dico_fr.txt";
            string fichierCible = "dico_reduit.txt";

            int tailleMot = 0;
            // Demande de la taille du mot
            do
            {
                Console.WriteLine("Entrer une taille de mot à deviner entre 6 et 10");
                tailleMot = int.Parse(Console.ReadLine());
            }
            while (tailleMot < 6 || tailleMot > 10);

            // Création du fichier avec seulement des mots de la taille demandée
            reduireFichier(fichierSource, tailleMot, fichierCible);


            // Nb de tentatives = nb de lignes du tableau
            Console.WriteLine("Entrer le nombre de tentatives autorisées");
            int nbTentatives = int.Parse(Console.ReadLine());

            // Pour parametrer un temps pour proposer un mot
            Console.WriteLine("Souhaitez-vous mettre un temps limité pour proposer un mot? O/N");
            string temp = Console.ReadLine(); // variable temporaire
            temp = temp.ToUpper(); //Normalisation de la chaine

            TimeSpan tempsTour = new TimeSpan();

            // Cas où on veut un chrono pour chaque proposition de mot
            if (temp.Equals("O"))
            {
                int seconde = 0;
                do
                {
                    Console.WriteLine("Entrer un temps compris entre 5 et 60 secondes : ");
                    tempsTour = TimeSpan.FromSeconds(int.Parse(Console.ReadLine()));

                    // Creation d'une variable temporaire seconde pour faire une verification avec un int
                    seconde = tempsTour.Seconds;
                    Console.WriteLine(tempsTour);
                }
                
                while (seconde < 4 && seconde > 61); // verification de la donnée saisie par l'utilisateur
            }
            // Cas où on ne souhaite pas avoir de limite de temps pour proposer un mot
            else
            {
                // .FromDays(1) pour mettre une "limite" de temps a un jour
                tempsTour = TimeSpan.FromDays(1);
            }

            // création de la grille de jeu
            char[,] grille = new char[nbTentatives, tailleMot];

            // Remplit la grille de caractères '.' pour la verification des cases, correspondant à des cases encore "vide" pour le jeu
            for(int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    grille[i, j] = '.';
                }
            }

            // grille pour connaitre la couleur a afficher selon la validité de la lettre
            int [,] grilleCouleur = new int[nbTentatives, tailleMot];

            // Appel de la fonction JouerPartie pour débuter une action
            JouerPartie(grille, tempsTour, fichierCible, grilleCouleur);

        }

        // Fonction permettant de créer le mot utilisé pour la partie, initialise le chrono pour la partie et vérifie si la partie est terminée (gagnée ou perdue)
        public static void JouerPartie(char[,] grille, TimeSpan tempsTour, string fichierCible, int[,] grilleCouleur)
        {
            // Clear de la console pour des raisons esthétiques
            Console.Clear();

            //Générer le mot random du dico
            string motInitial = GenererMotAleatoire(fichierCible);
            Console.WriteLine(motInitial);

            // Ajout de la lettre connue (première lettre) du mot a la grille sur la 1ère ligne
            grille[0, 0] = motInitial[0];
            grilleCouleur[0, 0] = 1;

            //Affichage de la grille pour débuter la partie
            AfficherGrille(grille, grilleCouleur);

            // Variable pour vérifier si la partie est terminée et/ou gagnée
            bool finPartie = false;
            bool partieGagne = false;

            // Start chronometre total d'une partie
            System.Diagnostics.Stopwatch tpsTotal = new System.Diagnostics.Stopwatch();
            tpsTotal.Start();

            // Boucle qui va permettre de jouer un tour tant que la partie n'est pas terminée
            while (!finPartie)
            {
                // Appel à la fonction JouerTour pour savoir si la partie est gagnée ou pas
                partieGagne = JouerTour(grille, tempsTour, grilleCouleur, motInitial);

                // Si la partie est gagnée
                if(partieGagne)
                {
                    finPartie = partieGagne; // Alors la partie est terminée
                }
                // Sinon, on vérifie si le nombre de tentatives autorisées n'est pas dépassées
                else if(!grille[grille.GetLength(0)-1, 2].Equals('.'))
                {
                    finPartie = true; // Si oui, la partie est aussi terminée
                }
                // Affichage de la grille après avoir jouer un tour = proposer un mot
                AfficherGrille(grille, grilleCouleur);
            }

            // Arret du chronomètre total d'une partie
            tpsTotal.Stop();
            TimeSpan ts = tpsTotal.Elapsed;

            // Affichage de fin
            AfficherFinDePartie(partieGagne, motInitial, ts);
            
        }

        // Fonction pour jouer un tour = proposer un mot + le vérifier
        public static bool JouerTour(char[,] grille, TimeSpan tempsTour, int[,] grilleCouleur, string motInitial)
        {
            string motPropose = "";
            int tourActuel = 0;

            // Début du chrono pour proposer un mot
            System.Diagnostics.Stopwatch tpsTour = new System.Diagnostics.Stopwatch();
            tpsTour.Start();

            // Demande du mot en vérifiant qu'il soit correct à ce que l'on attends
            while (motPropose.Length != grille.GetLength(1))
            {
                Console.WriteLine("\nEntrer votre proposition de mot");
                motPropose = Console.ReadLine();
                motPropose = motPropose.ToUpper();
            }

            // Arret du chronomètre pour la propostion du mot
            tpsTour.Stop();
            TimeSpan ts = tpsTour.Elapsed;

            // Cas ou le temps mis pour proposer un mot a dépassé celui parametré
            if(ts.Seconds >= tempsTour.Seconds && tempsTour.Seconds != 0)
            {
                Console.WriteLine("Temps imparti dépassé, votre proposition n'a pas été retenue");
                // reset du motPropose

                motPropose = "";
                // remplace le mot proposé par des etoiles pour simuler un échec
                for (int i = 0; i < grille.GetLength(1); i++)
                {
                    motPropose += "*";
                }
            }

            // Pour savoir a quelle ligne / tentative on est et où il faut ajouter le mot dans la grille
            for (tourActuel = 0; tourActuel < grille.GetLength(0); tourActuel++)
            {
                if (grille[tourActuel, 2].Equals('.'))
                {
                    break;
                }
            }

            // Rajoute le mot dans la grille
            for (int i = 0; i < grille.GetLength(1); i++)
            {
                    grille[tourActuel, i] = motPropose[i];
            }

            // On retourne le résultat (boolean) de la fonction VerifierMot qui determinera si le mot proposer est le bon ou nom
            return VerifierMot(motInitial, motPropose, tourActuel, grilleCouleur);
        }

        // Vérifie si le mot entré correspond au mot initial + remplis la grilleCouleur selon l'exactitude du mot propose
        public static bool VerifierMot(string motInitial, string motPropose, int tourActuel, int [,] grilleCouleur)
        {
            bool gagne = false;

            // Vérifie si le mot est parfaitement égal
            if (motInitial.Equals(motPropose))
            {
                // mettre grille couleur a 1 = toutes les lettres sont bonnes
                for (int i = 0; i < grilleCouleur.GetLength(1); i++)
                {
                    grilleCouleur[tourActuel, i] = 1;
                    
                }
                gagne = true;
            }
            
            else
            {
                // Parcours la grille de couleur
                for (int i = 0; i < grilleCouleur.GetLength(1); i++)
                {
                    // Vérifie si des lettres sont bien placées
                    if (motPropose[i].Equals(motInitial[i]))
                    {
                        grilleCouleur[tourActuel, i] = 1; // remplit à 1 (= lettre bien placée) si la lettre où l'on se situe est bien placee
                    }
                    // Vérifie si une lettre est mal placée, mais présente dans le mot initial 
                    else
                    {
                        int j = i;
                        bool bonneLettre = false;
                        // Parcours du mot propose tant qu'on ne trouve pas de lettre presente dans le mot initial
                        while((!bonneLettre) && (j < grilleCouleur.GetLength(1)))
                        {
                            // lettre présente
                            if (motPropose[i].Equals(motInitial[j]))
                            {
                                grilleCouleur[tourActuel, i] = 2; // remplit la grille couleur à cette position pour indiquer que la lettre est présente mais mal placee
                                bonneLettre = true;
                            }
                            j++;
                        }
                            
                    }
                    // Lettres pas présente
                    if (grilleCouleur[tourActuel, i] == 0)
                    {
                        grilleCouleur[tourActuel, i] = -1; // remplissage de la grille Couleur à cette position pour indiquer que la lettre n'est pas présente
                    }
                }

            }

            return gagne; // Retourne la valeur de la variable gagne qui est mise a "true" dans le cas ou toutes les lettres du mot propose sont bonnes
            
        }

        // Retourne un mot aléatoire du dico reduit (et donc ne contenant que des mots de x lettres choisies par l'utilisateur)
        public static string GenererMotAleatoire(string fichierCible) 
        {
            string mot = "";
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier cible 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                //Premier reader
                StreamReader monStreamReader = new StreamReader(fichierCible, encoding);
                //Deuxieme reader
                StreamReader monStreamReader2 = new StreamReader(fichierCible, encoding);

                mot = monStreamReader.ReadLine();
                

                int nbMots = 0;
                
                // Parcours pour connaitre le nb de mots du fichier
                while (mot != null)
                {
                    mot = monStreamReader.ReadLine();
                    
                    nbMots++;
                }

                // Choix d'un nombre aléatoire entre 1 et le nb de mots du fichiers
                Random rnd = new Random();
                int random = rnd.Next(1, nbMots);
                
                
                nbMots = 0;

                // Parcours du dico pour trouver le mot correspondant a la ligne aléatoire "choisie"
                while (nbMots != random)
                {
                    mot = monStreamReader2.ReadLine();
                    nbMots++;
                }
                
                // Fermeture du StreamReader 
                monStreamReader.Close();
                monStreamReader2.Close();


            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'opération :");
                Console.WriteLine(ex.Message);
            }

            return mot.ToUpper(); // retourne le mot choisie en majuscule (pour l'affichage et la standardisation)
        }

        // Affichage du temps
        public static void AfficherTemps(TimeSpan ts)
        {
            string tempsEcoule = String.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10); // permet de changer le format pour standardiser l'affichage
            Console.WriteLine("Temps de jeu : " + tempsEcoule);
        }

        // Affichage de fin de partie
        public static void AfficherFinDePartie(bool partieGagne, string motInitial, TimeSpan ts)
        {
            // Reset du background pour l'ecriture
            Console.BackgroundColor = ConsoleColor.Black;

            // Cas victorieux
            if (partieGagne)
            {
                Console.WriteLine("\nBravo, vous avez gagné la partie!");
                JouerUnSon(@"E:\IPROG\Music\motus_mot_trouve.wav");
            }
            else // Défaite
            {
                Console.WriteLine("\nTu feras mieux la prochaine fois!");
                Console.WriteLine("Le mot a deviné était : " + motInitial);
            }

            // Affichage du temps de la partie
            AfficherTemps(ts);

            // Sauvegarde de la partie
            Sauvegarder(motInitial, ts, partieGagne);

            // Demande de rejouer
            Console.WriteLine("Souhaitez-vous rejouer? O/N");
            string rejouer = Console.ReadLine();

            if (rejouer.Equals("O"))
            {
                InstancierPartie(); // Appel de InstancerPartie() pour recommencer
            }
            else
            {
                AfficherMenu(); // Retour au menu
            }

        }

        // Affichage de la grille 
        public static void AfficherGrille(char[,] grille, int[,] grilleCouleur)
        {
            // Parcours du contenu du tableau grille
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                Console.Write("\n");
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    int caseSwitch = grilleCouleur[i,j];

                    // Pour colorier la case selon les précedentes (ou pas) propositions de mots
                    switch (caseSwitch)
                    {
                        case 1: // lettre bien placée = rouge
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case 2: // lettre présente mais mal placée = jaune
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case -1: // lettre non présente = bleue
                            Console.BackgroundColor = ConsoleColor.Blue;
                            break;
                        case 0: // cas ou aucune proposition n'a encore ete faite, donc pas de couleur
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                    }
                    
                    Console.Write(grille[i, j] );

                }

            }
          
        }

        // -------------------------------------------------------------------------------- Statistiques -------------------------------------------------------------------------------- //

        // Génération d'un score avec la longueur du mot, le tps de la partie total, le nb de tentatives,                                  ABANDON // TODOOOOOO
        public static void GenerationScore()
        {

        }

        // Création d'un fichier de sauvegarde de la partie --> se situe dans /bin/Debug
        public static void Sauvegarder(string motIni, TimeSpan tempsPartie, bool partieGagne)
        {
            try
            {
                // Demande a l'utilisateur d'entrer un nom de sauvegarde pour lui
                Console.WriteLine("Entrer un nom de Joueur :");
                string nomJoueur = Console.ReadLine();

                                                                                                                                // TODO : verif que le fichier existe
                                                                                                                                // if not, créer le fichier + la légende du début

                // Création d'une instance de StreamWriter pour permettre l'ecriture de notre fichier cible
                StreamWriter monStreamWriter = File.AppendText("sauvegarde.txt");
                
                // On écrit dans le fichier les données de la dernière partie
                monStreamWriter.WriteLine("{0} ; {1} ; {2} ; {3} ", nomJoueur, partieGagne, motIni, tempsPartie.ToString());
                
                // Fermeture du StreamWriter
                monStreamWriter.Close();
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'opération :");
                Console.WriteLine(ex.Message);
            }

        }

         // ----------------------------------------------------------------------------------- Options ----------------------------------------------------------------------------------- //
         public static void JouerUnSon(string chemin)
        {
            // Creation de l'objet SoundPlayer
            System.Media.SoundPlayer s = new System.Media.SoundPlayer();

            // Pour donner la localisation du fichier son 
            s.SoundLocation = chemin;

            // Play looping
            // s.PlayLooping();

            // Play normal
            s.Play();

            // Stop 
            // s.Stop();

        }

        // -------------------------------------------------------------------------------- Quitter le jeu -------------------------------------------------------------------------------- //
        static void QuitterJeu()
        {
            // Verification que l'utilisateur veut vraiment quitter le jeu
            Console.WriteLine("Voulez-vous vraiment quitter le jeu? O/N");
            string temp = Console.ReadLine();

            temp = temp.ToUpper(); // standardisation de la reponse

            if(temp.Equals("O"))
            {
                Environment.Exit(0); // ferme la console
            }
            else
            {
                AfficherMenu(); // retourne au menu
            }
        }


        // -------------------------------------------------------------------------------- Manipulations du dico -------------------------------------------------------------------------------- //
        // Enleve tous les accents d'une chaine de caractere donnee
        static string SupprimerAccents(string text)
        {
            var stringNormaliser = text.Normalize(NormalizationForm.FormD); // standardisation de la chaine
            var stringConstruit = new StringBuilder();

            // Parcours de la chaine
            foreach (var c in stringNormaliser) 
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c); // obtient la categorie du caractere en Unicode
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) // NonSpacingMark = caractere qui indique que des modification ont ete apporte a un caractere de base
                {
                    stringConstruit.Append(c); // Ajoute un nombre specifie de copies de la representation sous forme de chaine d'un caractere Unicode a la variable
                }
            }

            return stringConstruit.ToString().Normalize(NormalizationForm.FormC); // retourne la chaine sans accents
        }


        // Création du dico correspondant au nb de lettres souhaité
        public static void reduireFichier(string fichier_source, int tailleMot, string fichier_cible)
        {
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier source 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichier_source, encoding);

                // Création d'une instance de StreamWriter pour permettre l'ecriture de notre fichier cible
                StreamWriter monStreamWriter = File.CreateText(fichier_cible);
                
                string mot = monStreamReader.ReadLine();

                // Lecture de tous les mots du fichier (un par lignes) 
                while (mot != null)
                {
                    if (mot.Length == tailleMot)              // tous les mots de la taille donnée
                    {
                        mot = SupprimerAccents(mot);        //... on convertit le mot sans accent
                        monStreamWriter.WriteLine(mot);   //... on écrit dans le fichier cible
                    }
                        
                    mot = monStreamReader.ReadLine();

                }
                // Fermeture du StreamReader
                monStreamReader.Close();
                // Fermeture du StreamWriter
                monStreamWriter.Close();
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'opération :");
                Console.WriteLine(ex.Message);
            }
        }



    }
}
