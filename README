QCIntegration is provided as is with no warranty by One Shore (http://one-shore.com) and is free to use or modify as you see fit.

To use QCIntegration.exe, first modify QCIntegration.exe.config to specify the location of your own Quality Center settings.

	qcHostname      - optional, not used in current configuration
	qcUrl           - the URL used to connect to your Quality Center server
	qcDomain        - your Quality Center domain
	qcProject       - your project name in Quality Center
	qcLoginName     - your Quality Center username
	qcPassword      - your Quality Center password
	qcPath          - path to the TestLab folder in QC -- start with "\Root\"
	qcTestSetName   - optional, not used in current configuration
	qcTestName      - optional, not used in current configuration
	testResultsFile - this file contains a mapping of QC Test Names and Statuses
	delimeter       - used when parsing the testResultsFile (typically a comma ",")

The project App.config contains defaults and is compiled to include bogus QC info.  It won't work on your server unless you modify qcHostname, qcUrl, qcDomain, qcProject, qcLogin, and qcPassword.

To change config settings without recompiling, edit QCIntegration.exe.config 

The qcPath will need to be changed for each sprint/iteration/test pass.

If you place your test results file in c:\temp\testresults.csv it should work as is.  The test results should contain the test name and status for each test you wish to update in QC.  See testResults.example.csv for an example format.  

Consider donating to OSSO (http://orphanagesupport.org) if you feel prompted to provide remuneration for this code.  If you'd like to see more like it, I might also be available for consulting work.

If you have any questions, comments, or feedback, feel free to email me <aarone@one-shore.com>
Copyright 2011 Aaron Evans.


