namespace IsekaiWorld;

public readonly struct HexOffsetCoord
{
    public int Column { get; }
    public int Row { get; }

    public HexOffsetCoord(int column, int row)
    {
        Column = column;
        Row = row;
    }

    public HexCubeCoord ToCube()
    {
        var q = Column - (Row - (Row & 1)) / 2;
        var r = Row;
        return new HexCubeCoord(r, q, -q - r);
    }
}