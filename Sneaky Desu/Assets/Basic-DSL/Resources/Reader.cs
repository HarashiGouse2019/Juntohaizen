using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reader : IDSLCoord
{
    
    public const string LEGAL_VARIABLE_CHARACTERS =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890";

    public int Line { get; set; }
    public int line = 0;

    public int Col { get; set; }
    public int col = 0;
    public int TotalLines { get; set; }
    public int totalLines = 0;

    public int Index { get; set; }
    public int index = 0;

}
