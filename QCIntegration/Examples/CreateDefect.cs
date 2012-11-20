using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TDAPIOLELib;

namespace oneshore.QCIntegration.Examples
{
    class CreateDefect
    {
        public CreateDefect()
        {
            TDConnection connection = Connect("qcUrl", "qcDomain", "qcProject", "qcLoginName", "qcPassword");

            BugFactory bugFactory = connection.BugFactory;
            Bug bug = bugFactory.AddItem(null);

            bug.Status = "New";
            bug.Project = "QCIntegration";
            bug.Summary = "Short description of the bug";
            bug.DetectedBy = "Aaron Evans";
            bug.AssignedTo = "Nobody";
            bug.Priority = "Low";
     
            bug.Post();
        }

        public TDConnection Connect(string qcUrl, string qcDomain, string qcProject, string qcLoginName, string qcPassword)
        {
            var connection = new TDConnection();
            connection.InitConnectionEx(qcUrl);
            connection.ConnectProjectEx(qcDomain, qcProject, qcLoginName, qcPassword);

            return connection;
        }
    }
}
