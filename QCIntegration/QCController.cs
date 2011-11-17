using System;
using System.Collections.Generic;
using System.Reflection;
using TDAPIOLELib;
using NLog;

namespace oneshore.QCIntegration
{
    class QCController
    {
        public string qcHostname { get; set; }
        public string qcDomain { get; set; }
        public string qcProject { get; set; }
        public string qcLoginName { get; set; }
        public string qcPassword { get; set; }
        public string qcUrl
        {
            get
            {
                if (qcUrl == null) { return "http://" + qcHostname + ":8080/qcbin"; }
                else { return qcUrl; }
            }
            set { qcUrl = value; }
        }

        public static string DELIM = "\t";
        public int testCount { get; set; }

        private TDConnection tdConn;
        private string message;

        private static Logger log = NLog.LogManager.GetCurrentClassLogger();

        /**
         * Constructor creates the Connection object
         * 
         */
        public QCController()
        {
            try
            {
                this.tdConn = createTDConnection();
            }
            catch (QCException e)
            {
                log.Error(e.Message);
                throw;
            }
        }



        /**
		 * create a new TDConnection object or throw QCException on failure
		 * 
		 */
        public TDConnection createTDConnection()
        {
            log.Debug("Creating TD connection.");

            TDConnection tdConn = new TDConnection();

            if (tdConn == null)
            {
                message = "Can't create TDConnection object -- maybe you need to reference TDAPIOLELib?";
                throw new QCException(message);
            }

            return tdConn;
        }



        /**
         * Establish a connection to Quality Center
         * 
         * @param string qcUrl
         * @param string qcDomain
         * @param string qcProject
         * @param string qcLoginName
         * @param string qcPassword
         * @return TDConnection
         */
        public TDConnection connectToQC(string qcUrl, string qcDomain, string qcProject, string qcLoginName, string qcPassword)
        {
            try
            {
                tdConn.InitConnectionEx(qcUrl);
                tdConn.ConnectProjectEx(qcDomain, qcProject, qcLoginName, qcPassword);
            }
            catch (Exception e)
            {
                log.Warn("unable to connect to project in QC");
                log.Warn(e.Message);
                throw;
            }

            if (tdConn.Connected)
            {
                message = "connected";

                if (tdConn.ProjectConnected)
                {
                    message += " to QC project " + tdConn.ProjectName;
                    log.Info(message);
                }
                else
                {
                    message += " to QC but unable to connect to QC Project " + qcProject;
                    log.Warn(message);
                    throw new QCException(message);
                }
            }
            else
            {
                message = "failed to connect to QC";
                log.Warn(message);
                throw new QCException(message);
            }

            return tdConn;
        }



        /**
         * find the test sets that will be updated in QC
         * 
         * @param string tsPath
         * @param string tsName
         * @return List of TestSet
        */
        public List retrieveTestSets(string tsPath, string tsName)
        {
            log.Debug("...in retriveTestSet() " + tsPath + " " + tsName);

            TestSetFolder tsFolder = retrieveFolder(tsPath);

            if (tsFolder == null)
            {
                throw new QCException("no TestSetFolder at " + tsPath);
            }

            List tsList = tsFolder.FindTestSets(tsName, false, null);

            if (tsList == null)
            {
                throw new QCException("no TestSets matching " + tsName);
            }

            if (tsList.Count < 1)
            {
                throw new QCException("no TestSets found matching " + tsName);
            }

            return tsList;
        }



        /***
         * get one or more tests that matches a specific test case name
         * 
         * @param string tsFolderName
         * @param string tsTestName
         * @return List of TsTest
         */
        public List retrieveTests(string tsFolderName, string tsTestName)
        {
            TestSetFolder tsFolder = retrieveFolder(tsFolderName);
            log.Debug("tsFolder.Path: " + tsFolder.Path);

            List tsTestList = tsFolder.FindTestInstances(tsTestName, false, null);
            return tsTestList;
        }



        /**
         * get a folder in QC
         * 
         * @param string tsPath
         * @return TestSetFolder
         */
        public TestSetFolder retrieveFolder(string tsPath)
        {
            TestSetFactory tsFactory = (TestSetFactory)tdConn.TestSetFactory;
            TestSetTreeManager tsTreeMgr = (TestSetTreeManager)tdConn.TestSetTreeManager;

            TestSetFolder tsFolder = null;
            try
            {
                tsFolder = (TestSetFolder)tsTreeMgr.get_NodeByPath(tsPath);
            }
            catch (Exception e)
            {
                log.Warn("couldn't get TestSetFolder with path " + tsPath);
                log.Warn(e.Message);
            }

            return tsFolder;
        }


        /**
		 * set status for tests in a test set and update in QC
		 * 
		 * @param TestSet testSet
         * @param Dictionary<string, string> testResults - testCaseName, testResult (e.g. "EHR_REF_PAT_0001", "Passed")
		 */
        public void recordTestSetResults(TestSet testSet, Dictionary<string, string> testResults)
        {
            TestSetFolder tsFolder = (TestSetFolder)testSet.TestSetFolder;
            log.Debug("tsFolder.Path: " + tsFolder.Path);

            string testSetInfo = "testSet.ID: " + testSet.ID.ToString() + DELIM +
                                 "testSet.Name: " + testSet.Name + DELIM +
                                 "testSet.Status: " + testSet.Status + DELIM +
                                 "";

            log.Debug("testSetInfo: " + testSetInfo);

            TSTestFactory tsTestFactory = (TSTestFactory)testSet.TSTestFactory;
            List tsTestList = tsTestFactory.NewList("");

            foreach (TSTest tsTest in tsTestList)
            {
                testCount++;

                string testInfo = DELIM + DELIM + DELIM +
                                "TestId: " + tsTest.TestId + DELIM +
                                "TestName: " + tsTest.TestName + DELIM +
                                "";

                Run lastRun = (Run)tsTest.LastRun;
                if (lastRun != null)
                {
                    testInfo += lastRun.Name + DELIM + lastRun.Status;
                }

                log.Debug("TestInfo: " + testInfo);

                // look for a test in the results from this test set
                if (testResults.ContainsKey(tsTest.TestName))
                {
                    string status = testResults[tsTest.TestName];
                    recordTestResult(tsTest, status);
                }

            }
        }



        /**
         * set status for a single test and update in QC
         * 
         * @param TSTest test - the test to be updated
         * @param string status - "Passed", "Failed", etc.
         */
        public void recordTestResult(TSTest test, string status)
        {
            string testInfo = DELIM + DELIM + DELIM +
                "TestId: " + test.TestId + DELIM +
                "TestName: " + test.TestName + DELIM +
                "";

            Run lastRun = (Run)test.LastRun;
            if (lastRun != null)
            {
                testInfo += lastRun.Name + DELIM + lastRun.Status;
            }

            log.Debug(testInfo);

            RunFactory runFactory = (RunFactory)test.RunFactory;
            String date = DateTime.Now.ToString("yyyyMMddhhmmss");
            Run run = (Run)runFactory.AddItem("Run" + date);
            run.Status = status;
            run.Post();
        }
    }
}
