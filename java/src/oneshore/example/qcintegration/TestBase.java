package oneshore.example.qcintegration;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;

import org.junit.After;
import org.junit.Before;
import org.junit.Rule;
import org.junit.rules.TestName;

public class TestBase {

	List<String> covered;
	List<String> related;
	
	@Rule public TestName testName = new TestName();
	
	@Before
	public void getQCTestCoverage() {
		print("in getQCTestCoverage()");
		
		
		print("testName: " + testName.getMethodName());
		print("reflected class name: " + this.getClass().getCanonicalName());
		try {
			Method m = this.getClass().getMethod("testSomething");
			print("reflected method name: " + m.getName());
			
			if (m.isAnnotationPresent(QCTestCases.class)) {
				print("found annotation");
				QCTestCases qcTestCases = m.getAnnotation(QCTestCases.class);
				covered = new ArrayList<String>();
				related = new ArrayList<String>();
				
				for (String coveredTestId : qcTestCases.covered())
				{
					print("covered test: " + coveredTestId);
					if (covered == null) {
						covered = new ArrayList<String>();
					}		
					covered.add(coveredTestId);
				}
				for (String relatedTestId : qcTestCases.related())
				{
					print("related test: " + relatedTestId);
					
					if (related == null) {
						related = new ArrayList<String>();
					}
					related.add(relatedTestId);
				}

			} else {
				print("no annotation found");
			}
			
		} catch (SecurityException e) {
			e.printStackTrace();
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
		}
	}
	
	@After
	public void writeCoverageReport() {
		StringBuilder result = new StringBuilder();
		result.append(testName.getMethodName());
		result.append(",");
		for (String qcTestId : covered)
		{
			result.append(qcTestId);
			result.append(",");
		}
		
		print("qc tests covered: " + result);
		
	}
	
	public static void print(String s) {
		System.out.println(s);
	}
}
