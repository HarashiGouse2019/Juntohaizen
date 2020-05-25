public interface IDSLCoord
{
    /// <summary>
    /// Total line in the .dsl file
    /// </summary>
    int TotalLines { get; set; }

    /// <summary>
    /// Current Line in the .dsl file
    /// </summary>
    int Line {get; set;}

    /// <summary>
    /// Current Column or position of a line.
    /// </summary>
    int Col { get; set; }

    /// <summary>
    /// Current index of the whole document.
    /// </summary>
    int Index { get; set; }

}
