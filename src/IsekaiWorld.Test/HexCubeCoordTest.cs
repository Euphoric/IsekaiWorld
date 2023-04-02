
using FluentAssertions;
using Xunit;

namespace IsekaiWorld;

public class HexCubeCoordTest
{
    [Theory]
    [InlineData(HexagonDirection.Left)]
    [InlineData(HexagonDirection.Right)]
    [InlineData(HexagonDirection.BottomLeft)]
    [InlineData(HexagonDirection.BottomRight)]
    [InlineData(HexagonDirection.TopRight)]
    [InlineData(HexagonDirection.TopLeft)]
    public void Calculate_direction_between_neighbor_cells(HexagonDirection direction)
    {
        var position = HexCubeCoord.Zero;
        var neighbor = position + direction;
        var calculatedDirection = position.DirectionTo(neighbor);
        calculatedDirection.Should().Be(direction);
    }
}