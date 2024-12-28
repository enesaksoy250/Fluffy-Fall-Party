using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Character Selection/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<GameObject> characters;
}