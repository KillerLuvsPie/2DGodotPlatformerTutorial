using Godot;
using System;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class SavedInputMap : Resource
{
    [Export] public Dictionary<string, Array<InputEvent>> inputList = new Dictionary<string, Array<InputEvent>>();
    
    public string PrintInputs()
    {
        string str = "";
        foreach (var entry in inputList)
        {
            str += "Key: " + entry.Key + " | Values: ";
            for(int i = 0; i < entry.Value.Count(); i++)
            {
                str += entry.Value[i].AsText();
                if(i < entry.Value.Count() - 1)
                    str += " | ";
            }
            str += "\n";
        }
        if(str == "")
            str += "The dictionary is empty";
        return str;
    }
}
