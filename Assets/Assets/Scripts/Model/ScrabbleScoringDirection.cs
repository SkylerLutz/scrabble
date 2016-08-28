using System;

public enum ScrabbleScoringDirection {
	HORIZONTAL,
	VERTICAL
}

public class ScrabbleScoringDirectionHelper {
	public static int horizontalDelta(ScrabbleScoringDirection direction) {

		switch(direction) {
		case ScrabbleScoringDirection.HORIZONTAL: return 1;
		case ScrabbleScoringDirection.VERTICAL: return 0;
		}
		//		System.exit(1);

		return 0;
	}
	public static int verticalDelta(ScrabbleScoringDirection direction) {
		switch(direction) {
		case ScrabbleScoringDirection.HORIZONTAL: return 0;
		case ScrabbleScoringDirection.VERTICAL: return 1;
		}
		//		System.exit(1);

		return 0;
	}
	public static ScrabbleScoringDirection orthogonal(ScrabbleScoringDirection direction) {
		switch(direction) {
		case ScrabbleScoringDirection.HORIZONTAL: return ScrabbleScoringDirection.VERTICAL;
		case ScrabbleScoringDirection.VERTICAL: return ScrabbleScoringDirection.HORIZONTAL;
		}
		//		System.exit(1);

		return ScrabbleScoringDirection.HORIZONTAL;

	}
}
