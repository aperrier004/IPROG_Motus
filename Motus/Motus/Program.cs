using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Motus
{
    // Auteurs : Coline BINET et Alban PERRIER
    // Groupe 1 en 1A a l'ENSC
    // Projet de realisation du jeu MOTUS en C#
    class Program
    {
        //---------------------------------------------------------------------------------- Variables globales ----------------------------------------------------------------------------------//
        // Nom de fichiers avec chemin relatif
        public static string fichierDico = @"..\..\..\Dicos\dico_fr.txt";
        public static string fichierDicoReduit = @"..\..\..\Dicos\dico_reduit.txt";
        public static string fichierSauvegarde = @"..\..\sauvegarde.txt";
        // Mode defis, indice aleatoire et random
        public static bool indiceAleatoire = false;
        public static Random rnd = new Random();
        public static bool modeDefi = false;
        public static System.Diagnostics.Stopwatch tempsModeDefi;
        public static int nbPartiesGagneesDefi = 0;
        public static int nbMotsDefi = 0;

        // Main qui affiche un ecran de depart et dirige vers le menu de jeu
        static void Main(string[] args)
        {
            // Configure de la console
            Console.Title = "Motus - BINET et PERRIER";
            Console.SetWindowSize(101, 30);

            // Affichage de debut
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("                                        ~~~ MO-MO-MO-MOTUS ~~~                                        ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("\nBy Coline BINET et Alban PERRIER G1 1A ENSC");

            // Joueur le son du debut en boucle
            JouerUnSonEnBoucle("generique_intro");

            Console.WriteLine("\n\n\nAppuyez sur une touche pour commencer !");
            Console.ReadKey();

            // MENU 
            AfficherMenu();
        }

        // Affichage du menu et permet d'appeler la fonction correspondant a ce que l'utilisateur choisi
        public static void AfficherMenu()
        {
            // Affichage
            Console.Clear();
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("                                               MENU                                                    ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("1: Nouvelle partie \n2: Statistiques \n3: Options \n4: Quitter");

            // Verifications de l'entree clavier + Feedback de ce que rentre l'utilisateur
            bool saisieIsValid = false;
            int caseSwitch = 0;
            while (!saisieIsValid)
            {
                Console.WriteLine("\nSaisir le numero de l'option que vous souhaitez");
                string saisie = Console.ReadLine();
                if (int.TryParse(saisie, out caseSwitch))
                {
                    caseSwitch = int.Parse(saisie);
                    if (caseSwitch > 0 && caseSwitch < 5)
                    {
                        saisieIsValid = true;
                    }
                }
            }

            // Permet d'appeler la fonction permettant de realiser l'action que l'utilisateur souhaite
            switch (caseSwitch)
            {
                case 1: // Nouvelle Partie
                    ParametrerPartie();
                    break;
                case 2: // Statistiques
                    AfficherStats();
                    break;
                case 3: // Options
                    AfficherOptions();
                    break;
                case 4: // Quitter
                    QuitterJeu();
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------- Nouvelle Partie---------------------------------------------------------------------------------- //
        // Initialise la partie en donnant le nom des dicos utilises et demandant a l'utilisateur les paramètres de jeu pour la partie
        public static void ParametrerPartie()
        {
            Console.Clear();
            ArreterUnSon();

            // Affichage des regles
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("                                               REGLES                                                ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("Le jeu Motus repose sur la recherche de mots d'un nombre fixe de lettres.");
            Console.WriteLine("Le mot doit etre present dans le dictionnaire du jeu et ");
            Console.WriteLine("Chaque lettre du mot encore a trouver correspond a un \".\" dans la grille de jeu");

            // Selection de la difficulte
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("                                               DIFFICULTE                                            ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("1: Facile \n2: Moyen \n3: Difficile \n4: Personnalise \n5: Mode Defi");
            Console.WriteLine("\nFacile : Mot de 6 ou 7 lettres avec un nombre de tentatives egale a la taille du mot");
            Console.WriteLine("Moyen : Mot entre 7 et 9 lettres avec un nombre de tentatives egale a la taille du mot moins 2");
            Console.WriteLine("Difficile : Mot entre 8 et 10 lettres avec un nombre de tentatives egale a la taille du mot moins 3");
            Console.WriteLine("Personnalise : Cela vous permet de choisir vous meme les parametres du jeu");
            Console.WriteLine("Mode Defi : Trouver un maximum de mots dans le temps imparti : 5 minutes");

            // Verification de l'entrees clavier + Feedback de ce que rentre l'utilisateur
            bool saisieIsValid = false;
            int difficulte = 0;
            while (!saisieIsValid)
            {
                Console.WriteLine("\nSaisir la difficulte que vous souhaitez");
                string saisie = Console.ReadLine();
                if (int.TryParse(saisie, out difficulte))
                {
                    difficulte = int.Parse(saisie);
                    if (difficulte > 0 && difficulte < 6)
                    {
                        saisieIsValid = true;
                    }                    
                }
            }

            // Variables de difficultes a parametrer
            int tailleMot = 0;
            int nbTentatives = 0;
            TimeSpan dureeTour = new TimeSpan();
            dureeTour = TimeSpan.FromDays(1);

            // pour le mode defi
            TimeSpan dureeMode = new TimeSpan();

            char[,] grille;
            int[,] grilleCouleur;

            // Permet d'appeler fonction permettant de selectionner la difficulte que l'utilisateur souhaite
            switch (difficulte)
            {
                case 1: // Facile
                    tailleMot = rnd.Next(6, 7);
                    nbTentatives = tailleMot;

                    break;
                case 2: // Moyen
                    tailleMot = rnd.Next(7, 9);
                    nbTentatives = tailleMot - 2;

                    break;
                case 3: // Difficile
                    tailleMot = rnd.Next(8, 10);
                    nbTentatives = tailleMot - 3;

                    break;
                case 4: // Personnalise
                    PersonnaliserDifficulte(ref tailleMot, ref nbTentatives, ref dureeTour);
                    break;

                case 5: // Mode defi
                    nbTentatives = rnd.Next(3, 10);
                    tailleMot = rnd.Next(6, 10);
                    dureeMode = TimeSpan.FromMinutes(5);
                    break;
            }

            //on initialise la partie
            InitialiserPartie(tailleMot, nbTentatives, out grille, out grilleCouleur);

            if(difficulte != 5)
            {
                // Appel de la fonction JouerPartie pour debuter une action
                JouerPartie(grille, dureeTour, grilleCouleur, difficulte);
            }
            else // mode defi
            {
                ModeDefi(grille, dureeMode, grilleCouleur, difficulte);
            }
        }
        
        //fonction initialisation de la partie
        public static void InitialiserPartie(int tailleMot, int nbTentatives, out char[,] grille, out int[,] grilleCouleur)
        {
            // Creation du fichier avec seulement des mots de la taille demandee
            reduireFichier(tailleMot);

            // creation de la grille de jeu
            grille = new char[nbTentatives, tailleMot];

            // Remplit la grille de caractères '.' pour la verification des cases, correspondant a des cases encore "vides" pour le jeu
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    grille[i, j] = '.';
                }
            }

            // grille pour connaitre la couleur a afficher selon la validite de la lettre
            grilleCouleur = new int[nbTentatives, tailleMot];
        }

        // Fonction qui permet a l'utilisateur de parametrer ses parametres de jeu
        public static void PersonnaliserDifficulte(ref int tailleMot, ref int nbTentatives, ref TimeSpan tempsTour)
        {
            // Demande de la taille du mot + Verification
            bool saisieIsValid = false;
            while (!saisieIsValid)
            {
                Console.WriteLine("\nEntrer une taille de mot a deviner entre 6 et 10");
                string saisie = Console.ReadLine();
                if (int.TryParse(saisie, out tailleMot))
                {
                    tailleMot = int.Parse(saisie);
                    if(tailleMot > 5 && tailleMot < 11)
                    {
                        saisieIsValid = true;
                    }
                }
            }

            // Nb de tentatives = nb de lignes du tableau
            saisieIsValid = false;
            while (!saisieIsValid)
            {
                Console.WriteLine("\nEntrer le nombre de tentatives autorisees");
                string saisie = Console.ReadLine();
                if (int.TryParse(saisie, out nbTentatives))
                {
                    nbTentatives = int.Parse(saisie);
                    if (nbTentatives > 0)
                    {
                        saisieIsValid = true;
                    }
                }
            }

            // Pour parametrer un temps pour proposer un mot
            tempsTour = new TimeSpan();

            Console.WriteLine("\nSouhaitez-vous mettre un temps limite pour proposer un mot? O/N");
            ConsoleKeyInfo saisiekey = Console.ReadKey(true);
            if (saisiekey.Key == ConsoleKey.O)
            {
                int seconde = 0;
                saisieIsValid = false;
                while (!saisieIsValid)
                {
                    Console.WriteLine("\nEntrer un temps compris entre 5 et 60 secondes : ");
                    string saisie = Console.ReadLine();
                    if (int.TryParse(saisie, out seconde))
                    {
                        seconde = int.Parse(saisie);
                        if(seconde > 4 && seconde < 61)
                        {
                            saisieIsValid = true;
                            tempsTour = TimeSpan.FromSeconds(seconde);
                        }                        
                    }
                }
            }
            else
            {
                // .FromDays(1) pour mettre une "limite" de temps a un jour
                tempsTour = TimeSpan.FromDays(1);
            }
        }

        // --------------------------------------------------- Mode defi --------------------------------------------------- //
        // fonction pour lancer le mode Defi
        public static void ModeDefi(char[,] grille, TimeSpan dureeMode, int[,] grilleCouleur, int difficulte)
        {
            modeDefi = true;
            // on lance le chrono pour le debut du mode defi
            tempsModeDefi = new System.Diagnostics.Stopwatch();
            tempsModeDefi.Start();

            //debut de la partie
            JouerPartie(grille, dureeMode, grilleCouleur, difficulte);
        }

        //fonction pour initialiser une nouvelle partie ( = recherche de mot ) du mode defi + compteur mots
        public static void CreerPartieDefi()
        {
            // On met en pause le chrono du mode
            tempsModeDefi.Stop();

            // On reinitialise tous les parametres de jeu comme si on venait de commencer
            char[,] grille;
            int[,] grilleCouleur;

            int nbTentatives = rnd.Next(3, 10);
            int tailleMot = rnd.Next(6, 10);

            // On commence la partie
            InitialiserPartie(tailleMot, nbTentatives, out grille, out grilleCouleur);
           
            TimeSpan tempsTour = TimeSpan.FromDays(1); // tps infini
            TimeSpan tempsModeDefiSpan = tempsModeDefi.Elapsed;
            string tempsEcoule = FormaterTemps(tempsModeDefiSpan);

            Console.WriteLine("\nIl s'est ecoule {0} sur 5 minutes. \nVous avez trouve {1} mot(s) sur {2}.", tempsEcoule, nbPartiesGagneesDefi, nbMotsDefi);

            Console.WriteLine("\nAppuyez sur une touche pour continuer le mode et trouver un nouveau mot");
            Console.ReadKey();

            // On relance le chrono
            tempsModeDefi.Start();

            JouerPartie(grille, tempsTour, grilleCouleur, 5);

            
        }

        // --------------------------------------------------- Deroulement de la partie --------------------------------------------------- //
        // Fonction permettant de creer le mot utilise pour la partie, initialise le chrono pour la partie et verifie si la partie est terminee (gagnee ou perdue)
        public static void JouerPartie(char[,] grille, TimeSpan dureeTour, int[,] grilleCouleur, int difficulte)
        {
            // Clear de la console pour des raisons esthetiques
            Console.Clear();

            //Genère le mot random du dico reduit
            string motInitial = GenererMotAleatoire();

            // on verifie si l'indice est aleatoire ou non
            if(indiceAleatoire)
            {
                //on determine la position de l'indice aleatoirement
                int positionIndice = rnd.Next(0, grille.GetLength(0));
                grille[0, positionIndice] = motInitial[positionIndice];
                // grilleCouleur[0, positionIndice] = 1;

            }
            else
            {
                // Ajout de la lettre connue (première lettre) du mot a la grille sur la 1ère ligne
                grille[0, 0] = motInitial[0];
                // grilleCouleur[0, 0] = 1;
            }

            //Affichage de la grille pour debuter la partie
            AfficherGrille(grille, grilleCouleur);

            // Variable pour verifier si la partie est terminee et/ou gagnee
            bool finPartie = false;
            bool partieGagne = false;

            // Start chronometre total d'une partie
            System.Diagnostics.Stopwatch tpsTotal = new System.Diagnostics.Stopwatch();
            tpsTotal.Start();

            // Boucle qui va permettre de jouer un tour tant que la partie n'est pas terminee
            while (!finPartie)
            {
                // Appel a la fonction JouerTour pour savoir si la partie est gagnee ou pas
                partieGagne = JouerTour(grille, dureeTour, grilleCouleur, motInitial);

                // Si la partie est gagnee
                if(partieGagne)
                {
                    finPartie = partieGagne; // Alors la partie est terminee
                }
                // Sinon, on verifie si le nombre de tentatives autorisees n'est pas depassee
                else if(!grille[grille.GetLength(0)-1, 2].Equals('.'))
                {
                    finPartie = true; // Si oui, la partie est aussi terminee
                }
                // Affichage de la grille après avoir joue un tour = proposer un mot
                AfficherGrille(grille, grilleCouleur);
            }

            TimeSpan ts = new TimeSpan();

            if (!modeDefi) // on stoppe le chrono que si on est pas en mode defi
            {
                // Arret du chronomètre total d'une partie
                tpsTotal.Stop();
            }
            ts = tpsTotal.Elapsed;
            // Affichage de fin
            AfficherFinDePartie(partieGagne, motInitial, ts, difficulte);
            
        }

        // Fonction pour jouer un tour = proposer un mot + le verifier
        public static bool JouerTour(char[,] grille, TimeSpan dureeTour, int[,] grilleCouleur, string motInitial)
        {
            string motPropose = "";
            int tourActuel = 0;

            // Debut du chrono pour proposer un mot
            System.Diagnostics.Stopwatch tpsTour;
            if (modeDefi)
            {
                // on donne la valeur ecoulee depuis le debut du mode defi a la tpsTour
                tpsTour = tempsModeDefi;
            }
            else
            {
                tpsTour = new System.Diagnostics.Stopwatch();
            }
            tpsTour.Start();

            // Demande du mot en verifiant qu'il soit correct ( longueur demandee + mot existant dans le dico reduit )
            bool motExistant = false;
            bool longueur = false;
            
            // Verification de l'entree clavier + Feedback de ce qu'entre l'utilisateur comme mot
            while (!longueur || !motExistant)
            {
                Console.WriteLine("\nEntrer votre proposition de mot");
                motPropose = Console.ReadLine();
                motPropose = motPropose.ToUpper(); // Normalisation de la chaine
            
                // Si le mot fait la bonne longueur
                if (motPropose.Length == grille.GetLength(1)) 
                {
                    longueur = true;
                    //alors on verifie qu'il existe dans le dico reduit
                    motExistant = VerifierMotDansDico(motPropose);

                    // Si le mot n'existe pas, on previent qu'il y a une erreur
                    if (!motExistant)
                    {
                        Console.WriteLine("Le mot n'existe pas dans le dictionnaire français.");
                    }
                }
                // Mauvaise longueur de mot
                else if(!longueur)
                {
                    Console.WriteLine("Le mot ne fait pas la bonne taille");
                }
            }
            
            // Arret du chronomètre pour la proposition du mot
            tpsTour.Stop();
            TimeSpan ts = tpsTour.Elapsed;

            // Cas ou le temps mis pour proposer un mot a depasse celui parametre
            if(ts.Seconds >= dureeTour.Seconds && dureeTour.Seconds != 0)
            {
                Console.WriteLine("Temps imparti depasse, votre proposition n'a pas ete retenue");
                // reset du motPropose

                motPropose = "";
                // remplace le mot propose par des etoiles pour simuler un echec
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
                    break; // Permet d'arreter directement la boucle 
                }
            }

            // Rajoute le mot dans la grille
            for (int i = 0; i < grille.GetLength(1); i++)
            {
                    grille[tourActuel, i] = motPropose[i];
            }
            
            // On retourne le resultat (boolean) de la fonction VerifierMot qui determinera si le mot propose est le bon ou non
            return VerifierMot(motInitial, motPropose, tourActuel, grilleCouleur);
        }

        // Affichage de la grille + legende des couleurs
        public static void AfficherGrille(char[,] grille, int[,] grilleCouleur)
        {
            // Affichage de la legende de la grille
            Console.WriteLine("\n---------------------------------------------------");
            Console.WriteLine("                      LEGENDE                      ");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("ROUGE : Lettre bien placee");
            Console.WriteLine("JAUNE : Lettre presente dans le mot mais mal placee");
            Console.WriteLine("BLEU : Lettre non presente dans le mot");

            // Parcours du contenu du tableau grille
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                Console.Write("\n");
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    int caseSwitch = grilleCouleur[i, j];

                    // Pour colorier la case selon les precedentes (ou pas) propositions de mots
                    switch (caseSwitch)
                    {
                        case 1: // lettre bien placee = rouge
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case 2: // lettre presente mais mal placee = jaune
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case -1: // lettre non presente = bleue
                            Console.BackgroundColor = ConsoleColor.Blue;
                            break;
                        case 0: // cas ou aucune proposition n'a encore ete faite, donc pas de couleur
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                    }

                    Console.Write(grille[i, j]);
                }
                Console.Write("\n");
            }
        }

        // --------------------------------------------------- Gestion du mot avec le dictionnaire --------------------------------------------------- //
        // Verifie si le mot entre correspond au mot initial + remplit la grilleCouleur selon l'exactitude du mot propose
        public static bool VerifierMot(string motInitial, string motPropose, int tourActuel, int [,] grilleCouleur)
        {
            bool gagne = false;
            
            // Verifie si le mot est parfaitement egal
            if (motInitial.Equals(motPropose))
            {
                JouerUnSon("motus_mot_trouve");

                // mettre grille couleur a 1 = toutes les lettres sont bonnes
                for (int i = 0; i < grilleCouleur.GetLength(1); i++)
                {
                    grilleCouleur[tourActuel, i] = 1;
                }
                gagne = true;
            }
            
            else
            {
                // Parcourt la grille de couleurs
                for (int i = 0; i < grilleCouleur.GetLength(1); i++)
                {
                    // Verifie si des lettres sont bien placees
                    if (motPropose[i].Equals(motInitial[i]))
                    {
                        JouerUnSon("rouge");
                        Thread.Sleep(350);

                        grilleCouleur[tourActuel, i] = 1; // remplit a 1 (= lettre bien placee) si la lettre où l'on se situe est bien placee
                    }
                    // Verifie si une lettre est mal placee, mais presente dans le mot initial 
                    else
                    {
                        int j = i;
                        bool bonneLettre = false;

                        // Parcours du mot propose tant qu'on ne trouve pas de lettre presente dans le mot initial
                        while((!bonneLettre) && (j < grilleCouleur.GetLength(1)))
                        {
                            // lettre presente
                            if (motPropose[i].Equals(motInitial[j]))
                            {
                                JouerUnSon("jaune");
                                Thread.Sleep(350);

                                grilleCouleur[tourActuel, i] = 2; // remplit la grille couleurs a cette position pour indiquer que la lettre est presente mais mal placee
                                bonneLettre = true;
                            }
                            j++;
                        }
                    }
                    // Lettres pas presentes
                    if (grilleCouleur[tourActuel, i] == 0)
                    {
                        JouerUnSon("bleu");
                        Thread.Sleep(350);

                        grilleCouleur[tourActuel, i] = -1; // remplissage de la grille Couleur a cette position pour indiquer que la lettre n'est pas presente
                    }
                }
            }
            return gagne; // Retourne la valeur de la variable gagne qui est mise a "true" dans le cas ou toutes les lettres du mot propose sont bien placees
            
        }

        // Verifie si le mot entre existe dans le Dictionnaire
        public static bool VerifierMotDansDico(string motPropose)
        {
            bool motExistant = false;

            try
            {
                // Creation d'une instance de StreamReader pour permettre la lecture de notre fichier  
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichierDicoReduit, encoding);
                
                string mot = monStreamReader.ReadLine();

                // Lecture de tous les mots du fichier
                while (mot != null)
                {
                    // Si le mot de la ligne a laquelle on se situe est le meme que celui propose
                    if (mot.Equals(motPropose.ToUpper()))            
                    {
                        // Fermeture du StreamReader et renvoi "true" pour signifier que le mot existe bel et bien
                        monStreamReader.Close();
                        return true;
                    }
                    mot = monStreamReader.ReadLine(); // on passe au mot suivant
                }

                // Fermeture du StreamReader
                monStreamReader.Close();
            }
            catch (Exception ex)
            {
                // Code execute en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'operation :");
                Console.WriteLine(ex.Message);
            }

            //return false : le mot n'a pas ete trouve dans le dictionnaire
            return motExistant;
        }

        // Retourne un mot aleatoire du dico reduit (et donc ne contenant que des mots de x lettres choisies par l'utilisateur)
        public static string GenererMotAleatoire() 
        {
            string mot = "";
            try
            {
                // Creation d'une instance de StreamReader pour permettre la lecture de notre fichier cible 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichierDicoReduit, encoding);
                mot = monStreamReader.ReadLine();
                

                int nbMots = 0;
                
                // Parcourt le fichier pour connaitre le nb de mots du fichier
                while (mot != null)
                {
                    mot = monStreamReader.ReadLine();
                    
                    nbMots++;
                }

                //close puis ouvre le sreamreader pour revenir en debut de fichier
                monStreamReader.Close();

                monStreamReader = new StreamReader(fichierDicoReduit, encoding);
                // Choix d'un nombre aleatoire entre 1 et le nb de mots du fichiers
                int random = rnd.Next(1, nbMots);
                
                
                nbMots = 0;

                // Parcours du dico pour trouver le mot correspondant a la ligne aleatoire "choisie"
                while (nbMots != random)
                {
                    mot = monStreamReader.ReadLine();
                    nbMots++;
                }
                
                // Fermeture du StreamReader 
                monStreamReader.Close();


            }
            catch (Exception ex)
            {
                // Code execute en cas d'exception 
                Console.Write("Une erreur est survenue au cours de l'operation :");
                Console.WriteLine(ex.Message);
            }

            return mot.ToUpper(); // retourne le mot choisi en majuscules (pour l'affichage et la standardisation)
        }

        // --------------------------------------------------- Fin de la partie --------------------------------------------------- //
        // Affichage de fin de partie
        public static void AfficherFinDePartie(bool partieGagne, string motInitial, TimeSpan ts, int difficulte)
        {
            // Reset du background pour l'ecriture
            Console.BackgroundColor = ConsoleColor.Black;

            // Cas victorieux
            if (partieGagne)
            {
                Console.WriteLine("\nBravo, vous avez gagne la partie!");

                JouerUnSon("motus_mot_trouve");
            }
            else // Defaite
            {
                Console.WriteLine("\nTu feras mieux la prochaine fois!");
                Console.WriteLine("Le mot a deviner etait : " + motInitial);

                JouerUnSon("aie_coup_dur");
            }

            // Affichage du temps de la partie
            string tempsEcoule = FormaterTemps(ts);
            Console.WriteLine("\nTemps de jeu : " + tempsEcoule);

            // Sauvegarde de la partie
            Sauvegarder(motInitial, ts, partieGagne, difficulte);

            if(modeDefi)
            {
                //si on est en mode defi, on incremente le score + le nb de mots et on relance automatiquement
                if(partieGagne)
                {
                    nbPartiesGagneesDefi++; // nb de mots que l'utilisateur a trouve durant le mode
                }
                nbMotsDefi++; // nb de mots a trouver dans le mode

                TimeSpan tpsDefi = tempsModeDefi.Elapsed;
                // On verifie si on a depasse le temps de jeu du mode, 300 sec = 5min
                if (tpsDefi.TotalSeconds <= 15)
                {
                    CreerPartieDefi(); // On relance une partie pour enchainer le prochain mot
                }
                else // On termine le mode de jeu
                {
                    TimeSpan tempsModeDefiSpan = tempsModeDefi.Elapsed;
                    Console.WriteLine("\nBravo, vous avez termine le mode defi!");
                    Console.WriteLine("\nVous avez trouve {0} mot(s) sur {1}.", nbPartiesGagneesDefi, nbMotsDefi);
                    
                    Rejouer();
                }
            }
            else
            {
                Rejouer();
            }
        }

        // Fonction d'affichage pour rejouer une partie
        public static void Rejouer()
        {
            // Demande de rejouer
            Console.WriteLine("\nSouhaitez-vous rejouer? O/N");
            ConsoleKeyInfo saisie = Console.ReadKey(true);
            if (saisie.Key == ConsoleKey.O)
            {
                ParametrerPartie(); // Appel de InstancerPartie() pour recommencer
            }
            else
            {
                AfficherMenu(); // Retour au menu
            }
        }

        // ---------------------------------------------------------------------------------------------- Statistiques -------------------------------------------------------------------------------- //

        // Creation d'un fichier de sauvegarde de la partie --> se situe dans /bin/Debug
        public static void Sauvegarder(string motIni, TimeSpan tempsPartie, bool partieGagne, int difficulte)
        {
            try
            {
                // Demande a l'utilisateur d'entrer un nom de sauvegarde pour lui associer les statistiques de la partie
                string nomJoueur = "";
                do
                {
                    Console.WriteLine("\nEntrer un nom de Joueur (12 caracteres max.):");
                    nomJoueur = Console.ReadLine();
                }
                while (nomJoueur.Length < 2 || nomJoueur.Length > 12); // Feedback tant que le nom n'est pas de la longueur souhaitee
                
                
                // Si le fichier n'existe pas deja
                if (!File.Exists(fichierSauvegarde))
                {
                    // Creation d'une instance de StreamWriter pour creer le fichier
                    StreamWriter monStreamWriter1 = File.AppendText(fichierSauvegarde);
                    
                    // Creation d'une première ligne pour l'intitule des donnees
                    monStreamWriter1.WriteLine("{0,-20}{1,-20}{2,-20}{3,-20}{4,-20} \n", "Nom Joueur",  "Partie Gagne",  "Mot Initial",  "Temps",  "Difficulte");

                    // Fermeture
                    monStreamWriter1.Close();
                }

                // Creation d'une instance de StreamWriter pour permettre l'ecriture de notre fichier cible
                StreamWriter monStreamWriter = File.AppendText(fichierSauvegarde);

                // On ecrit dans le fichier les donnees de la dernière partie
                string tempsTexte = FormaterTemps(tempsPartie);


                monStreamWriter.WriteLine("{0,-20}{1,-20}{2,-20}{3,-20}{4,-20}", nomJoueur, partieGagne, motIni, tempsTexte, difficulte);


                Console.WriteLine("\nPartie sauvergardee !"); // Feedback
                Thread.Sleep(700);

                // Fermeture du StreamWriter
                monStreamWriter.Close();
            }
            catch (Exception ex)
            {
                // Code execute en cas d'exception 
                Console.Write("\nUne erreur est survenue au cours de l'operation :");
                Console.WriteLine(ex.Message);
            }

        }

        // Fonction qui affiche les statistiques
        public static void AfficherStats()
        {
            try
            {
                Console.Clear();
                ArreterUnSon();

                Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
                Console.WriteLine("                                               STATISTIQUES                                             ");
                Console.WriteLine("-----------------------------------------------------------------------------------------------------");

                Console.WriteLine("<--  Appuyez sur une touche pour revenir au menu \n\n\n");
                
                // Cas ou le fichier n'existe pas
                if (!File.Exists(fichierSauvegarde))
                {
                    Console.WriteLine("\nAucune partie n'a encore ete jouee");
                }
                else
                {
                    // Creation d'une instance de StreamReader pour permettre la lecture
                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                    StreamReader monStreamReader = new StreamReader(fichierSauvegarde, encoding);

                    string ligne = monStreamReader.ReadLine(); // ligne = premiere ligne du fichier

                    // Lecture de toutes les lignes du fichier
                    while (ligne != null)
                    {
                        Console.WriteLine(ligne);
                        ligne = monStreamReader.ReadLine();
                    }

                    // Fermeture du StreamReader
                    monStreamReader.Close();
                }               
            }
            catch (Exception ex)
            {
                // Code execute en cas d'exception 
                Console.Write("\nUne erreur est survenue au cours de l'operation :");
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
            AfficherMenu(); // Retour au menu des qu'une touche a ete appuye
        }

        // ------------------------------------------------------------------------------------------------- Options ----------------------------------------------------------------------------------- //
        
        // Fonction qui affiche les options
        public static void AfficherOptions()
        {
            // Affichage
            Console.Clear();
            ArreterUnSon();
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("                                               OPTIONS                                               ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("\n1: Reinitialiser les donnees du jeu \n2: Modifier la position de l'indice \n3: Changer de dictionnaire \n\n4: <-- Retour au menu");


            // Verification + Feedback de ce que l'utilisateur rentre
            int caseSwitch=0;
            while (caseSwitch !=4)
            {
                bool saisieIsValid = false;
                while (!saisieIsValid)
                {
                    Console.WriteLine("\nSaisir le numero de l'option que vous souhaitez");
                    string saisie = Console.ReadLine();
                    if (int.TryParse(saisie, out caseSwitch))
                    {
                        caseSwitch = int.Parse(saisie);
                        if (caseSwitch > 0 && caseSwitch < 5)
                        {
                            saisieIsValid = true;
                        }
                    }
                }

                // Permet d'appeler la fonction permettant de realiser l'action que l'utilisateur souhaite
                switch (caseSwitch)
                {
                    case 1: // Reset sauvegarde
                        ResetFichier(fichierSauvegarde);
                        break;
                    case 2: // modifier indice 
                        ModifierIndice();
                        break;
                    case 3: // Change de dico    
                        ModifierDico();
                        break;
                    case 4: // Retour au menu
                        AfficherMenu();
                        break;
                }
            }
            
        }

        // Fonction qui permet de reinitialiser les donnees sauvegardees
        public static void ResetFichier(string fichier)
        {
            // On verifie si le fichier existe ou pas
            if (File.Exists(fichier))
            {
                File.Delete(fichier); // On supprime le fichier
                Console.WriteLine(fichier + " a ete supprime");

            }
            else
            {
                Console.WriteLine(fichier + " n'existe pas, il ne peut pas etre supprime");
            }

        }

        // Fonction pour modifier la position de l'indice
        public static void ModifierIndice()
        {
            if(indiceAleatoire) // si l'indice est aleatoire
            {
                Console.WriteLine("\nPour vous aider, une lettre est revelee au hasard dans le mot. \nSouhaitez vous que l'indice soit la première lettre ? O/N");
                ConsoleKeyInfo saisie = Console.ReadKey(true);
                if (saisie.Key == ConsoleKey.O)
                {
                    indiceAleatoire = false; // l'indice sera la premiere lettre
                    Console.WriteLine("\nLa modification a ete prise en compte.");
                }
                else
                {
                    indiceAleatoire = true; // l'indice reste aleatoire
                    Console.WriteLine("\nL'indice reste alatoire.");
                }
            }
            else // Si l'indice est la premiere lettre
            {
                Console.WriteLine("\nPour vous aider, la première lettre est revelee. \nSouhaitez vous que l'indice soit aleatoire ? O/N");
                ConsoleKeyInfo saisie = Console.ReadKey(true);
                if (saisie.Key == ConsoleKey.O)
                {
                    indiceAleatoire = true; // l'indice sera  aleatoire
                    Console.WriteLine("\nLa modification a ete prise en compte.");
                }
                else
                {
                    indiceAleatoire = false; // l'indice reste la premiere lettre
                    Console.WriteLine("\nL'indice reste la premiere lettre.");
                }
            }
        }

        // Fonction qui modifie le dictionnaire avec lequel on joue
        public static void ModifierDico()
        {
            string nomDico = "";

            // Verification + Feedback de ce que l'utilisateur rentre
            int caseSwitch = 0;

            bool saisieIsValid = false;
            while (!saisieIsValid)
            {
                Console.WriteLine("\nSaisir le numero associe au dictionnaire avec lequel vous souhaitez jouer");
                Console.WriteLine("\n1: Animaux \n2: Couleurs \n3: Metiers \n4: Noel \n5: Par defaut\n");
                string saisie = Console.ReadLine();
                if (int.TryParse(saisie, out caseSwitch))
                {
                    caseSwitch = int.Parse(saisie);
                    if (caseSwitch > 0 && caseSwitch < 6)
                    {
                        saisieIsValid = true;
                    }
                }
            }
            // Permet d'appeler la fonction permettant de realiser l'action que l'utilisateur souhaite
            switch (caseSwitch)
            {
                case 1: // Animaux
                    nomDico = "dico_animaux";
                    break;
                case 2: // Couleurs
                    nomDico = "dico_couleurs";
                    break;
                case 3: // Metiers
                    nomDico = "dico_metiers";
                    break;
                case 4: // Noel
                    nomDico = "dico_noel";
                    break;
                case 5: // Dico par defaut fr
                    nomDico = "dico_fr";
                    break;
            }

            // Creation du chemin relatif pour trouver les dictionnaires correspondants
            string chemin = @"..\..\..\Dicos\" + nomDico + ".txt";
            // On assigne le nouveau chemin de dico a la variable globable fichierDico
            fichierDico = chemin;
            Console.WriteLine("Vous avez choisi de jouer avec le " + nomDico);
        }
        
        // -------------------------------------------------------------------------------------------- Quitter le jeu -------------------------------------------------------------------------------- //
        // Fonction qui permet de quitter le Jeu (= fermer la console)
        static void QuitterJeu()
        {
            // Affichage
            Console.Clear();
            ArreterUnSon();
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("                                               QUITTER                                               ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");

            // Verification que l'utilisateur veut vraiment quitter le jeu
            Console.WriteLine("\nVoulez-vous vraiment quitter le jeu? O/N");
            ConsoleKeyInfo saisie = Console.ReadKey(true);
            if (saisie.Key == ConsoleKey.O)
            {
                Environment.Exit(0); // ferme la console
            }
            else
            {
                AfficherMenu(); // retourne au menu
            }
        }

        // ------------------------------------------------------------------------------------------ Manipulations du dico -------------------------------------------------------------------------------- //
        // Enleve tous les accents d'une chaine de caractere donnee
        static string SupprimerAccents(string text)
        {
            var stringNormaliser = text.Normalize(NormalizationForm.FormD); // standardisation de la chaine
            var stringConstruit = new StringBuilder();

            // Parcours de la chaine
            foreach (var c in stringNormaliser) 
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c); // obtient la categorie du caractere en Unicode
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) // NonSpacingMark = caractere qui indique que des modification ont ete apportees a un caractere de base
                {
                    stringConstruit.Append(c); // Ajoute un nombre specifie de copies de la representation sous forme de chaine d'un caractere Unicode a la variable
                }
            }
            return stringConstruit.ToString().Normalize(NormalizationForm.FormC); // retourne la chaine sans accents
        }
        
        // Creation du dico correspondant au nb de lettres souhaite
        public static void reduireFichier(int tailleMot)
        {
            try
            {
                // Creation d'une instance de StreamReader pour permettre la lecture de notre fichier source 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichierDico, encoding);

                // Si le fichier n'existe pas deja
                if (!File.Exists(fichierDicoReduit))
                {
                    // Creation d'une instance de StreamWriter pour creer le fichier
                    StreamWriter monStreamWriter1 = File.CreateText(fichierDicoReduit);
                    // Fermeture
                    monStreamWriter1.Close();
                }

                // Creation d'une instance de StreamWriter pour permettre l'ecriture de notre fichier cible
                StreamWriter monStreamWriter = File.CreateText(fichierDicoReduit);
                
                string mot = monStreamReader.ReadLine();

                // Lecture de tous les mots du fichier (un par ligne) 
                while (mot != null)
                {
                    if (mot.Length == tailleMot)              // tous les mots de la taille donnee
                    {
                        mot = SupprimerAccents(mot);        //... on convertit le mot sans accent
                        mot = mot.ToUpper(); // normalisation du dico
                        monStreamWriter.WriteLine(mot);   //... on ecrit dans le fichier cible
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
                // Code execute en cas d'exception 
                Console.Write("\nUne erreur est survenue au cours de l'operation :");
                Console.WriteLine(ex.Message);
            }
        }

        // ------------------------------------------------------------------------------------------ Normalisation ------------------------------------------------------------------------------------------ //
        // Formatage en String des variables de type TimeSpan
        public static string FormaterTemps(TimeSpan ts)
        {
            string tempsEcoule = String.Format("{0:00}min et {1:00}s", ts.Minutes, ts.Seconds); // permet de changer le format pour standardiser l'affichage en ne prenant en compte que minutes et secondes
            return tempsEcoule;
        }

        // ------------------------------------------------------------------------------------------ Gestion du son ------------------------------------------------------------------------------------------ //
        // Permet de jouer un son avec seulement le nom
        public static void JouerUnSon(string nomSon)
        {
            nomSon = @"..\..\..\Music\" + nomSon + ".wav"; // Permet de rajouter le chemin relatif et l'extension du fichier pour le trouver

            // Verification que le fichier existe bien
            if (File.Exists(nomSon))
            {
                // Creation de l'objet SoundPlayer
                System.Media.SoundPlayer s = new System.Media.SoundPlayer();

                // Pour donner la localisation du fichier son 
                s.SoundLocation = nomSon;
                // Joue le son
                s.Play();
            }
        }

        // Permet de joueur un son en boucle
        public static void JouerUnSonEnBoucle(string nomSon)
        {
            nomSon = @"..\..\..\Music\" + nomSon + ".wav"; // Permet de rajouter le chemin relatif et l'extension du fichier pour le trouver
            // Verification que le fichier existe bien
            if (File.Exists(nomSon))
            {
                // Creation de l'objet SoundPlayer
                System.Media.SoundPlayer s = new System.Media.SoundPlayer();

                // Pour donner la localisation du fichier son 
                s.SoundLocation = nomSon;

                // Joue le son en boucle
                s.PlayLooping();
            }
        }

        // Permet d'arreter le son qui est en cours
        public static void ArreterUnSon()
        {
            // Creation de l'objet SoundPlayer
            System.Media.SoundPlayer s = new System.Media.SoundPlayer();

            s.Stop();
        }

    }
}
