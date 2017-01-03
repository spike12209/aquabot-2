using System;
using static Parca;

class ConditionalHelpers {
	/// If Not
	public static void Unless (bool cond, Action trueBr, Action falseBr = null) {
		DieUnless(trueBr != null, "True branch can't be null.");

		if (!cond) 
			trueBr(); 
		else 
			if (falseBr!=null)
				falseBr();
	}
}
