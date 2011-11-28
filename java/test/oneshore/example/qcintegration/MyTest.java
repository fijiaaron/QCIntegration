package oneshore.example.qcintegration;

import static org.junit.Assert.*;

import org.junit.Test;

public class MyTest extends TestBase {
	@Test
	@QCTestCases(covered={"QC-TEST-1", "QC-TEST-2"}, related={"QC-TEST-3"})
	public void testSomething() {
		print("in testSomething()");
		assertTrue(false);
	}
}
