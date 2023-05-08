using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class CharacterTestView
{
    private readonly MessagingEndpoint _messaging;
    
    public String Id { get; }
    public String Label { get; private set; } = null!;
    public HexCubeCoord Position { get; private set; }
    public string ActivityName { get; private set; } = "";
    public double Hunger { get; private set; }

    public List<ItemTestView> CarriedItems { get; } = new();
    
    public CharacterTestView(string id, MessagingEndpoint messaging)
    {
        _messaging = messaging;
        Id = id;
    }

    public void UpdateFrom(CharacterCreated characterCreated)
    {
        Label = characterCreated.Label;
    }
    
    public void UpdateFrom(CharacterUpdated characterUpdated)
    {
        Position = characterUpdated.Position;
        ActivityName = characterUpdated.ActivityName;
        Hunger = characterUpdated.Hunger;
    }

    public void SetHungerTo(double hunger)
    {
        _messaging.Broadcast(new SetCharacterHunger(Id, hunger));
    }

    public void AddCarriedItem(ItemTestView item)
    {
        CarriedItems.Add(item);
    }

    public ItemTestView DropItem(string itemId)
    {
        var item = CarriedItems.First(x => x.Id == itemId);
        CarriedItems.Remove(item);
        return item;
    }

    public bool IsIdle => ActivityName is "" or "IdleActivity";

    public bool IsActive => ActivityName is not ("" or "IdleActivity" or "ThinkingActivity");
}