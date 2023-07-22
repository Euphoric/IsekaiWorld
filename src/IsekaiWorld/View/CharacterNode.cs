using Godot;

namespace IsekaiWorld.View;

public partial class CharacterNode : Node2D
{
    private string _direction = "south";
    private string _bodyType = "Male";
    private bool _isDirty = true;
    private string? _clothes;
    private Color _clothesColor = Colors.White;
    private string? _hair;
    private Color _hairColor;

    public string Direction
    {
        get => _direction;
        set
        {
            if (_direction != value)
                _isDirty = true;               
            _direction = value;
        }
    }

    public string BodyType
    {
        get => _bodyType;
        set
        {
            _bodyType = value;
            _isDirty = true;
        }
    }

    public string? Clothes
    {
        get => _clothes;
        set
        {
            _clothes = value;
            _isDirty = true;
        }
    }

    public Color ClothesColor
    {
        get => _clothesColor;
        set
        {
            _clothesColor = value;
            _isDirty = true;
        }
    }

    public string? Hair
    {
        get => _hair;
        set
        {
            _hair = value;
            _isDirty = true;
        }
    }

    public Color HairColor
    {
        get => _hairColor;
        set
        {
            _hairColor = value;
            _isDirty = true;
        }
    }

    public override void _Process(double delta)
    {
        if (!_isDirty)
            return;

        var previousCharacterNode = GetChildOrNull<Node>(0);
        if (previousCharacterNode != null)
            RemoveChild(previousCharacterNode);

        var characterNode = new Node2D();
        var bodyType = BodyType;

        var direction = Direction;
        if (direction == "west")
        {
            direction = "east";
            characterNode.Scale = new Vector2(-1, 1);
        }

        var bodySprite = new Sprite2D();
        bodySprite.Texture =
            ResourceLoader.Load<Texture2D>($"Textures/Character/Bodies/Naked_{bodyType}_{direction}.png");
        var skinColor = new Color("FFDCB1");
        bodySprite.Modulate = skinColor;
        characterNode.AddChild(bodySprite);
        var headNode = new Node2D();
        headNode.Position = new Vector2(0, -32);

        if (Clothes != null)
        {
            var clothesSprite = new Sprite2D();
            clothesSprite.Texture =
                ResourceLoader.Load<Texture2D>($"Textures/Character/Apparel/{Clothes}_{bodyType}_{direction}.png");
            clothesSprite.Modulate = ClothesColor;
            characterNode.AddChild(clothesSprite);
        }

        var headSprite = new Sprite2D();
        headSprite.Texture =
            ResourceLoader.Load<Texture2D>(
                $"Textures/Character/Heads/{bodyType}/{bodyType}_Average_Normal_{direction}.png");
        headSprite.Modulate = skinColor;
        headNode.AddChild(headSprite);
        if (Hair != null)
        {
            var hairSprite = new Sprite2D();
            hairSprite.Texture = ResourceLoader.Load<Texture2D>($"Textures/Character/{Hair}_{direction}.png");
            hairSprite.Modulate = HairColor;
            hairSprite.Scale = new Vector2(128, 128) / hairSprite.Texture.GetSize();
            headNode.AddChild(hairSprite);
        }

        characterNode.AddChild(headNode);

        AddChild(characterNode);

        base._Process(delta);
    }
}