using System;
using TDAPIOLELib;

namespace oneshore.QCIntegration.examples
{
    public class RunTests
    {
        public String qcUrl = "http://localhost:8080/qcbin";
        public String qcDomain = "oneshore";
        public String qcProject = "QCIntegration";
        public String qcLoginName = "aaron";
        public String qcPassword = "secret";

        public String testSetPath = @"ROOT\path\to\testset";
        public String testSetName = "TestSet name";

        private TDConnection connection;

        public RunTests()
        {
            Connect(qcUrl, qcDomain, qcProject, qcLoginName, qcPassword);
            TestSet testSet = GetTestSet(testSetPath, testSetName);
            RunTestSet(testSet);
        }

        public TDConnection Connect(string qcUrl, string qcDomain, string qcProject, string qcLoginName, string qcPassword)
        {
            connection = new TDConnection();
            connection.InitConnectionEx(qcUrl);
            connection.ConnectProjectEx(qcDomain, qcProject, qcLoginName, qcPassword);

            return connection;
        }

        public TestSet GetTestSet(String path, String testSetName)
        {
            TestSetFactory testSetFactory = connection.TestSetFactory;
            TestSetTreeManager testSetTreeManager = connection.TestSetTreeManager;

            TestSetFolder testSetFolder = (TestSetFolder)testSetTreeManager.NodeByPath[path];
            List testSetList = testSetFolder.FindTestSets(testSetName);
            TestSet testSet = testSetList[0];

            return testSet;
        }

        public void RunTestSet(TestSet testSet)
        {
            TSScheduler scheduler = testSet.StartExecution("");
            scheduler.RunAllLocally = true;
            scheduler.Run();
        }
    }
}
