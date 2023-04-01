using Godot;
using System;

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
			_direction = value;
			_isDirty = true;
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
		set { _hairColor = value; _isDirty = true; }
	}

	public override void _Ready()
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
		bodySprite.Texture = ResourceLoader.Load<Texture2D>($"Textures/Character/Bodies/Naked_{bodyType}_{direction}.png");
		var skinColor = new Color("FFDCB1");
		bodySprite.Modulate = skinColor;
		characterNode.AddChild(bodySprite);
		var headNode = new Node2D();
		headNode.Position = new Vector2(0, -32);

		var clothesSprite = new Sprite2D();
		clothesSprite.Texture =
			ResourceLoader.Load<Texture2D>($"Textures/Character/Apparel/{Clothes}_{bodyType}_{direction}.png");
		clothesSprite.Modulate = ClothesColor;
		characterNode.AddChild(clothesSprite);

		var headSprite = new Sprite2D();
		headSprite.Texture = ResourceLoader.Load<Texture2D>($"Textures/Character/Heads/{bodyType}/{bodyType}_Average_Normal_{direction}.png");
		headSprite.Modulate = skinColor;
		headNode.AddChild(headSprite);
		var hairSprite = new Sprite2D();
		hairSprite.Texture = ResourceLoader.Load<Texture2D>($"Textures/Character/{Hair}_{direction}.png");
		hairSprite.Modulate = HairColor;
		hairSprite.Scale = new Vector2(128, 128) / hairSprite.Texture.GetSize();
		headNode.AddChild(hairSprite);
		characterNode.AddChild(headNode);		
		
		AddChild(characterNode);
	}
}

public partial class CharacterTest : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		{
			var bodyType = "Female";
			var clothes = "DameDress/DameDress";
			var clothesColor = new Color("9efff5");
			var hair = "Hair_Female/AFUf08";
			var hairColor = new Color("FAF0BE");
			
			var characterNode = new CharacterNode();
			characterNode.Direction = "west";
			characterNode.BodyType = bodyType;
			characterNode.Clothes = clothes;
			characterNode.ClothesColor = clothesColor;
			characterNode.Hair = hair;
			characterNode.HairColor = hairColor;
			characterNode.Position = new Vector2(100 + 75 * 0, 350);
			AddChild(characterNode);
			
			var characterNode1 = new CharacterNode();
			characterNode1.Direction = "south";
			characterNode1.BodyType = bodyType;
			characterNode1.Clothes = clothes;
			characterNode1.ClothesColor = clothesColor;
			characterNode1.Hair = hair;
			characterNode1.HairColor = hairColor;
			characterNode1.Position = new Vector2(100 + 75 * 1, 350);
			AddChild(characterNode1);
			
			var characterNode2 = new CharacterNode();
			characterNode2.Direction = "east";
			characterNode2.BodyType = bodyType;
			characterNode2.Clothes = clothes;
			characterNode2.ClothesColor = clothesColor;
			characterNode2.Hair = hair;
			characterNode2.HairColor = hairColor;
			characterNode2.Position = new Vector2(100 + 75 * 2, 350);
			AddChild(characterNode2);
			
			var characterNode3 = new CharacterNode();
			characterNode3.Direction = "north";
			characterNode3.BodyType = bodyType;
			characterNode3.Clothes = clothes;
			characterNode3.ClothesColor = clothesColor;
			characterNode3.Hair = hair;
			characterNode3.HairColor = hairColor;
			characterNode3.Position = new Vector2(100 + 75 * 3, 350);
			AddChild(characterNode3);
		}
		{
			var bodyType = "Male";
			var clothes = "ShirtBasic/ShirtBasic";
			var clothesColor = new Color("cfcfcf");
			var hair = "Hair_Male/AFUm03";
			var hairColor = new Color("bb6d3e");
			
			var characterNode = new CharacterNode();
			characterNode.Direction = "west";
			characterNode.BodyType = bodyType;
			characterNode.Clothes = clothes;
			characterNode.ClothesColor = clothesColor;
			characterNode.Hair = hair;
			characterNode.HairColor = hairColor;
			characterNode.Position = new Vector2(100 + 75 * 0, 500);
			AddChild(characterNode);
			
			var characterNode1 = new CharacterNode();
			characterNode1.Direction = "south";
			characterNode1.BodyType = bodyType;
			characterNode1.Clothes = clothes;
			characterNode1.ClothesColor = clothesColor;
			characterNode1.Hair = hair;
			characterNode1.HairColor = hairColor;
			characterNode1.Position = new Vector2(100 + 75 * 1, 500);
			AddChild(characterNode1);
			
			var characterNode2 = new CharacterNode();
			characterNode2.Direction = "east";
			characterNode2.BodyType = bodyType;
			characterNode2.Clothes = clothes;
			characterNode2.ClothesColor = clothesColor;
			characterNode2.Hair = hair;
			characterNode2.HairColor = hairColor;
			characterNode2.Position = new Vector2(100 + 75 * 2, 500);
			AddChild(characterNode2);
			
			var characterNode3 = new CharacterNode();
			characterNode3.Direction = "north";
			characterNode3.BodyType = bodyType;
			characterNode3.Clothes = clothes;
			characterNode3.ClothesColor = clothesColor;
			characterNode3.Hair = hair;
			characterNode3.HairColor = hairColor;
			characterNode3.Position = new Vector2(100 + 75 * 3, 500);
			AddChild(characterNode3);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
