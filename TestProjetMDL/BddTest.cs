using BaseDeDonnees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace TestProjetMDL
{
    
    
    /// <summary>
    ///Classe de test pour BddTest, destinée à contenir tous
    ///les tests unitaires BddTest
    ///</summary>
    [TestClass()]
    public class BddTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        // 
        //Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilisez ClassCleanup pour exécuter du code après que tous les tests ont été exécutés dans une classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Test pour HeureVac
        ///</summary>
        [TestMethod()]
        public void HeureVacTest()
        {

            Bdd target = new Bdd("employemdl", "employemdl"); // TODO: initialisez à une valeur appropriée
            short pNumVac = 2; // TODO: initialisez à une valeur appropriée
            string pfctheure = "heurefinvacation"; // TODO: initialisez à une valeur appropriée
            DateTime expected = new DateTime(); // TODO: initialisez à une valeur appropriée
            DateTime actual;
            actual = target.HeureVac(pNumVac, pfctheure);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour creerVacation
        ///</summary>
        [TestMethod()]
        public void creerVacationTest()
        {

            Bdd target = new Bdd("employemdl", "employemdl"); // TODO: initialisez à une valeur appropriée
            int pIdAtelier = 2; // TODO: initialisez à une valeur appropriée
            string pHeureDebut = "15/09/2013 12:00:00"; // TODO: initialisez à une valeur appropriée
            string pHeureFin = "15/09/2013 12:00:00"; // TODO: initialisez à une valeur appropriée
            target.creerVacation(pIdAtelier, pHeureDebut, pHeureFin);
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }

        /// <summary>
        ///Test pour creerTheme
        ///</summary>
        [TestMethod()]
        public void creerThemeTest()
        {

            Bdd target = new Bdd("employemdl", "employemdl"); // TODO: initialisez à une valeur appropriée
            short pIdAtelier = 2; // TODO: initialisez à une valeur appropriée
            string pLibelleTheme = "TestUnitaire"; // TODO: initialisez à une valeur appropriée
            target.creerTheme(pIdAtelier, pLibelleTheme);
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }

        /// <summary>
        ///Test pour creerAtelier
        ///</summary>
        [TestMethod()]
        public void creerAtelierTest()
        {
     
            Bdd target = new Bdd("employemdl", "employemdl"); // TODO: initialisez à une valeur appropriée
            string pLibelleAtelier = "TestUnitaire"; // TODO: initialisez à une valeur appropriée
            int pNbPlacesMax = 114; // TODO: initialisez à une valeur appropriée
            string pLibelleTheme = "testunitaire"; // TODO: initialisez à une valeur appropriée
            DateTime pHeureDebVac = Convert.ToDateTime("15/09/2013 12:00:00"); // TODO: initialisez à une valeur appropriée
            DateTime pHeureFinVac = Convert.ToDateTime("15/09/2013 13:30:00"); // TODO: initialisez à une valeur appropriée
            target.creerAtelier(pLibelleAtelier, pNbPlacesMax, pLibelleTheme, pHeureDebVac, pHeureFinVac);
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }

        /// <summary>
        ///Test pour ObtenirDonnesOracleNumVac
        ///</summary>
        [TestMethod()]
        public void ObtenirDonnesOracleNumVacTest()
        {
          
            Bdd target = new Bdd("employemdl", "employemdl"); // TODO: initialisez à une valeur appropriée
            int IdAtelierVac = 2; // TODO: initialisez à une valeur appropriée
            DataTable expected = null; // TODO: initialisez à une valeur appropriée
            DataTable actual;
            actual = target.ObtenirDonnesOracleNumVac(IdAtelierVac);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour ObtenirDonnesOracleAtVac
        ///</summary>
        [TestMethod()]
        public void ObtenirDonnesOracleAtVacTest()
        {
            Bdd target = new Bdd("employemdl","employemdl"); // TODO: initialisez à une valeur appropriée
            DataTable expected = null; // TODO: initialisez à une valeur appropriée
            DataTable actual;
            actual = target.ObtenirDonnesOracleAtVac();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour MajVacation
        ///</summary>
        [TestMethod()]
        public void MajVacationTest()
        {

            Bdd target = new Bdd("employemdl", "employemdl"); // TODO: initialisez à une valeur appropriée
            DateTime pHeureDebutVac = Convert.ToDateTime("15/09/2013 12:00:00"); // TODO: initialisez à une valeur appropriée
            DateTime pHeureFinVac = Convert.ToDateTime("15/09/2013 13:30:00"); // TODO: initialisez à une valeur appropriée
            short pNumeroVac = 2; // TODO: initialisez à une valeur appropriée
            target.MajVacation(pHeureDebutVac, pHeureFinVac, pNumeroVac);
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }
    }
}
