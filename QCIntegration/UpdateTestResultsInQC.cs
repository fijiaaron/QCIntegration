using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NLog;
using TDAPIOLELib;

namespace oneshore.QCIntegration
{
    class UpdateTestResultsInQC
    {
        public static string qcHostname { get; set; }
        public static string qcUrl { get; set; }
        public static string qcDomain { get; set; }
        public static string qcProject { get; set; }
        public static string qcLoginName { get; set; }
        public static string qcPassword { get; set; }

        public static string qcPath { get; set; }
        public static string qcTestSetName { get; set; }
        public static string qcTestName { get; set; }

        public static string testResultsFile { get; set; }
        public static char DELIMITER { get; set; }

        public static Dictionary<string, string> results;
        private static Logger log = NLog.LogManager.GetCurrentClassLogger();



        static void Main(string[] args)
        {
            log.Debug("starting QCIntegration");

            getConfig();
            results = getTestResultsFromFile(testResultsFile);

            QCController qc = new QCController();
            qc.connectToQC(qcUrl, qcDomain, qcProject, qcLoginName, qcPassword);

            List tsTestSetList = qc.retrieveTestSets(qcPath, qcTestSetName);
            foreach (TestSet testSet in tsTestSetList)
            {
                qc.recordTestSetResults(testSet, results);
            }

            log.Info("total # of tests updated: " + qc.testCount);

            Console.Out.Write("press <NE> key to continue");
            Console.ReadKey();
        }



        /**
         * read configuration data from App.config into variables
         */
        static void getConfig()
        {
            // new (.NET 4.0) wants ConfigurationManager
            // older (.NET 3.5) wants ConfigurationSettings
            qcHostname = ConfigurationManager.AppSettings["qcHostname"];
            qcUrl = ConfigurationManager.AppSettings["qcUrl"];
            qcDomain = ConfigurationManager.AppSettings["qcDomain"];
            qcProject = ConfigurationManager.AppSettings["qcProject"];
            qcLoginName = ConfigurationManager.AppSettings["qcLoginName"];
            qcPassword = ConfigurationManager.AppSettings["qcPassword"];

            qcPath = ConfigurationManager.AppSettings["qcPath"];
            qcTestSetName = ConfigurationManager.AppSettings["qcTestSetName"];
            qcTestName = ConfigurationManager.AppSettings["qcTestName"];

            testResultsFile = ConfigurationManager.AppSettings["testResultsFile"];
            DELIMITER = ConfigurationManager.AppSettings["delimiter"][0];
        }



        /**
         * parse a csv file to get test results
         * 
         * the expected file format is:
         * qcTestName, qcTestStatus
         * 
         * e.g.:
         * TEST_ID_1, Passed
         * 
         * @param string filename
         * @return Dictionary<string, string> 
         */
        static Dictionary<string, string> getTestResultsFromFile(string filename)
        {
            log.Debug("reading file: " + filename);
            Dictionary<string, string> results = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines(filename);

            if (!File.Exists(filename))
            {
                throw new IOException("file doesn't exist: " + filename);
            }

            int lineCount = 0;
            foreach (string line in lines)
            {
                lineCount++;
                log.Debug("line#" + lineCount + ": " + line);
                line.Trim();

                if (line.Length == 0)
                {
                    // skip blank lines
                    log.Debug("skipping blank line #" + lineCount);
                }

                else if (line[0] == '#')
                {
                    // skip comments
                    log.Debug("skipping comment line #" + lineCount);
                }
                else
                {
                    string[] result = line.Split(DELIMITER);

                    if (result.Length != 2)
                    {
                        // skip incorrectly formatted lines
                        log.Warn("skipping incorrectly formatted line " + lineCount);
                    }
                    else
                    {
                        string testName = result[0].Trim();
                        string testStatus = result[1].Trim();

                        log.Debug("testName: " + testName);
                        log.Debug("testStatus: " + testStatus);
                        
                        try
                        {
                            results.Add(testName, testStatus);
                        }
                        catch (ArgumentException e)
                        {
                            log.Error("Exception: " + e.Message + " on line #" + lineCount);
                        }
                    }
                }

            } // end foreach loop

            return results;
        }
    }
}
