package oneshore.example.qcintegration;

import java.lang.reflect.Method;

import org.junit.After;
import org.junit.Before;
import org.junit.Rule;
import org.junit.rules.TestName;

public class TestBase {
	@Rule public TestName testName = new TestName();
	
	QCTestCases qcTestCases;
	
	@Before
	public void getQCTestCoverage() {
		try {
			Method m = this.getClass().getMethod("testSomething");
			
			if (m.isAnnotationPresent(QCTestCases.class)) {
				qcTestCases = m.getAnnotation(QCTestCases.class);
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
		
		for (String qcTestId : qcTestCases.covered())
		{
			result.append(qcTestId);
			result.append(",");
		}
		
		System.out.println("qc tests covered: " + result);
	}
}
