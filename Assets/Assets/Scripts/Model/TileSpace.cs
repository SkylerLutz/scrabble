public enum TileSpaceType {
	NORMAL,
	CENTER,
	DOUBLE_LETTER_SCORE,
	TRIPLE_LETTER_SCORE,
	DOUBLE_WORD_SCORE,
	TRIPLE_WORD_SCORE
}

public sealed class TileSpace {

	public TileSpaceType type;
	public TileSpace(TileSpaceType type) {
		this.type = type;
	}
}
